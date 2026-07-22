# NewLife.IoT — Bibliothèque Standard IoT &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** est la bibliothèque standard IoT — définissant des contrats de protocole de communication unifiés pour le domaine IoT. **Aucune implémentation concrète de protocole.**

| Plateforme | Pilote | Conception clé |
|:---|:---|:---|
| Consomme `IDriver`/`INode`/`IDevice` — une API pour tout | Implémente `DriverBase` | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, plateformes personnalisées | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` avec Quality/Timestamp, `WriteResult` struct |
| `ThingSpec` — propriétés/événements/services | `ITransport` — série/TCP/UDP/HTTP unifié | Diagnostic zéro allocation |

| Source | NuGet | Documents |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [Exigences](Doc/需求文档.md) · [Fonctionnalités](Doc/功能清单.md) · [Architecture](Doc/架构设计.md) |

---

## Matrice de Projets NewLife / Project Matrix

Frameworks : `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| Projet | Année | Description |
|:-------|:-----:|:------------|
| **Fondation** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Bibliothèque centrale — logging, config, cache, réseau, sérialisation, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | ORM Big Data — 10Md+ lignes/table, MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Bibliothèque réseau — 22,66M tps, 4M connexions TCP simultanées |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | Framework RPC — Http/RPC, haut débit, compatible IoT |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Plateforme de développement rapide Cube — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | Gestionnaire de services — Windows Service / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero scaffold — modèles Web/WebApi/Service |
| **Middleware** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Client Redis — latence microseconde, débit millionnaire |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | Client RocketMQ — Apache et Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **Bibliothèque Standard IoT — interfaces de pilote unifiées et spécification de modèle de chose** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Protocole Siemens PLC |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | Composants cartographiques — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | Codec audio — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **Produits** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Plateforme de services distribués — gestion de nœuds, APM, centre de configuration |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | Plateforme de calcul big data distribué |
| FIoT Plateforme IoT | 2020 | Solution IoT totale — 100K+ points par nœud |
| UWB Positionnement Intérieur | 2020 | Positionnement centimétrique (10~20cm) |

---

## Équipe NewLife / Team

![XCode](https://newlifex.com/logo.png)

L'équipe NewLife, fondée en 2002, est un fournisseur de solutions IoT pour la nouvelle ère.  
80+ projets open-source, 4M+ téléchargements NuGet.

| Site web | Open Source | Communauté |
|----------|------------|------------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
