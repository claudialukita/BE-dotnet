#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster as build
WORKDIR /app
EXPOSE 80
EXPOSE 443

# copy csproj and restore as distinct layers
COPY *.sln .
COPY OB-BE-dotnet/*.csproj ./OB-BE-dotnet/
COPY ClassLibrary1/*.csproj ./ClassLibrary1/
COPY BLL.Test/*.csproj ./BLL.Test/
COPY DAL/*.csproj ./DAL/
# COPY External/*.csproj ./External/
COPY Scheduler/*.csproj ./Scheduler/

#restore dependencies
RUN dotnet restore

# copy everything else and build app
COPY OB-BE-dotnet/ ./OB-BE-dotnet/
COPY ClassLibrary1/ ./ClassLibrary1/
COPY BLL.Test/ ./BLL.Test/
COPY DAL/ ./DAL/
# COPY External/ ./External/
COPY Scheduler/ ./Scheduler/

#
WORKDIR /app/OB-BE-dotnet
RUN dotnet publish -c Release -o out 
#
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS runtime
WORKDIR /app 
#
COPY --from=build /app/OB-BE-dotnet/out ./
ENTRYPOINT ["dotnet", "API.dll"]