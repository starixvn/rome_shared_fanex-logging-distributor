# syntax=docker/dockerfile:1
FROM hub.nexdev.net/base/dotnet/core/sdk:6.0 AS builder
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./src/Fanex.Logging.Distributor/Fanex.Logging.Distributor.csproj ./
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

# Copy everything else and build
COPY ./src/Fanex.Logging.Distributor .
RUN dotnet publish -r linux-x64 --self-contained false -o out

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

# Build runtime image
WORKDIR /app
COPY --from=builder /app/out .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Logging.Distributor.dll"]