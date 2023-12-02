# ASP.NET Core 8 Dockerized Application with Jenkins CI/CD

This repository contains a sample ASP.NET Core 8 web application that is containerized using Docker and has continuous integration and continuous deployment (CI/CD) set up with Jenkins.

# Tools used:
- [.NET SDK 8](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/)
- [Jenkins](https://www.jenkins.io/download/)

# Getting Started
``` bash
# Clone repository
https://github.com/WinsomeQuill/WebApp.git
cd WebApp
# Run project
dotnet run --project WebApp
```
# Using Docker
``` bash
docker-compose up --build -d
```
The application will be accessible at http://localhost:8080.

Swagger will be accessible at http://localhost:8080/swagger.

# Using Jenkins
Jenkins CI/CD Setup

- Install the necessary plugins in Jenkins:
    - Git
    - Pipeline

Use ```Jenkinsfile``` for pipeline job in Jenkins.

Save the pipeline configuration.

Trigger a build to test the CI/CD pipeline.

# Folder Structure
- **WebApp** - ASP.NET Core application
  - **Dockerfile** and **docker-compose.yaml** - Defines the Docker image for the application.
- **WebAppTests** - Testing
- **Jenkinsfile** - Pipeline