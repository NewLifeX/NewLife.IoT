# NewLife.IoT — Стандартная Библиотека IoT &nbsp;🌐

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
🌐 [English](README.md) | [简体中文](README.zh-CN.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [Español](README.es.md) | [Français](README.fr.md) | [हिन्दी](README.hi.md) | [日本語](README.ja.md) | [Português](README.pt.md)
<!-- lang:end -->

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

**Возможности среды выполнения**: локализация на 31 языке (в соответствии с NewLife.Core) · журналирование с префиксом экземпляра · NativeAOT проверен (20 сценариев, Windows/Linux/macOS) · 8 целевых платформ (net45 → net10.0).

---

## Матрица Проектов NewLife / Project Matrix

Фреймворки: `net10.0` / `net8.0` / `net6.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net461` / `net45`

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
