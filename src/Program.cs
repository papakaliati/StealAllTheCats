using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.Configuration;
using Polly.Extensions.Http;
using Polly;
using Refit;
using StealAllTheCats.Integrations.TheCatApi;
using System.Text.Json;
using StealAllTheCats.Api.Middlewares;
using StealAllTheCats.Infrastructure.Data.Repositories;
using StealAllTheCats.Api.Features.Cats.Fetch;
using StealAllTheCats.Api.Features.Cats.GetbyID;
using StealAllTheCats.Api.Features.Cats.ListCats;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;

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
    //.UseMemoryStorage()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"),
                         new SqlServerStorageOptions
                         {
                             PrepareSchemaIfNecessary = true
                         })
    );
builder.Services.AddHangfireServer();

// --- DEPENDENCY INJECTION ---


// Register as interfaces to allow segregation
builder.Services.AddScoped<ICatDBContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ITagDBContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<ICatTagDBContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddScoped<ICatRepository, CatRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IFileDownloader, FileDownloader>();

builder.Services.AddScoped<IFetchCatsService, FetchCatsService>();
builder.Services.AddScoped<IGetCatByIdService, GetCatByIdService>();
builder.Services.AddScoped<IListCatsService, ListCatsService > ();

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
        c.DefaultRequestHeaders.Add("x-api-key", builder.Configuration["CATAPI_APIKEY"] ?? 
                throw new InvalidConfigurationException("CATAPI_APIKEY in configuration is missing or invalid"));
    })
    .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
    .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy());

WebApplication app = builder.Build();

// --- MIDDLEWARE ---
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();
//app.UseHttpsRedirection();
//app.UseHangfireDashboard("/hangfire", 
//                         new DashboardOptions {
//                             Authorization = []
//                         });
app.MapHangfireDashboard(new DashboardOptions {
                             Authorization = []
                         });
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
