#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster as build
WORKDIR /app
EXPOSE 80
EXPOSE 443

# copy csproj and restore as distinct layers
COPY *.sln .
COPY API/*.csproj ./API/
COPY BLL/*.csproj ./BLL/
COPY BLL.Test/*.csproj ./BLL.Test/
COPY DAL/*.csproj ./DAL/
COPY External/*.csproj ./External/
COPY Scheduler/*.csproj ./Scheduler/

#restore dependencies
RUN dotnet restore

# copy everything else and build app
COPY API/ ./API/
COPY BLL/ ./BLL/
COPY BLL.Test/ ./BLL.Test/
COPY DAL/ ./DAL/
COPY External/ ./External/
COPY Scheduler/ ./Scheduler/

#
WORKDIR /app/API
RUN dotnet publish -c Release -o out 
#
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS runtime
WORKDIR /app 
#
COPY --from=build /app/API/out ./
ENTRYPOINT ["dotnet", "API.dll"]