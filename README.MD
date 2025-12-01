\# SantanderTest API



This project is a .NET 9 Web API running in Docker, using Redis as a supporting service.  

The application exposes its HTTP endpoints through Swagger UI and can be easily executed using Docker Compose.



---



\## Prerequisites



Before running the application, make sure you have installed:



\- \*\*Docker\*\*  

\- \*\*Docker Compose\*\*



---



\## How to Run the Application



1\. Open a terminal in the \*\*root folder of the Project (src folder)\*\* (where the `docker-compose.yml` file is located).



2\. Run the application using:



&nbsp;  ```bash

&nbsp;  docker compose up --build



3\. Open your browser and go to:

&nbsp;   http://localhost8080/swagger
    This will load the Swagger UI where you can test all endpoints.

