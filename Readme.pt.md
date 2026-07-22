# NewLife.IoT — Biblioteca Padrão IoT &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** é a biblioteca padrão de IoT — define contratos de protocolo de comunicação unificados para o campo IoT. **Sem implementações concretas de protocolo.**

| Plataforma | Driver | Design principal |
|:---|:---|:---|
| Consome `IDriver`/`INode`/`IDevice` — uma API para tudo | Implementa `DriverBase` | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, plataformas personalizadas | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` com Quality/Timestamp, `WriteResult` struct |
| `ThingSpec` — propriedades/eventos/serviços | `ITransport` — serial/TCP/UDP/HTTP unificado | Diagnóstico sem alocação |

| Fonte | NuGet | Documentos |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [Requisitos](Doc/需求文档.md) · [Funcionalidades](Doc/功能清单.md) · [Arquitetura](Doc/架构设计.md) |

---

## Matriz de Projetos NewLife / Project Matrix

Frameworks: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| Projeto | Ano | Descrição |
|:--------|:---:|:----------|
| **Fundação** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Biblioteca central — logging, config, cache, rede, serialização, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | ORM Big Data — 10Bi+ linhas/tabela, MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Biblioteca de rede — 22,66M tps, 4M conexões TCP simultâneas |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | Framework RPC — Http/RPC, alta vazão, compatível com IoT |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Plataforma de desenvolvimento rápido Cube — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | Gerenciador de serviços — Windows Service / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero scaffold — templates Web/WebApi/Service |
| **Middleware** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Cliente Redis — latência de microssegundos, vazão milionária |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | Cliente RocketMQ — Apache e Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **Biblioteca Padrão IoT — interfaces de driver unificadas e especificação de modelo de coisa** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Protocolo Siemens PLC |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | Componentes de mapa — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | Codec de áudio — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **Produtos** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Plataforma de serviços distribuídos — gestão de nós, APM, centro de configuração |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | Plataforma de computação big data distribuída |
| FIoT Plataforma IoT | 2020 | Solução IoT total — 100K+ pontos por nó |
| UWB Posicionamento Interior | 2020 | Posicionamento centimétrico (10~20cm) |

---

## Equipe NewLife / Team

![XCode](https://newlifex.com/logo.png)

A equipe NewLife, fundada em 2002, é fornecedora de soluções IoT para a nova era.  
80+ projetos open-source, 4M+ downloads NuGet.

| Site | Open Source | Comunidade |
|------|------------|------------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
