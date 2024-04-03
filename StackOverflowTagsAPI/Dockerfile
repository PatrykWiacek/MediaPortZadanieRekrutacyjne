FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StackOverflowTagsAPI/StackOverflowTagsAPI.csproj", "StackOverflowTagsAPI/"]
COPY ["StackOverflowTagsApi.BLL/StackOverflowTagsApi.BLL.csproj", "StackOverflowTagsApi.BLL/"]
RUN dotnet restore "StackOverflowTagsAPI/StackOverflowTagsAPI.csproj"
COPY . .
WORKDIR "/src/StackOverflowTagsAPI"
RUN dotnet build "StackOverflowTagsAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StackOverflowTagsAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackOverflowTagsAPI.dll"]
