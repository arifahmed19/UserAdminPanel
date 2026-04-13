FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY ["UserAdminPanel.csproj", "./"]
RUN dotnet restore "./UserAdminPanel.csproj"

COPY . .
RUN dotnet publish "UserAdminPanel.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "UserAdminPanel.dll"]
