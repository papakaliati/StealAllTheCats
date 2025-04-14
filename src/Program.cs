using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Polly.Extensions.Http;
using Polly;
using Refit;
using StealAllTheCats.Infrastructure.Repositories;
using StealAllTheCats.Middlewares;
using StealAllTheCats.Services;
using StealAllTheCats.Integrations.TheCatApi;
using System.Text.Json;
using Microsoft.Data.SqlClient;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


// --- DATABASE SETUP ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/// On production enviroments I would be using a separate fan-out service (Running on Azure function App / AWS Lambda) for handling the background job processing.
/// I would be feeding the data using a queue Service (Azure Service bus) to feed the background tasks. 
/// The background tasks progress would be probably stored on Redis and then written on the DB.
/// For this scenario Hangfire seems like a good option to use
/// I will be using it with local MS SQL storage which wouldnt need additional dependencies, 
/// and for somewhat better performance Redis.
// --- HANGFIRE SETUP ---
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// --- DEPENDENCY INJECTION ---


// Register as interfaces to allow segregation
builder.Services.AddScoped<ICatDBContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ITagDBContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ICatTagDBContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddScoped<ICatRepository, CatRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddScoped<ICatService, CatService>();
builder.Services.AddScoped<IFileDownloader, FileDownloader>();

// --- CONTROLLERS + SWAGGER ---
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
        opts.JsonSerializerOptions.WriteIndented = true;
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // Enable Swagger annotations
});
builder.Services.AddHttpClient(); // for IHttpClientFactory

builder.Services.AddRefitClient<ICatApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://api.thecatapi.com");
        c.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["CatApi:ApiKey"] ?? 
                throw new InvalidConfigurationException("CatApi:ApiKey in configuration is missing or invalid"));
    })
    .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
    .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy());

WebApplication app = builder.Build();

// --- MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapHangfireDashboard();
app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();  // Add custom exception handling middleware

app.Run();


public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy() =>
        Policy.TimeoutAsync<HttpResponseMessage>(10); // 10 seconds
}
