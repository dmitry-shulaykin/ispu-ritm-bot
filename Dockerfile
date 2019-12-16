FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /app
RUN apk add --update nodejs npm 
# copy passface server csproj files
COPY . /app

# copy everything else and build an app
RUN dotnet publish "GradesNotification.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY ClientApp/build/ /app/ClientApp/build/
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "GradesNotification.dll"]
