An .env file is needed with following data:
The values themselves are not that important, since we use and set them during docker-compose, only CATAPI_TOKEN is needed to be acquired from 

CATAPI_TOKEN=... // CatAPI_Token
SA_PASSWORD=... // SQL Database Password
MINIO_ROOT_USER=...
MINIO_ROOT_PASSWORD=...

http://localhost:5000/hangfire opens Hangfire Console
http://localhost:5000/swagger/index.html opens Swagger
http://localhost:5000/api... for requests
http://127.0.0.1:9001/browser/imagebucket opens minIO

Initial implementation was storing the images on MSSQL, was extremelly non performant.

### RUN ONCE: 
docker compose up --build migrations
 **it waits for the DB to come online**

### Afterwards:
docker compose up --build webapp
 
### To always recreate DB:
docker compose up --build

## Authentication
I consider Authentication to be out of scope since it was not in the requirements.
If it were to implement it, I would use JWT header based.

## https://thecatapi.com/ API Token
Without an API token, always 10 results are returned (query params are disabled).
I currently force existence of API Token, that enforcement can be disabled at [line 99 of Program.cs](https://github.com/papakaliati/StealAllTheCats/blob/b577d63f83a9c4f0cfdd85f86abb422d23833010/src/Program.cs#L99)

[The Cat API Tokens](https://thecatapi.com/#pricing) are free for up to 10.000 requests per month 