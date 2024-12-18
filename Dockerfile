FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN cd Chess-Final.Blazor
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080
WORKDIR /App/Chess-Final.Blazor
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "Chess-Final.Blazor.dll"]