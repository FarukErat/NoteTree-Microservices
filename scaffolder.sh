set -eu

PROJECT_NAME=$1

( mkdir ${PROJECT_NAME} && cd ${PROJECT_NAME} # Project Directory

    ( mkdir src && cd src # Source Directory

        # Source Solution
        dotnet new sln -n "${PROJECT_NAME}"

        # Project Layers
        dotnet new classlib -n "${PROJECT_NAME}.Domain"
        dotnet new classlib -n "${PROJECT_NAME}.Application"
        dotnet new classlib -n "${PROJECT_NAME}.Infrastructure"
        dotnet new console -n "${PROJECT_NAME}.Presentation"

        # Add Projects to the Solution
        dotnet sln add "${PROJECT_NAME}.Domain/${PROJECT_NAME}.Domain.csproj"
        dotnet sln add "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj"
        dotnet sln add "${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj"
        dotnet sln add "${PROJECT_NAME}.Presentation/${PROJECT_NAME}.Presentation.csproj"

        # Add Project References
        dotnet add "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj" reference "${PROJECT_NAME}.Domain/${PROJECT_NAME}.Domain.csproj"
        dotnet add "${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj" reference "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj"
        dotnet add "${PROJECT_NAME}.Presentation/${PROJECT_NAME}.Presentation.csproj" reference "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj"
        dotnet add "${PROJECT_NAME}.Presentation/${PROJECT_NAME}.Presentation.csproj" reference "${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj"

        # Application Dependencies
        dotnet add "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj" package ErrorOr
        dotnet add "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj" package FluentValidation
        dotnet add "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj" package FluentValidation.AspNetCore
        dotnet add "${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj" package MediatR

        # Infrastructure Dependencies
        dotnet add "${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj" package Microsoft.EntityFrameworkCore
        dotnet add "${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj" package Microsoft.EntityFrameworkCore.Design
        dotnet add "${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj" package Microsoft.EntityFrameworkCore.Tools
    )

)
