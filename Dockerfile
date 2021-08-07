FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS="http://*:5000"

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MyStromRelay.csproj", "./"]
RUN dotnet restore "MyStromRelay.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MyStromRelay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyStromRelay.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyStromRelay.dll"]
