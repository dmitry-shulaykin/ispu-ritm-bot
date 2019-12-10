FROM microsoft/dotnet:3.1-sdk-alpine AS build
WORKDIR /app

# copy passface server csproj files
COPY . /GradesNotification

# copy everything else and build an app
COPY ["./PassFaceServer/.","."]
WORKDIR "/app/PassFace"
RUN dotnet publish "PassFace.csproj" -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
WORKDIR /app
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY WebClient/dist/Client/* /app/client-app/
COPY --from=build /app/PassFace/out ./
ENTRYPOINT ["dotnet", "PassFace.dll"]
