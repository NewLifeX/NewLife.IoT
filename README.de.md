# NewLife.IoT — IoT-Standardbibliothek &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)
[![test](https://github.com/NewLifeX/NewLife.IoT/actions/workflows/test.yml/badge.svg?branch=master)](https://github.com/NewLifeX/NewLife.IoT/actions/workflows/test.yml)
[![NativeAOT](https://github.com/NewLifeX/NewLife.IoT/actions/workflows/aot.yml/badge.svg?branch=master)](https://github.com/NewLifeX/NewLife.IoT/actions/workflows/aot.yml)
![TFM](https://img.shields.io/badge/TFM-8%20%28net45%20%E2%86%92%20net10.0%29-512BD4?logo=dotnet)
![locales](https://img.shields.io/badge/locales-31-2ea44f?logo=dotnet)
[![platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS%20%7C%20Android%20%7C%20iOS-blue)](Doc/架构设计.md#14-平台与指令集支持矩阵)
[![arch](https://img.shields.io/badge/arch-x64%20%7C%20x86%20%7C%20arm64%20%7C%20arm%20%7C%20loongarch64%20%7C%20riscv64%20%7C%20s390x-orange)](Doc/架构设计.md#14-平台与指令集支持矩阵)
[![contributors](https://img.shields.io/github/contributors/newlifex/newlife.iot?logo=github)](https://github.com/NewLifeX/NewLife.IoT/graphs/contributors)

<!-- lang:start -->
🌐 [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [Español](README.es.md) | [Français](README.fr.md) | [हिन्दी](README.hi.md) | [日本語](README.ja.md) | [Português](README.pt.md) | [Русский](README.ru.md) | [한국어](README.ko.md)
<!-- lang:end -->

---

**NewLife.IoT** ist die IoT-Standardbibliothek — sie definiert einheitliche Verträge für Kommunikationsprotokolle im IoT-Umfeld. **Keine konkreten Protokollimplementierungen.** Sie ist die einzige Abstraktionsschicht zwischen IoT-Plattformen und Hardwaredrivern.

| Plattformseite | Treiberseite | Zentrale Entwurfsentscheidungen |
|:---|:---|:---|
| `IDriver`/`INode`/`IDevice` nutzen — eine API für alle Hardware | `DriverBase` ableiten — in das Ökosystem einsteigen | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, eigene Plattformen | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` mit Quality/Timestamp, `WriteResult` als Struktur |
| `ThingSpec`-Datenmodell — Eigenschaften/Ereignisse/Dienste | `ITransport` — seriell/TCP/UDP/HTTP vereinheitlicht | Diagnose-Rahmen abschaltbar, produktiv ohne Overhead |

| Quelle | NuGet | Dokumentation |
|------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [Anforderungen](Doc/需求文档.md) · [Funktionsliste](Doc/功能清单.md) · [Architektur](Doc/架构设计.md) |

**Laufzeitfunktionen**: Lokalisierung in 31 Sprachen (abgestimmt auf NewLife.Core) · protokollierte Instanzpräfixe · NativeAOT verifiziert (20 Szenarien, Windows/Linux/macOS) · 8 Zielframeworks (net45 → net10.0).

---

## Ökosystem-Landkarte

NewLife ist ein geschichtetes Ökosystem unabhängig versionierter Open-Source-Projekte. Die folgende Landkarte ist in allen NewLife-Repositories wortgleich; jedes Projekt verlinkt sein Repository und sein Haupt-NuGet-Paket.

| Projekt | Kurzbeschreibung | Downloads |
| --- | --- | --- |
| **Fundament** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | Protokollierung, Konfiguration, Caching, Netzwerk, Serialisierung, Zeitplanung und APM-Tracing. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Core?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Core) |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | Datenmiddleware: einzelne Tabellen mit zig Milliarden Zeilen, automatisches Sharding, Lese-/Schreibtrennung, über 10 Datenbanken. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCode?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCode) |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | Hochleistungs-Netzwerkbibliothek: 1,77 Mio. pps / 14,5 Gbps auf einem Rechner (TCP 1 KB, 2026-10), 4 Mio. langlebige TCP-Verbindungen. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Net?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Net) |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | Protokollkommunikation und RPC-Framework für Client-Server-Anwendungen und IoT-Geräte. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Remoting?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Remoting) |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | Installiert .NET-Anwendungen als Betriebssystemdienste: Windows-Dienste und Linux-systemd. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Agent?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Agent) |
| [NewLife.Templates](https://github.com/NewLifeX/NewLife.Zero) | `dotnet new`-Projektvorlagen für Konsole, Web, API, Dienst, Daten und IoT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Templates?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Templates) |
| **Daten & Messaging** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | Redis-Client mit Mikrosekunden-Latenz, Millionen Operationen pro Sekunde und eingebauten Nachrichtenwarteschlangen. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Redis?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Redis) |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | Reiner Managed-RocketMQ-Client für Apache RocketMQ und Alibaba Cloud Message Queue. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.RocketMQ?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.RocketMQ) |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | MQTT-Client und -Server, einschließlich Alibaba-Cloud-IoT-Anbindung. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MQTT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MQTT) |
| [NewLife.NovaDb](https://github.com/NewLifeX/NewLife.NovaDb) | Hybriddatenbank in C#: eingebettet oder im Netzwerk, relational, Zeitreihen, Nachrichtenwarteschlange und KV in einem. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.NovaDb?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.NovaDb) |
| [NewLife.MySql](https://github.com/NewLifeX/NewLife.MySql) | MySql-Treiber und Hilfsschicht auf dem NewLife-Datenstack. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MySql?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MySql) |
| **Web, KI & Verteiltes** | | |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | Schnelle Admin-Plattform: Berechtigungen, SSO, OAuth-Server und CRUD-Gerüst. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Cube?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Cube) |
| [Stardust](https://github.com/NewLifeX/Stardust) | Verteiltes Dienstplatform: Knotenverwaltung, APM, Konfigurationscenter, Registry und Release-Center. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Stardust?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Stardust) |
| [AntJob](https://github.com/NewLifeX/AntJob) | Verteiltes Aufgaben- und Echtzeit-Datenplatform mit Sharding, Wiederholungen und Backfill. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AntJob?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AntJob) |
| [NewLife.AI](https://github.com/NewLifeX/NewLife.AI) | Einheitlicher Client für 46 LLM-Anbieter plus ChatAI, eine sofort nutzbare Web-Chat-Anwendung. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AI?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AI) |
| **Industrie & IoT** | | |
| [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT) | IoT-Standardbibliothek mit Protokollverträgen und Geräteabstraktionen. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IoT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IoT) |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | Modbus TCP/RTU/ASCII auf Basis von NewLife.IoT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Modbus?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Modbus) |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | Siemens-SPS-Protokoll auf Basis von NewLife.IoT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Siemens?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Siemens) |
| [NewLife.Omron](https://github.com/NewLifeX/NewLife.Omron) | Omron-SPS-Protokoll auf Basis von NewLife.IoT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Omron?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Omron) |
| [NewLife.Melsec](https://github.com/NewLifeX/NewLife.Melsec) | Mitsubishi-Melsec-SPS-Protokoll auf Basis von NewLife.IoT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Melsec?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Melsec) |
| [NewLife.OPC](https://github.com/NewLifeX/NewLife.OPC) | OPC-Client für industriellen Datenzugriff. | — |
| [NewLife.BACnet](https://github.com/NewLifeX/NewLife.BACnet) | BACnet-Protokollstack für die Gebäudeautomation. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.BACnet?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.BACnet) |
| [ZeroIoT](https://github.com/NewLifeX/ZeroIoT) | Leichtgewichtige Open-Source-IoT-Plattform mit Gateway und Geräteverwaltung. | — |
| **Fahrzeug-Telematik** | | |
| [NewLife.JT808](https://github.com/NewLifeX/NewLife.JT808) | JT/T 808 Fahrzeugortungsprotokoll: über 70 Nachrichtentypen, Versionen 2011/2013/2019. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT808?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT808) |
| [NewLife.JT809](https://github.com/NewLifeX/NewLife.JT809) | JT/T 809 Plattform-zu-Plattform-Protokoll für Verkehrsbehörden. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT809?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT809) |
| [NewLife.JT1078](https://github.com/NewLifeX/NewLife.JT1078) | JT/T 1078 Fahrzeugvideo: RTP-Parsing, FLV/fMP4-Remuxing und Live-Streaming. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT1078?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT1078) |
| **Domänenkomponenten** | | |
| [NewLife.Office](https://github.com/NewLifeX/NewLife.Office) | Ein Paket für Excel, Word, PPT, PDF und über 15 Dokumentformate. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Office?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Office) |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | Plattformübergreifendes Audio: Codecs, Geräte-I/O, DSP, Container und Streaming. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Audio?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Audio) |
| [NewLife.Video](https://github.com/NewLifeX/NewLife.Video) | Videocodecs, Containerformate, Frame-Verarbeitung und Streaming. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Video?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Video) |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | Einheitliche Schnittstelle für Baidu, AMap, Tencent und Tianditu mit Geokodierung und Koordinatentransformation. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Map?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Map) |
| [NewLife.IP](https://github.com/NewLifeX/NewLife.IP) | IPv4-Geolokalisierung mit offline speicherabgebildeter Datenbank und Online-API-Fallback. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IP?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IP) |
| **Werkzeuge & Produkte** | | |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | Entwicklerwerkzeug: Netzwerk, serielle Schnittstelle, Kryptografie, Regex, Modbus und MQTT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCoder?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCoder) |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | Selbst gehostetes Git-Quellcodeverwaltungssystem. | — |
| [LuckyClover](https://github.com/NewLifeX/LuckyClover) | Installiert bei Start aus einem älteren Host automatisch die passende .NET-Runtime. | — |

> Die Badges zeigen das Hauptpaket jedes Projekts; Theme-, Erweiterungs- und Satellitenpakete sind auf ihren NuGet-Seiten zu finden. Die über 30 hier nicht gelisteten Pakete (FTP, LoRa, Office-Erweiterungen, Protokolltreiber und mehr) finden Sie in der [NewLifeX-Organisation](https://github.com/NewLifeX) und auf [newlifex.com](https://newlifex.com).

## Team

<img src="https://newlifex.com/logo.png" width="48" align="right" alt="NewLife" />

Das NewLife-Team wurde 2002 gegründet und arbeitet seit mehr als zwei Jahrzehnten am .NET- und IoT-Ökosystem. Es betreut fast 100 Open-Source-Projekte (34 davon sind Kernprojekte dieser Landkarte) und liefert integrierte Software- und Hardwarelösungen für Energie, Logistik, Gesundheitswesen, Industrieautomation und Internetbranchen. Unser Leitspruch: „Lernen kennt keine Reihenfolge — wer es beherrscht, wird geachtet“.

Open-Source-Komponenten wurden <!--nuget-total:start-->6.48M<!--nuget-total:end--> Mal von NuGet heruntergeladen (ökosystemweit, Stand 2026-10).

Website: https://newlifex.com · Open-Source-Organisation: https://github.com/NewLifeX

<details>
<summary>WeChat-Kanal: 智能大石头 (zum Folgen scannen)</summary>

![智能大石头](https://newlifex.com/stone.jpg)

</details>
