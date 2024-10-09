FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18 AS build
WORKDIR /dashboard
COPY src/dashboard .
RUN ls -alL
RUN dotnet publish -o /artifacts

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.18
LABEL org.opencontainers.image.source https://github.com/nikolamilosa/dashboard
WORKDIR /dashboard
COPY --from=build /artifacts .
# 👇 set to use the non-root USER here
USER $APP_UID
ENTRYPOINT ["./dashboard"]
