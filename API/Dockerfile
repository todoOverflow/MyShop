#build env
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS  build-env

#work dir inside container, commads run in
WORKDIR /app
EXPOSE 8080

#copy csproj to workdir and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

#copy everything from host(PC) to workdir(container) and build
#-c means --configuration -o means --output
COPY . ./
RUN dotnet publish -c Release -o out

#build a runtime image
#copy from previous workdir out folder to new workdir
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "API.dll" ]