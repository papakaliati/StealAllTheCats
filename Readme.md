# StealAllTheCats
A reference .NET application implementing StealAllTheCats Solution


## Getting Started
This version is based on .NET 8.


#### Prerequisites
* Clone the repository: https://github.com/papakaliati/StealAllTheCats
* Install & start Docker Desktop


An .env file is needed with following data (example being file [.env.example](https://github.com/papakaliati/StealAllTheCats/blob/main/.env.example)):

* CATAPI_TOKEN= // CatAPI_Token
* SA_PASSWORD= // SQL Database Password
* MINIO_ROOT_USER=
* MINIO_ROOT_PASSWORD=

*The values themselves are not that important, since we use and set them during docker-compose, only CATAPI_TOKEN being an external token dependency*


## Running the solution

### Run DB Migrations
DB Migrations need to be run once before the service is started

*docker compose up --build migrations*

 **it waits for the DB to come online**

### Run Main Service 
After running migrations and every time afterwards to start the service:
*docker compose up --build webapp*
 
### "On-click" deploy
Will recreate DB and run migrations, but will run the migration scripts every time - Slower startup time
*docker compose up --build*

#### Note:
I would also split the MinIO bucket creation to a new docker compose job, but it doesn't affect much the starting time of the webapp service, just throws a bucket already created error, so i choose to ignore it. Tech_debt++


### After running the application: 
Peak through 
[Swagger](http://localhost:5000/swagger/index.html)


## Authentication
I consider Authentication to be out of scope since it was not in the requirements.
If it were to implement it, I would use JWT header based.

### https://thecatapi.com/ API Token
Without an API token, always 10 results are returned (query params are disabled).
I currently force existence of API Token, that enforcement can be disabled at [line 99 of Program.cs](https://github.com/papakaliati/StealAllTheCats/blob/b577d63f83a9c4f0cfdd85f86abb422d23833010/src/Program.cs#L99)

[The Cat API Tokens](https://thecatapi.com/#pricing) are free for up to 10.000 requests per month

## Tech Stack
* NET 8 - A free, multi/cross-platform and open-source framework;
* EF Core 8 - An open source object–relational mapping framework for ADO.NET;

Initial implementation of storing the images as binary data on MSSQL, was extremelly non performant. Switched to MinIO based solution for file storing.

### MSSQL
Microsoft SQL Server is a proprietary relational database management system developed by Microsoft using Structured Query Language (SQL, often pronounced "sequel").

https://www.microsoft.com/en-us/sql-server

### MinIO

MinIO is an object storage system released under GNU Affero General Public License v3.0.[3] It is API compatible with the Amazon S3 cloud storage service. It is capable of working with unstructured data such as photos, videos, log files, backups, and container images with the maximum supported object size being 50TB.[4]

https://min.io/

[Local MinIO UI](http://127.0.0.1:9001/browser)

### Hangfire
An easy way to perform background processing in .NET and .NET Core applications. No Windows Service or separate process required. In our case the MSSQL is used as persistence storage. Redis implementation would be couple of time faster

https://www.hangfire.io/

[Local Hangfire Console](http://localhost:5000/hangfire)


