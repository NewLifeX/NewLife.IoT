# NewLife.IoT — IoT স্ট্যান্ডার্ড লাইব্রেরি &nbsp;🌐

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
🌐 [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [العربية](README.ar.md) | [Español](README.es.md) | [Français](README.fr.md) | [हिन्दी](README.hi.md) | [日本語](README.ja.md) | [Português](README.pt.md) | [Русский](README.ru.md) | [Deutsch](README.de.md) | [한국어](README.ko.md)
<!-- lang:end -->

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

**রানটাইম সক্ষমতা**: 31টি ভাষায় স্থানীয়করণ (NewLife.Core শব্দভাণ্ডারের সাথে সমন্বিত) · ইনস্ট্যান্স-প্রিফিক্স লগিং · NativeAOT যাচাইকৃত (20টি দৃশ্যপট, Windows/Linux/macOS) · 8টি লক্ষ্য ফ্রেমওয়ার্ক (net45 → net10.0)।

---

## ইকোসিস্টেম ম্যাপ

NewLife ইকোসিস্টেম নির্ভরতার ভিত্তিতে স্তরবিন্যস্ত; প্রতিটি প্রজেক্টের স্বতন্ত্র সংস্করণ ও স্বাধীন রিলিজ। নিচের মানচিত্রটি সব NewLife রিপোজিটরিতে হুবহু একই; প্রতিটি প্রজেক্ট তার কোড রিপোজিটরি ও প্রধান NuGet প্যাকেজে সংযুক্ত।

| প্রজেক্ট | এক লাইনে পরিচিতি | ডাউনলোড |
| --- | --- | --- |
| **মূল ভিত্তি** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | লগিং, কনফিগারেশন, ক্যাশ, নেটওয়ার্ক, সিরিয়ালাইজেশন, সময়সূচি ও APM ট্রেসিং। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Core?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Core) |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | ডেটা মিডলওয়্যার: একক টেবিলে শতকোটি সারি, স্বয়ংক্রিয় শার্ডিং, পড়া-লেখার বিচ্ছেদ, 10টিরও বেশি ডেটাবেস সমর্থন। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCode?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCode) |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | উচ্চ-কর্মক্ষমতা নেটওয়ার্ক লাইব্রেরি: একক মেশিনে 1.77 মিলিয়ন pps, 14.5 Gbps (TCP 1KB, 2026-10 পরিমাপ), 4 মিলিয়ন Tcp দীর্ঘ সংযোগ। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Net?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Net) |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | CS অ্যাপ্লিকেশন ও IoT ডিভাইসের জন্য প্রোটোকল যোগাযোগ ও RPC ফ্রেমওয়ার্ক। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Remoting?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Remoting) |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | .NET অ্যাপ্লিকেশনকে অপারেটিং সিস্টেম ডেমন হিসেবে ইনস্টল করুন: Windows সার্ভিস ও Linux systemd। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Agent?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Agent) |
| [NewLife.Templates](https://github.com/NewLifeX/NewLife.Zero) | `dotnet new` প্রজেক্ট টেমপ্লেট: কনসোল, Web, WebApi, সার্ভিস, ডেটা ও IoT। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Templates?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Templates) |
| **ডেটা স্টোরেজ ও মেসেজিং** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | Redis ক্লায়েন্ট: মাইক্রোসেকেন্ড লেটেন্সি, মিলিয়ন-স্তরের থ্রুপুট, অন্তর্নির্মিত মেসেজ কিউ। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Redis?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Redis) |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | সম্পূর্ণ ম্যানেজড RocketMQ ক্লায়েন্ট, Apache RocketMQ ও Alibaba Cloud মেসেজ কিউ সমর্থন করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.RocketMQ?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.RocketMQ) |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | MQTT ক্লায়েন্ট ও সার্ভার, ক্লায়েন্ট Alibaba Cloud IoT সমর্থন করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MQTT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MQTT) |
| [NewLife.NovaDb](https://github.com/NewLifeX/NewLife.NovaDb) | C# হাইব্রিড ডেটাবেস: এমবেডেড/নেটওয়ার্ক দ্বৈত মোড, রিলেশনাল, টাইম-সিরিজ, মেসেজ কিউ ও KV একসাথে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.NovaDb?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.NovaDb) |
| [NewLife.MySql](https://github.com/NewLifeX/NewLife.MySql) | MySql ড্রাইভার ও ডেটা অ্যাক্সেস সহায়ক স্তর, NewLife ডেটা স্ট্যাকের উপর ভিত্তি করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MySql?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MySql) |
| **Web, AI ও ডিস্ট্রিবিউটেড** | | |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | কিউব দ্রুত উন্নয়ন প্ল্যাটফর্ম: ব্যবহারকারী অনুমতি, SSO, OAuth সার্ভার ও CRUD স্ক্যাফোল্ডিং। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Cube?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Cube) |
| [Stardust](https://github.com/NewLifeX/Stardust) | স্টারডাস্ট বিতরণকৃত সেবা প্ল্যাটফর্ম: নোড ম্যানেজমেন্ট, APM, কনফিগ সেন্টার, রেজিস্ট্রি সেন্টার ও রিলিজ সেন্টার। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Stardust?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Stardust) |
| [AntJob](https://github.com/NewLifeX/AntJob) | অ্যান্ট শিডিউলিং: বিতরণকৃত কাজ ও রিয়েল-টাইম ডেটা প্ল্যাটফর্ম, শার্ডিং, রিট্রাই ও ব্যাকফিল পুনর্গণনা সমর্থন করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AntJob?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AntJob) |
| [NewLife.AI](https://github.com/NewLifeX/NewLife.AI) | 46টি বড় মডেল সেবা প্রদানকারীকে একীভূতভাবে সংযুক্ত করে, সাথে রেডি-টু-ইউজ ChatAI Web চ্যাট অ্যাপ্লিকেশন। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AI?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AI) |
| **শিল্প ও IoT** | | |
| [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT) | IoT স্ট্যান্ডার্ড লাইব্রেরি, IoT যোগাযোগ প্রোটোকল চুক্তি ও ডিভাইস বিমূর্ততা সংজ্ঞায়িত করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IoT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IoT) |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | Modbus TCP/RTU/ASCII, NewLife.IoT-এর উপর ভিত্তি করে বাস্তবায়িত। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Modbus?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Modbus) |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | সিমেন্স PLC প্রোটোকল, NewLife.IoT-এর উপর ভিত্তি করে বাস্তবায়িত। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Siemens?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Siemens) |
| [NewLife.Omron](https://github.com/NewLifeX/NewLife.Omron) | ওমরন PLC প্রোটোকল, NewLife.IoT-এর উপর ভিত্তি করে বাস্তবায়িত। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Omron?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Omron) |
| [NewLife.Melsec](https://github.com/NewLifeX/NewLife.Melsec) | মিতসুবিশি Melsec PLC প্রোটোকল, NewLife.IoT-এর উপর ভিত্তি করে বাস্তবায়িত। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Melsec?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Melsec) |
| [NewLife.OPC](https://github.com/NewLifeX/NewLife.OPC) | OPC ক্লায়েন্ট, শিল্প ডেটা সংগ্রহে ব্যবহৃত। | — |
| [NewLife.BACnet](https://github.com/NewLifeX/NewLife.BACnet) | BACnet বিল্ডিং অটোমেশন প্রোটোকল স্ট্যাক। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.BACnet?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.BACnet) |
| [ZeroIoT](https://github.com/NewLifeX/ZeroIoT) | ওপেন সোর্স হালকা IoT প্ল্যাটফর্ম, গেটওয়ে ও ডিভাইস ম্যানেজমেন্ট সহ। | — |
| **যানবাহন নেটওয়ার্কিং** | | |
| [NewLife.JT808](https://github.com/NewLifeX/NewLife.JT808) | JT/T 808 যানবাহন অবস্থান প্রোটোকল: 70+ মেসেজ বডি, 2011/2013/2019 তিনটি সংস্করণ কভার করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT808?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT808) |
| [NewLife.JT809](https://github.com/NewLifeX/NewLife.JT809) | JT/T 809 মন্ত্রণালয় মান প্ল্যাটফর্ম আন্তঃসংযোগ প্রোটোকল, পরিবহন ব্যবস্থাপনা বিভাগের জন্য। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT809?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT809) |
| [NewLife.JT1078](https://github.com/NewLifeX/NewLife.JT1078) | JT/T 1078 যানবাহন ভিডিও: RTP পার্সিং, FLV/fMP4 রিমাক্স ও লাইভ পুশ। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT1078?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT1078) |
| **ডোমেইন কম্পোনেন্ট** | | |
| [NewLife.Office](https://github.com/NewLifeX/NewLife.Office) | একটি প্যাকেজে Excel, Word, PPT, PDF সহ 15+ ডকুমেন্ট ফরম্যাট। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Office?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Office) |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | ক্রস-প্ল্যাটফর্ম অডিও: কোডেক, ডিভাইস IO, DSP, কন্টেইনার ও স্ট্রিমিং। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Audio?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Audio) |
| [NewLife.Video](https://github.com/NewLifeX/NewLife.Video) | ভিডিও কোডেক, কন্টেইনার ফরম্যাট, ফ্রেম প্রসেসিং ও স্ট্রিমিং মিডিয়া। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Video?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Video) |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | বাইডু/আমাপ/টেনসেন্ট/তিয়ানদিতু একীভূত ইন্টারফেস, জিওকোডিং ও স্থানাঙ্ক রূপান্তর সমর্থন করে। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Map?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Map) |
| [NewLife.IP](https://github.com/NewLifeX/NewLife.IP) | IPv4 অবস্থান: অফলাইন ইন-মেমরি ম্যাপিং লাইব্রেরি + অনলাইন API দ্বৈত উৎস। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IP?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IP) |
| **টুল ও পণ্য** | | |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | কোডমাস্টার টুল: নেটওয়ার্ক, সিরিয়াল পোর্ট, এনক্রিপশন/ডিক্রিপশন, রেজেক্স, Modbus ও MQTT। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCoder?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCoder) |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | স্ব-হোস্টেড Git সোর্স কোড ম্যানেজমেন্ট সিস্টেম। | — |
| [LuckyClover](https://github.com/NewLifeX/LuckyClover) | পুরোনো হোস্ট থেকে চালু হলে স্বয়ংক্রিয়ভাবে উপযুক্ত .NET রানটাইম ইনস্টল করে। | — |

> ব্যাজগুলি প্রতিটি প্রজেক্টের প্রধান প্যাকেজ দেখায়; থিম প্যাকেজ, এক্সটেনশন প্যাকেজ ও স্যাটেলাইট প্যাকেজ সংশ্লিষ্ট NuGet পৃষ্ঠায় দেখুন। তালিকায় না থাকা 30টিরও বেশি প্যাকেজ (FTP, LoRa, Office এক্সটেনশন, বিভিন্ন প্রোটোকল ড্রাইভার ইত্যাদি) দেখুন [NewLifeX সংগঠন](https://github.com/NewLifeX) এবং [newlifex.com](https://newlifex.com)।

## নিউলাইফ ডেভেলপমেন্ট টিম

<img src="https://newlifex.com/logo.png" width="48" align="right" alt="NewLife" />

নিউলাইফ টিম 2002 সালে প্রতিষ্ঠিত; দুই দশকেরও বেশি সময় ধরে .NET ও IoT প্রযুক্তি ইকোসিস্টেমে নিবেদিত। টিম প্রায় একশত ওপেন-সোর্স প্রজেক্ট পরিচালনা করে (ইকোসিস্টেম ম্যাপে 34টি মূল প্রজেক্ট) এবং বিদ্যুৎ, লজিস্টিকস, চিকিৎসা, শিল্প নিয়ন্ত্রণ ও ইন্টারনেটসহ বিভিন্ন শিল্পে সফটওয়্যার-হার্ডওয়্যার সমন্বিত সমাধান প্রদান করে; আমাদের মূলমন্ত্র “শেখার কোনো ছোট-বড় নেই, যিনি জানেন তিনিই গুরু”।

ওপেন-সোর্স কম্পোনেন্টগুলি NuGet-এ মোট <!--nuget-total:start-->6.48M<!--nuget-total:end--> বার ডাউনলোড হয়েছে (সমগ্র ইকোসিস্টেম, 2026-10 পর্যন্ত)।

ওয়েবসাইট: https://newlifex.com · ওপেন-সোর্স সংগঠন: https://github.com/NewLifeX

<details>
<summary>WeChat অফিসিয়াল অ্যাকাউন্ট: 智能大石头 (স্ক্যান করে ফলো করুন)</summary>

![智能大石头](https://newlifex.com/stone.jpg)

</details>
