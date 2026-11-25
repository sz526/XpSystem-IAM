# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 先拷 .csproj 做 restore，利用 Docker 层缓存（改代码不会重装 NuGet）
COPY XpSystem/XpSystem.csproj XpSystem/
COPY XpSystem.Api/XpSystem.Api.csproj XpSystem.Api/
RUN dotnet restore XpSystem.Api/XpSystem.Api.csproj

# 拷源码并发布
COPY . .
WORKDIR /src/XpSystem.Api
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# curl 留给 compose 里的 healthcheck 调用 /health
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# API 在容器内监听 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "XpSystem.Api.dll"]

# - **两段式**：SDK 镜像负责编译发布，aspnet 镜像只跑成品（不带编译器，镜像小）。
# - `COPY . .` 后 `dotnet publish` 出 Release 产物到 `/app/publish`，运行阶段从 build 阶段拷过来。
# - `EXPOSE 8080` 只是声明；真正端口映射靠 `docker run -p` / compose。