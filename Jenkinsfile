pipeline {
    agent any
    environment {
        dotnet = '/usr/bin/dotnet'
    }
    stages {
        stage('Checkout Stage') {
            steps {
                git credentialsId: '27cf9409-2da4-4586-9caa-47915268b156', url: 'https://github.com/WinsomeQuill/WebApp.git', branch: 'main'
            }
        }
        stage('Build Stage') {
            steps {
                sh 'dotnet build WebApp.sln --configuration Release'
            }
        }
        stage('Test Stage') {
            steps {
                sh 'dotnet test'
            }
        }
    }
}