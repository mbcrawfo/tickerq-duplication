FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# See https://github.com/NuGet/Home/issues/13062
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

WORKDIR /build/Web
COPY Web/Web.csproj .
RUN dotnet restore Web.csproj

COPY ./Web .
RUN dotnet build Web.csproj -c Release

WORKDIR /build/Worker
COPY Worker/Worker.csproj .
RUN dotnet restore Worker.csproj

COPY Worker .
RUN dotnet publish Worker.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Worker.dll"]
