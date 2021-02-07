FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT="Production"
#RUN echo "deb http://deb.debian.org/debian bullseye main" >> /etc/apt/sources.list
#RUN apt-get update
#RUN apt-get install ca-certificates
#RUN sed -i '$ d' /etc/apt/sources.list
#RUN apt-get update
# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet publish Pakkasyohaaste.sln -c Release -o out
#RUN  dotnet publish --runtime alpine-x64 -c Release --self-contained true -o ./out /p:PublishSingleFile=false /p:PublishTrimmed=true
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0.2-alpine3.12
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT="Production"
ENV NO_SSL=1
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "./Pakkasyohaaste.dll"]