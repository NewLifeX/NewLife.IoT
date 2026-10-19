# NewLife.IoT — IoT मानक पुस्तकालय &nbsp;🌐

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
🌐 [English](README.md) | [简体中文](README.zh-CN.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [Español](README.es.md) | [Français](README.fr.md) | [日本語](README.ja.md) | [Português](README.pt.md) | [Русский](README.ru.md)
<!-- lang:end -->

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

**रनटाइम क्षमताएँ**: 31 भाषाओं में स्थानीयकरण (NewLife.Core शब्दावली से संरेखित) · इंस्टेंस-प्रीफ़िक्स लॉगिंग · NativeAOT परीक्षण पारित (20 परिदृश्य, Windows/Linux/macOS) · 8 लक्ष्य फ्रेमवर्क (net45 → net10.0)।

---

## पारिस्थितिकी तंत्र मानचित्र

NewLife पारिस्थितिकी तंत्र निर्भरता के आधार पर स्तरित है; प्रत्येक प्रोजेक्ट का संस्करण और रिलीज़ स्वतंत्र है। नीचे दिया गया मानचित्र सभी NewLife रिपॉज़िटरी में शब्दशः एक समान है; प्रत्येक प्रोजेक्ट अपने कोड रिपॉज़िटरी और मुख्य NuGet पैकेज से जुड़ा है।

| प्रोजेक्ट | एक पंक्ति में परिचय | डाउनलोड |
| --- | --- | --- |
| **मूल आधार** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | लॉगिंग, कॉन्फ़िगरेशन, कैश, नेटवर्किंग, सीरियलाइज़ेशन, शेड्यूलिंग और APM ट्रेसिंग। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Core?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Core) |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | डेटा मिडलवेयर: एकल तालिका में अरबों पंक्तियाँ, स्वचालित शार्डिंग, पढ़ने-लिखने का पृथक्करण, 10 से अधिक डेटाबेस का समर्थन। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCode?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCode) |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | उच्च-प्रदर्शन नेटवर्क लाइब्रेरी: एकल मशीन पर 1.77 मिलियन pps, 14.5 Gbps (TCP 1KB, 2026-10 मापा गया), 4 मिलियन Tcp दीर्घकालिक कनेक्शन। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Net?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Net) |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | CS अनुप्रयोगों और IoT उपकरणों के लिए प्रोटोकॉल संचार और RPC फ्रेमवर्क। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Remoting?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Remoting) |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | .NET अनुप्रयोगों को ऑपरेटिंग सिस्टम डेमॉन के रूप में इंस्टॉल करें: Windows सेवा और Linux systemd। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Agent?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Agent) |
| [NewLife.Templates](https://github.com/NewLifeX/NewLife.Zero) | `dotnet new` प्रोजेक्ट टेम्पलेट: कंसोल, Web, WebApi, सेवा, डेटा और IoT। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Templates?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Templates) |
| **डेटा संग्रहण और संदेश** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | Redis क्लाइंट: माइक्रोसेकंड-स्तरीय विलंब, लाखों-स्तरीय थ्रूपुट, अंतर्निहित संदेश कतार। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Redis?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Redis) |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | पूर्णतः प्रबंधित RocketMQ क्लाइंट, Apache RocketMQ और Alibaba Cloud संदेश कतार का समर्थन करता है। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.RocketMQ?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.RocketMQ) |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | MQTT क्लाइंट और सर्वर, क्लाइंट Alibaba Cloud IoT का समर्थन करता है। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MQTT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MQTT) |
| [NewLife.NovaDb](https://github.com/NewLifeX/NewLife.NovaDb) | C# हाइब्रिड डेटाबेस: एम्बेडेड/नेटवर्क दोहरी मोड, रिलेशनल, टाइम-सीरीज़, संदेश कतार और KV एक साथ। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.NovaDb?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.NovaDb) |
| [NewLife.MySql](https://github.com/NewLifeX/NewLife.MySql) | MySql ड्राइवर और डेटा एक्सेस सहायक परत, NewLife डेटा स्टैक पर आधारित। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MySql?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MySql) |
| **Web, AI और वितरित** | | |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | क्यूब त्वरित विकास प्लेटफ़ॉर्म: उपयोगकर्ता अनुमतियाँ, SSO, OAuth सर्वर और CRUD स्कैफ़ोल्डिंग। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Cube?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Cube) |
| [Stardust](https://github.com/NewLifeX/Stardust) | स्टारडस्ट वितरित सेवा प्लेटफ़ॉर्म: नोड प्रबंधन, APM, कॉन्फ़िग केंद्र, रजिस्ट्री केंद्र और रिलीज़ केंद्र। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Stardust?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Stardust) |
| [AntJob](https://github.com/NewLifeX/AntJob) | एंट शेड्यूलिंग: वितरित कार्य और रीयल-टाइम डेटा प्लेटफ़ॉर्म, शार्डिंग, पुनःप्रयास और बैकफ़िल पुनर्गणना का समर्थन करता है। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AntJob?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AntJob) |
| [NewLife.AI](https://github.com/NewLifeX/NewLife.AI) | 46 बड़े मॉडल सेवा प्रदाताओं का एकीकृत कनेक्शन, साथ में तैयार ChatAI Web चैट अनुप्रयोग। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AI?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AI) |
| **औद्योगिक और IoT** | | |
| [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT) | IoT मानक पुस्तकालय, IoT संचार प्रोटोकॉल अनुबंध और डिवाइस अमूर्तन परिभाषित करता है। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IoT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IoT) |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | Modbus TCP/RTU/ASCII, NewLife.IoT पर आधारित। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Modbus?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Modbus) |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | सीमेंस PLC प्रोटोकॉल, NewLife.IoT पर आधारित। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Siemens?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Siemens) |
| [NewLife.Omron](https://github.com/NewLifeX/NewLife.Omron) | ओमरॉन PLC प्रोटोकॉल, NewLife.IoT पर आधारित। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Omron?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Omron) |
| [NewLife.Melsec](https://github.com/NewLifeX/NewLife.Melsec) | मित्सुबिशी Melsec PLC प्रोटोकॉल, NewLife.IoT पर आधारित। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Melsec?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Melsec) |
| [NewLife.OPC](https://github.com/NewLifeX/NewLife.OPC) | OPC क्लाइंट, औद्योगिक डेटा संग्रह के लिए। | — |
| [NewLife.BACnet](https://github.com/NewLifeX/NewLife.BACnet) | BACnet बिल्डिंग ऑटोमेशन प्रोटोकॉल स्टैक। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.BACnet?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.BACnet) |
| [ZeroIoT](https://github.com/NewLifeX/ZeroIoT) | ओपन-सोर्स हल्का IoT प्लेटफ़ॉर्म, गेटवे और डिवाइस प्रबंधन सहित। | — |
| **वाहन नेटवर्किंग** | | |
| [NewLife.JT808](https://github.com/NewLifeX/NewLife.JT808) | JT/T 808 वाहन स्थिति निर्धारण प्रोटोकॉल: 70+ संदेश बॉडी, 2011/2013/2019 तीन संस्करणों को कवर करता है। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT808?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT808) |
| [NewLife.JT809](https://github.com/NewLifeX/NewLife.JT809) | JT/T 809 मानक प्लेटफ़ॉर्म इंटरकनेक्शन प्रोटोकॉल, परिवहन प्रबंधन विभागों के लिए। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT809?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT809) |
| [NewLife.JT1078](https://github.com/NewLifeX/NewLife.JT1078) | JT/T 1078 वाहन वीडियो: RTP पार्सिंग, FLV/fMP4 रीमक्स और लाइव पुश। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT1078?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT1078) |
| **डोमेन घटक** | | |
| [NewLife.Office](https://github.com/NewLifeX/NewLife.Office) | एक पैकेज में Excel, Word, PPT, PDF सहित 15+ दस्तावेज़ प्रारूप। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Office?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Office) |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | क्रॉस-प्लेटफ़ॉर्म ऑडियो: कोडेक, डिवाइस IO, DSP, कंटेनर और स्ट्रीमिंग। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Audio?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Audio) |
| [NewLife.Video](https://github.com/NewLifeX/NewLife.Video) | वीडियो कोडेक, कंटेनर प्रारूप, फ़्रेम प्रोसेसिंग और स्ट्रीमिंग मीडिया। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Video?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Video) |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | बाइडू/एमैप/टेनसेंट/तियानदितु एकीकृत इंटरफ़ेस, जियोकोडिंग और निर्देशांक रूपांतरण का समर्थन करता है। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Map?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Map) |
| [NewLife.IP](https://github.com/NewLifeX/NewLife.IP) | IPv4 स्थान: ऑफ़लाइन इन-मेमोरी मैपिंग लाइब्रेरी + ऑनलाइन API दोहरा स्रोत। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IP?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IP) |
| **उपकरण और उत्पाद** | | |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | कोडमास्टर टूल: नेटवर्क, सीरियल पोर्ट, एन्क्रिप्शन/डिक्रिप्शन, रेगुलर एक्सप्रेशन, Modbus और MQTT। | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCoder?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCoder) |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | स्व-होस्टेड Git स्रोत कोड प्रबंधन प्रणाली। | — |
| [LuckyClover](https://github.com/NewLifeX/LuckyClover) | पुराने होस्ट से प्रारंभ होने पर स्वचालित रूप से उपयुक्त .NET रनटाइम इंस्टॉल करता है। | — |

> बैज प्रत्येक प्रोजेक्ट के मुख्य पैकेज दर्शाते हैं; थीम पैकेज, एक्सटेंशन पैकेज और सैटेलाइट पैकेज संबंधित NuGet पृष्ठ पर देखें।सूची में शामिल नहीं 30 से अधिक पैकेज (FTP, LoRa, Office एक्सटेंशन, विभिन्न प्रोटोकॉल ड्राइवर आदि) [NewLifeX संगठन](https://github.com/NewLifeX) और [newlifex.com](https://newlifex.com) पर देखें।

## न्यूलाइफ डेवलपमेंट टीम

<img src="https://newlifex.com/logo.png" width="48" align="right" alt="NewLife" />

न्यूलाइफ टीम की स्थापना 2002 में हुई; टीम दो दशकों से अधिक समय से .NET और IoT प्रौद्योगिकी पारिस्थितिकी तंत्र पर केंद्रित है, लगभग सौ ओपन-सोर्स प्रोजेक्ट्स का संचालन करती है (पारिस्थितिकी मानचित्र में 34 मुख्य प्रोजेक्ट) और बिजली, लॉजिस्टिक्स, चिकित्सा, औद्योगिक नियंत्रण तथा इंटरनेट जैसे क्षेत्रों के लिए सॉफ़्टवेयर-हार्डवेयर एकीकृत समाधान प्रदान करती है; हमारा ध्येय वाक्य है “सीखने में छोटा-बड़ा नहीं होता, जो जानता है वही गुरु है”।

ओपन-सोर्स घटकों को NuGet पर कुल <!--nuget-total:start-->6.48M<!--nuget-total:end--> बार डाउनलोड किया गया (संपूर्ण पारिस्थितिकी तंत्र, 2026-10 तक)।

वेबसाइट: https://newlifex.com · ओपन-सोर्स संगठन: https://github.com/NewLifeX

<details>
<summary>WeChat आधिकारिक खाता: 智能大石头 (स्कैन करके फ़ॉलो करें)</summary>

![智能大石头](https://newlifex.com/stone.jpg)

</details>
