FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7148

ENV ASPNETCORE_URLS=http://+:7148
ENV PORT=7148

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["BudgetTracker.MinimalAPI/BudgetTracker.MinimalAPI.csproj", "BudgetTracker.MinimalAPI/"]
COPY ["ClassLib/ClassLib.csproj", "ClassLib/"]
RUN pwd && ls
RUN ls BudgetTracker.MinimalAPI/
RUN dotnet restore "BudgetTracker.MinimalAPI/BudgetTracker.MinimalAPI.csproj"
COPY . .
WORKDIR "/src/BudgetTracker.MinimalAPI"
RUN pwd && ls
RUN dotnet build "BudgetTracker.MinimalAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BudgetTracker.MinimalAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetTracker.MinimalAPI.dll"]
