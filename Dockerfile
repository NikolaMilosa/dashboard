FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18 AS build
WORKDIR /dashboard
COPY src/dashboard .
RUN ls -alL
RUN dotnet publish -o /artifacts

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.18
LABEL org.opencontainers.image.source https://github.com/nikolamilosa/dashboard

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    TZ=Europe/Belgrade

RUN apk add --no-cache \
    icu-data-full \
    icu-libs \
    tzdata

WORKDIR /dashboard
COPY --from=build /artifacts .

ENTRYPOINT ["./dashboard"]
