# NewLife.IoT — IoT 标准库 &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** 是物联网标准库——为 IoT 领域定义通信协议**统一契约**，不含具体协议实现。目标是成为 IoT 平台与硬件驱动之间的**单一抽象层**。

| 平台侧 | 驱动侧 | 设计亮点 |
|:---|:---|:---|
| 引用 `IDriver`/`INode`/`IDevice` —— 一套 API 操作所有硬件 | 继承 `DriverBase` —— 接入平台生态 | 异步优先 `Task<T>` + `CancellationToken` |
| 星尘、ZeroIoT、FIoT、自定义平台 | NewLife.Modbus、Siemens、OPC-UA、BACnet | `ReadResult` 携带质量码/时间戳，`WriteResult` 零堆分配 |
| `ThingSpec` 物模型 —— 属性/事件/服务 | `ITransport` —— 统一串口/TCP/UDP/HTTP | 诊断帧开关，生产环境零开销 |

| 源码 | NuGet | 文档 |
|------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [需求文档](Doc/需求文档.md) · [功能清单](Doc/功能清单.md) · [架构设计](Doc/架构设计.md) |

---

## 新生命项目矩阵 / Project Matrix

支持框架: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| 项目 | 年 | 说明 |
|:-----|:--:|:-----|
| **基础组件 / Foundation** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | 核心库 — 日志、配置、缓存、网络、序列化、APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | 大数据中间件 — 单表百亿级，MySql/SQLite/SqlServer/Oracle/PostgreSql/达梦 |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | 网络库 — 千万级吞吐（2266万tps），百万级连接（400万Tcp） |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | 协议通信库 — Http/RPC，高吞吐，物联网低开销 |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | 魔方快速开发平台 — 用户权限、SSO、OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | 服务管理 — Windows服务/Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero脚手架 — Web/WebApi/Service 项目模板 |
| **中间件 / Middleware** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redis客户端 — 微秒级延迟，百万级吞吐 |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | RocketMQ客户端 — 支持Apache/阿里云 |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient/MqttServer |
| **NewLife.IoT** | 2022 | **IoT标准库 — 统一驱动接口与物模型规范** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | 西门子 PLC 协议 |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | 地图组件 — 百度/高德/腾讯/天地图 |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | 音频编解码 — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **产品平台 / Products** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | 星尘分布式平台 — 节点管理、APM、配置中心、注册中心 |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | 蚂蚁调度 — 分布式大数据计算平台 |
| [NewLife.ERP](https://github.com/NewLifeX/NewLife.ERP) | 2021 | 企业ERP |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | 2006 | 码神工具集 |
| [EasyIO](https://github.com/NewLifeX/EasyIO) | 2023 | 简易文件存储 |
| [XProxy](https://github.com/NewLifeX/XProxy) | 2005 | 反向代理 — NAT/Http |
| [HttpMeter](https://github.com/NewLifeX/HttpMeter) | 2022 | Http压力测试 |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | 2015 | Git源码管理 |
| [SmartOS](https://github.com/NewLifeX/SmartOS) | 2014 | 嵌入式操作系统 — ARM Cortex-M |
| [SmartA2](https://github.com/NewLifeX/SmartA2) | 2019 | 嵌入式工业计算机 — 物联网边缘网关 |
| FIoT 物联网平台 | 2020 | 物联网整体解决方案 — 单机十万级点位 |
| UWB 高精度室内定位 | 2020 | 厘米级定位（10~20cm） |

---

## 新生命开发团队 / Team

![XCode](https://newlifex.com/logo.png)

新生命团队（NewLife）成立于2002年，是新时代物联网行业解决方案提供者。  
团队主导的80多个开源项目已被广泛应用于各行业，NuGet累计下载量高达400余万次。  
`新生命团队始于2002年，部分开源项目具有20年以上漫长历史，源码库保留有2010年以来所有修改记录`

| 网站 | 开源 | 社区 |
|------|------|------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ群: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
