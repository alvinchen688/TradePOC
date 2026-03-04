# TradePOC

TradePOC 是一个基于 .NET 10.0 的高性能交易对账系统概念验证项目，采用现代化架构设计和最佳实践。

## 解决方案架构

### 项目结构
- **TradePOC.Console**: 控制台应用程序入口，负责启动服务和模拟高并发交易请求
- **TradePOC.App**: 应用层，包含命令处理、业务流程协调等
- **TradePOC.Domain**: 领域层，定义业务实体、值对象和领域服务
- **TradePOC.Infrastructure**: 基础设施层，提供数据访问、缓存、消息队列等技术实现
- **TradePOC.Tests**: 单元测试和集成测试

### 核心特性
- **CQRS模式**: 使用MediatR实现命令查询职责分离
- **依赖注入**: 通过Microsoft.Extensions.DependencyInjection管理对象生命周期
- **高并发处理**: 支持模拟高并发交易对账请求
- **事件驱动**: 基于领域事件的异步处理机制
- **内存缓存**: 集成MemoryCache提升性能
- **消息队列**: 支持RabbitMQ进行异步通信

### 技术栈
- .NET 10.0
- MediatR (CQRS)
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting
- RabbitMQ (消息队列)
- MemoryCache (缓存)

### 功能演示
程序启动后会自动模拟1000个并发交易对账请求，验证系统的高并发处理能力。

### Docker部署
项目已配置Docker支持，可通过以下命令构建和运行：
```bash
# 构建镜像
docker build -t tradepoc-console .

# 运行容器
docker run tradepoc-console
```