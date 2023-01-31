#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
RUN apt update && \
    apt install unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /vsdbg
COPY --from=build /vsdbg .
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "NexusFit.Auth.API.dll" ]
