An .env file is needed with following data:

CATAPI_TOKEN=... // CatAPI_Token
SA_PASSWORD=... // SQL Database Password

http://localhost:5000/hangfire opens Hangfire Console
http://localhost:5000/swagger/index.html opens Swagger

### RUN ONCE: 
docker compose up --build migrations
 **Slow process, because it waits for the DB to come online**

### Afterwards:
docker compose up --build webapp
 
### To always recreate DB:
docker compose up --build

## Authentication
I consider Authentication to be out of scope since it was not in the requirements.
If it were to implement it, I would use JWT header based.