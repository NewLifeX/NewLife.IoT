# NewLife.IoT — Biblioteca Estándar IoT &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** es la biblioteca estándar de IoT — define contratos de protocolo de comunicación unificados para el campo IoT. **Sin implementaciones concretas de protocolo.** Construida como la capa de abstracción única entre plataformas IoT y controladores de hardware.

| Plataforma | Controlador | Diseño clave |
|:---|:---|:---|
| Consume `IDriver`/`INode`/`IDevice` — una API para todo | Implementa `DriverBase` | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, plataformas personalizadas | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` con Quality/Timestamp, `WriteResult` struct |
| `ThingSpec` — propiedades/eventos/servicios | `ITransport` — serie/TCP/UDP/HTTP unificado | Diagnóstico sin asignación, switch de producción |

| Fuente | NuGet | Documentos |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [Requisitos](Doc/需求文档.md) · [Funciones](Doc/功能清单.md) · [Arquitectura](Doc/架构设计.md) |

---

## Matriz de Proyectos NewLife / Project Matrix

Frameworks: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| Proyecto | Año | Descripción |
|:---------|:---:|:------------|
| **Fundación** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Biblioteca central — logging, config, caché, red, serialización, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | ORM Big Data — 10B+ filas/tabla, MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Biblioteca de red — 22.66M tps, 4M conexiones TCP concurrentes |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | Framework RPC — Http/RPC, alto rendimiento, IoT-friendly |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Plataforma de desarrollo rápido Cube — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | Gestor de servicios — Windows Service / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero scaffold — plantillas Web/WebApi/Service |
| **Middleware** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Cliente Redis — latencia de microsegundos, rendimiento millonario |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | Cliente RocketMQ — Apache y Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **Biblioteca Estándar IoT — interfaces de controlador unificadas y especificación de modelo de cosa** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Protocolo Siemens PLC |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | Componentes de mapa — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | Códec de audio — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **Productos** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Plataforma de servicios distribuidos — gestión de nodos, APM, centro de configuración |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | Plataforma de computación big data distribuida |
| FIoT Plataforma IoT | 2020 | Solución total IoT — 100K+ puntos por nodo |
| UWB Posicionamiento Interior | 2020 | Posicionamiento de nivel centimétrico (10~20cm) |

---

## Equipo NewLife / Team

![XCode](https://newlifex.com/logo.png)

El equipo NewLife, fundado en 2002, es un proveedor de soluciones IoT para la nueva era.  
80+ proyectos open-source, 4M+ descargas NuGet.

| Sitio web | Open Source | Comunidad |
|-----------|------------|-----------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
