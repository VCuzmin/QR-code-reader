#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/QRCodeReader.Api/QRCodeReader.Api.csproj", "src/QRCodeReader.Api/"]
COPY ["src/Application/QRCodeReader.Application/QRCodeReader.Application.csproj", "src/Application/QRCodeReader.Application/"]
RUN dotnet restore "src/QRCodeReader.Api/QRCodeReader.Api.csproj"
COPY . .
WORKDIR "/src/src/QRCodeReader.Api"
RUN dotnet build "QRCodeReader.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QRCodeReader.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QRCodeReader.Api.dll"]