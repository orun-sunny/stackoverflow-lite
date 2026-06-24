FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["StackOverflowLite.Host/StackOverflowLite.Host.csproj", "StackOverflowLite.Host/"]
COPY ["StackOverflowLite.Application/StackOverflowLite.Application.csproj", "StackOverflowLite.Application/"]
COPY ["StackOverflowLite.Infrastructure/StackOverflowLite.Infrastructure.csproj", "StackOverflowLite.Infrastructure/"]
COPY ["StackOverflowLite.Domain/StackOverflowLite.Domain.csproj", "StackOverflowLite.Domain/"]
RUN dotnet restore "StackOverflowLite.Host/StackOverflowLite.Host.csproj"
COPY . .
WORKDIR "/src/StackOverflowLite.Host"
RUN dotnet build "StackOverflowLite.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StackOverflowLite.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StackOverflowLite.Host.dll"]
