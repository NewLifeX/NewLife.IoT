# NewLife.IoT — IoT मानक पुस्तकालय &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** IoT मानक पुस्तकालय है — IoT क्षेत्र के लिए एकीकृत संचार प्रोटोकॉल अनुबंध परिभाषित करता है। **कोई ठोस प्रोटोकॉल कार्यान्वयन नहीं।**

| प्लेटफ़ॉर्म | ड्राइवर | मुख्य डिज़ाइन |
|:---|:---|:---|
| `IDriver`/`INode`/`IDevice` — सभी हार्डवेयर के लिए एक API | `DriverBase` लागू करें | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT | NewLife.Modbus, Siemens, OPC-UA | `ReadResult` (Quality/Timestamp), `WriteResult` struct |
| `ThingSpec` — गुण/घटनाएँ/सेवाएँ | `ITransport` — सीरियल/TCP/UDP/HTTP | शून्य-आवंटन डायग्नोस्टिक्स |

| स्रोत | NuGet | दस्तावेज़ |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [आवश्यकताएँ](Doc/需求文档.md) · [सुविधाएँ](Doc/功能清单.md) · [आर्किटेक्चर](Doc/架构设计.md) |

---

## न्यूलाइफ प्रोजेक्ट मैट्रिक्स / Project Matrix

फ्रेमवर्क: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| प्रोजेक्ट | वर्ष | विवरण |
|:----------|:----:|:------|
| **फाउंडेशन** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | कोर लाइब्रेरी — लॉगिंग, कॉन्फिग, कैश, नेटवर्किंग, सीरियलाइज़ेशन, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | बिग डेटा ORM — 10B+ पंक्तियाँ/टेबल, MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | नेटवर्क लाइब्रेरी — 22.66M tps, 4M समवर्ती TCP कनेक्शन |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | RPC फ्रेमवर्क — Http/RPC, उच्च थ्रूपुट, IoT-अनुकूल |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | क्यूब रैपिड डेव प्लेटफॉर्म — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | सर्विस मैनेजर — Windows सर्विस / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | ज़ीरो स्कैफोल्ड — Web/WebApi/Service टेम्पलेट |
| **मिडलवेयर** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redis क्लाइंट — माइक्रोसेकंड लेटेंसी, मिलियन-लेवल थ्रूपुट |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | RocketMQ क्लाइंट — Apache और Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **IoT मानक पुस्तकालय — एकीकृत ड्राइवर इंटरफेस और थिंग मॉडल स्पेक** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Siemens PLC प्रोटोकॉल |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | मैप घटक — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | ऑडियो कोडेक — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **उत्पाद** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | वितरित सेवा प्लेटफॉर्म — नोड प्रबंधन, APM, कॉन्फिग सेंटर |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | वितरित बिग डेटा कंप्यूटिंग प्लेटफॉर्म |
| FIoT IoT प्लेटफॉर्म | 2020 | IoT कुल समाधान — प्रति नोड 100K+ पॉइंट |
| UWB इनडोर पोज़िशनिंग | 2020 | सेंटीमीटर-स्तरीय पोज़िशनिंग (10~20cm) |

---

## न्यूलाइफ डेवलपमेंट टीम / Team

![XCode](https://newlifex.com/logo.png)

NewLife टीम, 2002 में स्थापित, नए युग के लिए IoT समाधान प्रदाता है।  
80+ ओपन-सोर्स प्रोजेक्ट, 4M+ NuGet डाउनलोड।

| वेबसाइट | ओपन सोर्स | समुदाय |
|---------|-----------|---------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
