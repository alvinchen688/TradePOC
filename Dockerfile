# 使用官方 .NET SDK 镜像作为构建阶段的基础镜像
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 将解决方案文件复制到容器中
COPY ["TradePOC.slnx", "."]
COPY ["TradePOC.App/TradePOC.App.csproj", "TradePOC.App/"]
COPY ["TradePOC.Console/TradePOC.Console.csproj", "TradePOC.Console/"]
COPY ["TradePOC.Domain/TradePOC.Domain.csproj", "TradePOC.Domain/"]
COPY ["TradePOC.Infrastructure/TradePOC.Infrastructure.csproj", "TradePOC.Infrastructure/"]

# 恢复 NuGet 包
RUN dotnet restore "TradePOC.Console/TradePOC.Console.csproj"

# 复制所有源代码
COPY . .

# 发布应用程序
RUN dotnet publish "TradePOC.Console/TradePOC.Console.csproj" -c Release -o out

# 使用更小的 .NET Runtime 镜像作为最终镜像
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/out .

# 设置容器启动时运行的应用程序
ENTRYPOINT ["dotnet", "TradePOC.Console.dll"]