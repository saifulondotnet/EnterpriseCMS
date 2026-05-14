FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/EnterpriseCMS.Web/EnterpriseCMS.Web.csproj", "src/EnterpriseCMS.Web/"]
COPY ["src/EnterpriseCMS.Application/EnterpriseCMS.Application.csproj", "src/EnterpriseCMS.Application/"]
COPY ["src/EnterpriseCMS.Core/EnterpriseCMS.Core.csproj", "src/EnterpriseCMS.Core/"]
COPY ["src/EnterpriseCMS.Infrastructure/EnterpriseCMS.Infrastructure.csproj", "src/EnterpriseCMS.Infrastructure/"]
COPY ["src/EnterpriseCMS.Plugins.Core/EnterpriseCMS.Plugins.Core.csproj", "src/EnterpriseCMS.Plugins.Core/"]
RUN dotnet restore "src/EnterpriseCMS.Web/EnterpriseCMS.Web.csproj"
COPY . .
WORKDIR "/src/src/EnterpriseCMS.Web"
RUN dotnet build "EnterpriseCMS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EnterpriseCMS.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Create uploads directory
RUN mkdir -p /app/wwwroot/uploads && chmod 777 /app/wwwroot/uploads

ENV ASPNETCORE_URLS=http://+:8080
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EnterpriseCMS.Web.dll"]
