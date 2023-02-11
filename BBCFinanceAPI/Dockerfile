FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BBCFinanceAPI/BBCFinanceAPI.csproj", "BBCFinanceAPI/"]
RUN dotnet restore "BBCFinanceAPI/BBCFinanceAPI.csproj"
COPY . .
WORKDIR "/src/BBCFinanceAPI"
RUN dotnet build "BBCFinanceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BBCFinanceAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BBCFinanceAPI.dll"]
