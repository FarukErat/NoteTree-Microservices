set -eu

PROJECT_NAME=$1

( mkdir ${PROJECT_NAME} && cd ${PROJECT_NAME} # Project Directory

    # Project Solution
    dotnet new sln -n "${PROJECT_NAME}"

    ( mkdir src && cd src # Source Directory

        # Project Layers
        dotnet new classlib -n "${PROJECT_NAME}.Domain"
        dotnet new classlib -n "${PROJECT_NAME}.Application"
        dotnet new classlib -n "${PROJECT_NAME}.Infrastructure"
        dotnet new console -n "${PROJECT_NAME}.Presentation"

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

    ( mkdir tests && cd tests # Test Directory

        # Test Projects
        dotnet new xunit -n "${PROJECT_NAME}.Domain.Tests"
        dotnet new xunit -n "${PROJECT_NAME}.Application.Tests"
        dotnet new xunit -n "${PROJECT_NAME}.Infrastructure.Tests"
        dotnet new xunit -n "${PROJECT_NAME}.Presentation.Tests"

        # Add Test Project References
        dotnet add "${PROJECT_NAME}.Domain.Tests/${PROJECT_NAME}.Domain.Tests.csproj" reference "../src/${PROJECT_NAME}.Domain/${PROJECT_NAME}.Domain.csproj"
        dotnet add "${PROJECT_NAME}.Application.Tests/${PROJECT_NAME}.Application.Tests.csproj" reference "../src/${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj"
        dotnet add "${PROJECT_NAME}.Infrastructure.Tests/${PROJECT_NAME}.Infrastructure.Tests.csproj" reference "../src/${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj"
        dotnet add "${PROJECT_NAME}.Presentation.Tests/${PROJECT_NAME}.Presentation.Tests.csproj" reference "../src/${PROJECT_NAME}.Presentation/${PROJECT_NAME}.Presentation.csproj"

    )

    # Add Source to the Solution
    dotnet sln add "src/${PROJECT_NAME}.Domain/${PROJECT_NAME}.Domain.csproj"
    dotnet sln add "src/${PROJECT_NAME}.Application/${PROJECT_NAME}.Application.csproj"
    dotnet sln add "src/${PROJECT_NAME}.Infrastructure/${PROJECT_NAME}.Infrastructure.csproj"
    dotnet sln add "src/${PROJECT_NAME}.Presentation/${PROJECT_NAME}.Presentation.csproj"

    # Add Tests to the Solution
    dotnet sln add "tests/${PROJECT_NAME}.Domain.Tests/${PROJECT_NAME}.Domain.Tests.csproj"
    dotnet sln add "tests/${PROJECT_NAME}.Application.Tests/${PROJECT_NAME}.Application.Tests.csproj"
    dotnet sln add "tests/${PROJECT_NAME}.Infrastructure.Tests/${PROJECT_NAME}.Infrastructure.Tests.csproj"
    dotnet sln add "tests/${PROJECT_NAME}.Presentation.Tests/${PROJECT_NAME}.Presentation.Tests.csproj"

)
