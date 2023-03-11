FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /build

COPY "DemoRDS/DemoRDS.csproj" "DemoRDS/DemoRDS.csproj"

RUN dotnet restore "DemoRDS/DemoRDS.csproj"

COPY . .

RUN dotnet publish "DemoRDS/DemoRDS.csproj" -o /published-app -c Release --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app

COPY --from=build /published-app ./

ENTRYPOINT ["dotnet", "DemoRDS.dll"]