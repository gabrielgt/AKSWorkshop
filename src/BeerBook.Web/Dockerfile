FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["BeerBook.Web/BeerBook.Web.csproj", "BeerBook.Web/"]
RUN dotnet restore "BeerBook.Web/BeerBook.Web.csproj"
COPY . .
WORKDIR "/src/BeerBook.Web"
RUN dotnet build "BeerBook.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BeerBook.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BeerBook.Web.dll"]