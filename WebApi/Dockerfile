# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia e restaura os pacotes
COPY *.csproj ./
RUN dotnet restore

# Copia o restante e publica
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Define porta que o Render usará
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Substitua pelo nome correto do seu DLL
ENTRYPOINT ["dotnet", "WebApi.dll"]