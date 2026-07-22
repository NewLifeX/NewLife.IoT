# NewLife.IoT — Стандартная Библиотека IoT &nbsp;🌐

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/newlife.iot?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/newlife.iot?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/newlife.iot?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/newlife.iot?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/newlife.iot?label=dev%20nuget&logo=nuget)

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** — стандартная библиотека IoT, определяющая унифицированные контракты протоколов связи для сферы Интернета вещей. **Без конкретных реализаций протоколов.**

| Платформа | Драйвер | Ключевой дизайн |
|:---|:---|:---|
| Использует `IDriver`/`INode`/`IDevice` — единый API для всего | Реализует `DriverBase` | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, пользовательские платформы | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult` с Quality/Timestamp, `WriteResult` struct |
| `ThingSpec` — свойства/события/службы | `ITransport` — последовательный/TCP/UDP/HTTP | Диагностика без выделения памяти |

| Источник | NuGet | Документы |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [Требования](Doc/需求文档.md) · [Функции](Doc/功能清单.md) · [Архитектура](Doc/架构设计.md) |

---

## Матрица Проектов NewLife / Project Matrix

Фреймворки: `net10.0` / `net9.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net45`

| Проект | Год | Описание |
|:-------|:---:|:---------|
| **Основа** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Основная библиотека — логирование, конфигурация, кэш, сеть, сериализация, APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | ORM для больших данных — 10 млрд+ строк/таблица, MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Сетевая библиотека — 22,66M tps, 4M одновременных TCP-соединений |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | RPC фреймворк — Http/RPC, высокая пропускная способность, IoT-совместимость |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Платформа быстрой разработки Cube — RBAC, SSO, OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | Менеджер служб — Windows Service / Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zero каркас — шаблоны Web/WebApi/Service |
| **Промежуточное ПО** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Клиент Redis — микросекундная задержка, миллионная пропускная способность |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | Клиент RocketMQ — Apache и Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **Стандартная библиотека IoT — унифицированные интерфейсы драйверов и спецификация модели вещи** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Протокол Siemens PLC |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | Картографические компоненты — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | Аудиокодек — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **Продукты** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Платформа распределённых сервисов — управление узлами, APM, центр конфигурации |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | Платформа распределённых вычислений больших данных |
| FIoT IoT Платформа | 2020 | Комплексное IoT решение — 100K+ точек на узел |
| UWB Внутреннее Позиционирование | 2020 | Сантиметровое позиционирование (10~20см) |

---

## Команда NewLife / Team

![XCode](https://newlifex.com/logo.png)

Команда NewLife, основанная в 2002 году — поставщик IoT-решений новой эры.  
80+ open-source проектов, 4M+ загрузок NuGet.

| Сайт | Open Source | Сообщество |
|------|------------|------------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
