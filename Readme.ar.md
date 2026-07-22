# NewLife.IoT — مكتبة IoT القياسية &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** هي مكتبة IoT القياسية — تُعرّف عقود بروتوكول اتصال موحدة لمجال إنترنت الأشياء. **بدون تنفيذات بروتوكول ملموسة.**

| المنصة | التعريف | التصميم الرئيسي |
|:---|:---|:---|
| تستهلك `IDriver`/`INode`/`IDevice` — واجهة واحدة لجميع الأجهزة | تنفذ `DriverBase` | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, منصات مخصصة | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` مع الجودة/الطابع الزمني، `WriteResult` هيكل |
| `ThingSpec` — الخصائص/الأحداث/الخدمات | `ITransport` — تسلسلي/TCP/UDP/HTTP موحد | تشخيص بدون تخصيص، مفتاح إنتاج |

| المصدر | NuGet | المستندات |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [المتطلبات](Doc/需求文档.md) · [الميزات](Doc/功能清单.md) · [المعمارية](Doc/架构设计.md) |

---

## مصفوفة مشاريع NewLife / Project Matrix

الأطر: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| المشروع | السنة | الوصف |
|:--------|:-----:|:------|
| **الأساس** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | المكتبة الأساسية — التسجيل، الإعداد، التخزين المؤقت، الشبكة، التسلسل، APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | ORM البيانات الضخمة — 10 مليار+ صف/جدول، MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | مكتبة الشبكة — 22.66M tps، 4M اتصالات TCP متزامنة |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | إطار RPC — Http/RPC، إنتاجية عالية، متوافق مع IoT |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | منصة التطوير السريع Cube — RBAC، SSO، OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | مدير الخدمات — Windows Service / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero scaffold — قوالب Web/WebApi/Service |
| **الوسيط** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | عميل Redis — زمن انتقال بالميكروثانية، إنتاجية بمستوى المليون |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | عميل RocketMQ — Apache و Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **مكتبة IoT القياسية — واجهات تعريف موحدة ومواصفات نموذج الشيء** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | بروتوكول Siemens PLC |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | مكونات الخرائط — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | مرمز الصوت — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **المنتجات** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | منصة الخدمات الموزعة — إدارة العقد، APM، مركز التكوين |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | منصة الحوسبة الموزعة للبيانات الضخمة |
| FIoT منصة IoT | 2020 | حل IoT الشامل — 100 ألف+ نقطة لكل عقدة |
| UWB تحديد المواقع الداخلي | 2020 | تحديد مواقع بمستوى السنتيمتر (10~20سم) |

---

## فريق NewLife / Team

![XCode](https://newlifex.com/logo.png)

فريق NewLife، تأسس عام 2002، هو مزود حلول IoT للعصر الجديد.  
80+ مشروع مفتوح المصدر، 4 مليون+ تحميل NuGet.

| الموقع | المصدر المفتوح | المجتمع |
|--------|---------------|---------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
