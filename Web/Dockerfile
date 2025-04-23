FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build

# See https://github.com/NuGet/Home/issues/13062
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

COPY Web.csproj .
RUN dotnet restore Web.csproj

COPY . .
RUN dotnet publish Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
EXPOSE 8080
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
