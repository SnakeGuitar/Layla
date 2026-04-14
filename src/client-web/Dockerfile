# -------- BUILD --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get update \
    && apt-get install -y nodejs \
    && npm install -g pnpm \
    && rm -rf /var/lib/apt/lists/*

COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile

COPY . .
ENV CI=true
RUN pnpm run build:frontend

# Certificados
RUN mkdir -p /app/certs \
    && dotnet dev-certs https -ep /app/certs/aspnetapp.pfx -p YourStrongPasswordAF

# Publish
RUN dotnet publish "client-web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- RUNTIME --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 7126
EXPOSE 5233

COPY --from=build /app/publish .
COPY --from=build /app/certs /app/certs

ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certs/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourStrongPasswordAF

ENTRYPOINT ["dotnet", "client-web.dll"]