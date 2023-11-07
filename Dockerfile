#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7148
EXPOSE 5103

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BudgetTracker.MinimalAPI/BudgetTracker.MinimalAPI.csproj", "BudgetTracker.MinimalAPI/"]
COPY ["ClassLib/ClassLib.csproj", "ClassLib/"]
RUN dotnet restore "BudgetTracker.MinimalAPI/BudgetTracker.MinimalAPI.csproj"
COPY . .
WORKDIR "/src/BudgetTracker.MinimalAPI"
RUN dotnet build "BudgetTracker.MinimalAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BudgetTracker.MinimalAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BudgetTracker.MinimalAPI.dll"]