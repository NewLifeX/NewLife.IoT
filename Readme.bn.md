# NewLife.IoT — IoT স্ট্যান্ডার্ড লাইব্রেরি &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** হল IoT স্ট্যান্ডার্ড লাইব্রেরি — IoT ক্ষেত্রের জন্য একীভূত যোগাযোগ প্রোটোকল চুক্তি সংজ্ঞায়িত করে। **কোনো কংক্রিট প্রোটোকল বাস্তবায়ন নেই।**

| প্ল্যাটফর্ম | ড্রাইভার | মূল ডিজাইন |
|:---|:---|:---|
| `IDriver`/`INode`/`IDevice` — সব হার্ডওয়্যারের জন্য এক API | `DriverBase` বাস্তবায়ন | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT | NewLife.Modbus, Siemens, OPC-UA | `ReadResult` (Quality/Timestamp), `WriteResult` struct |
| `ThingSpec` — বৈশিষ্ট্য/ইভেন্ট/পরিষেবা | `ITransport` — সিরিয়াল/TCP/UDP/HTTP | শূন্য-বরাদ্দ ডায়াগনস্টিক্স |

| উৎস | NuGet | নথি |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [প্রয়োজনীয়তা](Doc/需求文档.md) · [বৈশিষ্ট্য](Doc/功能清单.md) · [স্থাপত্য](Doc/架构设计.md) |

---

## নিউলাইফ প্রজেক্ট ম্যাট্রিক্স / Project Matrix

ফ্রেমওয়ার্ক: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| প্রজেক্ট | বছর | বিবরণ |
|:---------|:---:|:------|
| **ফাউন্ডেশন** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | কোর — লগিং, কনফিগ, ক্যাশ, নেটওয়ার্ক, সিরিয়ালাইজেশন, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | বিগ ডেটা ORM — MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | নেটওয়ার্ক — 22.66M tps, 4M TCP সংযোগ |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | RPC ফ্রেমওয়ার্ক — Http/RPC, IoT-বান্ধব |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | কিউব ডেভ প্ল্যাটফর্ম — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | সার্ভিস ম্যানেজার — Windows/Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | জিরো স্ক্যাফোল্ড — Web/WebApi/Service টেমপ্লেট |
| **মিডলওয়্যার** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redis ক্লায়েন্ট — মাইক্রোসেকেন্ড লেটেন্সি |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | RocketMQ — Apache ও Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient/MqttServer |
| **NewLife.IoT** | 2022 | **IoT স্ট্যান্ডার্ড — ড্রাইভার ও থিং মডেল** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Siemens PLC প্রোটোকল |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | ম্যাপ — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | অডিও কোডেক — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **প্রোডাক্ট** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | ডিস্ট্রিবিউটেড প্ল্যাটফর্ম — নোড, APM, কনফিগ |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | বিগ ডেটা কম্পিউটিং প্ল্যাটফর্ম |
| FIoT IoT প্ল্যাটফর্ম | 2020 | IoT সমাধান — 100K+ পয়েন্ট/নোড |
| UWB ইনডোর পজিশনিং | 2020 | সেন্টিমিটার-লেভেল (10~20cm) |

| ওয়েবসাইট | ওপেন সোর্স | কমিউনিটি |
|-----------|-----------|-----------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

---

## নিউলাইফ ডেভেলপমেন্ট টিম / Team

![XCode](https://newlifex.com/logo.png)

নিউলাইফ টিম, ২০০২ সালে প্রতিষ্ঠিত, নতুন যুগের জন্য IoT সমাধান প্রদানকারী।  
৮০+ ওপেন-সোর্স প্রজেক্ট, ৪M+ NuGet ডাউনলোড।

![智能大石头](https://newlifex.com/stone.jpg)
