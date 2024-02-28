FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QuietRoom.Server/QuietRoom.Server.csproj", "QuietRoom.Server/"]
RUN dotnet restore "QuietRoom.Server/QuietRoom.Server.csproj"
COPY . .
WORKDIR "/src/QuietRoom.Server"
RUN dotnet build "QuietRoom.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuietRoom.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuietRoom.Server.dll"]
