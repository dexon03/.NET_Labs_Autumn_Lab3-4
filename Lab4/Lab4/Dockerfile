FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Lab4/Lab4.csproj", "Lab4/"]
RUN dotnet restore "Lab4/Lab4.csproj"
COPY . .
WORKDIR "/src/Lab4"
RUN dotnet build "Lab4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lab4.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lab4.dll"]
