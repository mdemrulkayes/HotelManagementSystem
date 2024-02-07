FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /src
COPY HotelManagementSystem.BlazorWasm.csproj .
RUN dotnet restore HotelManagementSystem.BlazorWasm.csproj
COPY . .
RUN dotnet build HotelManagementSystem.BlazorWasm.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish HotelManagementSystem.BlazorWasm.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf