# NewLife.IoT — IoT Standard Library &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** is the IoT standard library — defining unified communication protocol contracts for the Internet of Things field. **No concrete protocol implementations.** Built as the single abstraction layer between IoT platforms and hardware drivers.

| Platform Side | Driver Side | Key Design |
|:---|:---|:---|
| Consume `IDriver`/`INode`/`IDevice` — one API for all hardware | Implement `DriverBase` — plug into the ecosystem | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, custom platforms | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` with Quality/Timestamp, `WriteResult` struct |
| `ThingSpec` thing model — properties/events/services | `ITransport` — serial/TCP/UDP/HTTP unified | Zero-allocation diagnostics, production switch |

| Source | NuGet | Docs |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [Requirements](Doc/需求文档.md) · [Features](Doc/功能清单.md) · [Architecture](Doc/架构设计.md) |

---

## 新生命项目矩阵 / Project Matrix

Targets: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| Project | Year | Description |
|:--------|:----:|:------------|
| **Foundation** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Core library — logging, config, cache, networking, serialization, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | Big data ORM — 10B+ rows/table, MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Network library — 22.66M tps, 4M concurrent TCP connections |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | RPC framework — Http/RPC, high throughput, IoT-friendly |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Cube rapid dev platform — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | Service manager — Windows Service / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero scaffold — Web/WebApi/Service project templates |
| **Middleware** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redis client — microsecond latency, million-level throughput |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | RocketMQ client — Apache & Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **IoT Standard Library — unified driver interfaces & thing model spec** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Siemens PLC protocol |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | Map components — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | Audio codec — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **Products** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Distributed service platform — node management, APM, config center |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | Distributed big data computing platform |
| [NewLife.ERP](https://github.com/NewLifeX/NewLife.ERP) | 2021 | Enterprise ERP |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | 2006 | Developer toolset |
| [EasyIO](https://github.com/NewLifeX/EasyIO) | 2023 | Simple file storage |
| [XProxy](https://github.com/NewLifeX/XProxy) | 2005 | Reverse proxy — NAT/Http |
| [HttpMeter](https://github.com/NewLifeX/HttpMeter) | 2022 | HTTP stress testing |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | 2015 | Git source management |
| [SmartOS](https://github.com/NewLifeX/SmartOS) | 2014 | Embedded OS — ARM Cortex-M |
| [SmartA2](https://github.com/NewLifeX/SmartA2) | 2019 | Embedded industrial computer — IoT edge gateway |
| FIoT IoT Platform | 2020 | IoT total solution — 100K+ points per node |
| UWB Indoor Positioning | 2020 | Centimeter-level positioning (10~20cm) |

---

## 新生命开发团队 / Team

![XCode](https://newlifex.com/logo.png)

NewLife team, founded in 2002, is an IoT solutions provider for the new era.  
80+ open-source projects widely adopted across industries, 4M+ NuGet downloads.

| Website | Open Source | Community |
|---------|------------|-----------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
