#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Backend/Commands/Crm.Commands.API/Crm.Commands.API.csproj", "src/Backend/Commands/Crm.Commands.API/"]
RUN dotnet restore "src/Backend/Commands/Crm.Commands.API/Crm.Commands.API.csproj"
COPY . .
WORKDIR "/src/src/Backend/Commands/Crm.Commands.API"
RUN dotnet build "Crm.Commands.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crm.Commands.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crm.Commands.API.dll"]