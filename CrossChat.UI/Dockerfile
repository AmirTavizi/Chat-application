#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["CrossChat.UI/CrossChat.UI.csproj", "CrossChat.UI/"]
COPY ["CrossChat.Domain.Common/CrossChat.Domain.Common.csproj", "CrossChat.Domain.Common/"]
COPY ["CrossChat.Services/CrossChat.Services.csproj", "CrossChat.Services/"]
COPY ["CrossChat.Domain/CrossChat.Domain.csproj", "CrossChat.Domain/"]
COPY ["CrossChat.DataAccess/CrossChat.DataAccess.csproj", "CrossChat.DataAccess/"]
RUN dotnet restore "CrossChat.UI/CrossChat.UI.csproj"
COPY . .
WORKDIR "/src/CrossChat.UI"
RUN dotnet build "CrossChat.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CrossChat.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrossChat.UI.dll"]