FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia toda a solution (incluindo os projetos referenciados)
COPY . ./

# Restaura dependências
RUN dotnet restore ./WebApi/WebApi.csproj

# Publica o projeto principal
RUN dotnet publish ./WebApi/WebApi.csproj -c Release -o out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "WebApi.dll"]