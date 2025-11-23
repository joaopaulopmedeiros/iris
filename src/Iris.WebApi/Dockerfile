FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5164

ENV ASPNETCORE_URLS=http://+:5164

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Iris.WebApi/Iris.WebApi.csproj", "src/Iris.WebApi/"]
RUN dotnet restore "src/Iris.WebApi/Iris.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Iris.WebApi"
RUN dotnet build "Iris.WebApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Iris.WebApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Iris.WebApi.dll"]
