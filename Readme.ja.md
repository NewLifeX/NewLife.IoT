# NewLife.IoT — IoT 標準ライブラリ &nbsp;🌐

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

🌐 **[English](Readme.MD) · [中文](Readme.zh-CN.md) · [हिन्दी](Readme.hi.md) · [Español](Readme.es.md) · [Français](Readme.fr.md) · [العربية](Readme.ar.md) · [বাংলা](Readme.bn.md) · [Português](Readme.pt.md) · [Русский](Readme.ru.md) · [日本語](Readme.ja.md)**

---

**NewLife.IoT** は IoT 標準ライブラリです — IoT 分野の統一通信プロトコル契約を定義します。**具体的なプロトコル実装は含みません。** IoT プラットフォームとハードウェアドライバ間の単一抽象レイヤーです。

| プラットフォーム | ドライバ | 主要設計 |
|:---|:---|:---|
| `IDriver`/`INode`/`IDevice` を利用 — 全HWに単一API | `DriverBase` を実装 | Async-first `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, カスタムプラットフォーム | NewLife.Modbus, Siemens, OPC-UA, BACnet | `ReadResult`（品質/タイムスタンプ付き）, `WriteResult` 構造体 |
| `ThingSpec` 物モデル — プロパティ/イベント/サービス | `ITransport` — シリアル/TCP/UDP/HTTP 統一 | ゼロアロケーション診断 |

| ソース | NuGet | ドキュメント |
|--------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [要件](Doc/需求文档.md) · [機能](Doc/功能清单.md) · [アーキテクチャ](Doc/架构设计.md) |

**ランタイム機能**：31 言語のローカライズ（NewLife.Core 語彙セットに準拠）· インスタンスプレフィックス付きログ · NativeAOT 実測通過（19 シナリオ、Windows/Linux/macOS）· 8 ターゲットフレームワーク（net45 → net10.0）。

---

## 新生命プロジェクトマトリックス / Project Matrix

対応フレームワーク: `net10.0` / `net8.0` / `net6.0` / `netstandard2.1` / `netstandard2.0` / `net462` / `net461` / `net45`

| プロジェクト | 年 | 説明 |
|:------------|:--:|:-----|
| **基礎コンポーネント** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | コアライブラリ — ログ、設定、キャッシュ、ネットワーク、シリアル化、APM |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | ビッグデータORM — 100億行/テーブル、MySql/SQLite/SqlServer/Oracle/PostgreSQL/DaMeng |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | ネットワークライブラリ — 2266万tps、400万同時TCP接続 |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | RPCフレームワーク — Http/RPC、高スループット、IoT対応 |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Cube高速開発プラットフォーム — RBAC、SSO、OAuth |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | 2008 | サービスマネージャ — Windowsサービス/Linux Systemd |
| [NewLife.Zero](https://github.com/NewLifeX/NewLife.Zero) | 2020 | Zeroスキャフォールド — Web/WebApi/Service テンプレート |
| **ミドルウェア** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redisクライアント — マイクロ秒レイテンシ、百万級スループット |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | RocketMQクライアント — Apache & Alibaba Cloud |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | MQTT — MqttClient / MqttServer |
| **NewLife.IoT** | 2022 | **IoT 標準ライブラリ — 統一ドライバインターフェースと物モデル仕様** |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Siemens PLC プロトコル |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 2022 | 地図コンポーネント — Baidu/Amap/Tencent/Tianditu |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 2023 | オーディオコーデック — PCM/ADPCMA/G711A/G722U/WAV/AAC |
| **製品** | | |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | 分散サービスプラットフォーム — ノード管理、APM、構成センター |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | 分散ビッグデータコンピューティングプラットフォーム |
| [NewLife.ERP](https://github.com/NewLifeX/NewLife.ERP) | 2021 | エンタープライズERP |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | 2006 | 開発者ツールセット |
| [EasyIO](https://github.com/NewLifeX/EasyIO) | 2023 | シンプルファイルストレージ |
| [XProxy](https://github.com/NewLifeX/XProxy) | 2005 | リバースプロキシ — NAT/Http |
| [HttpMeter](https://github.com/NewLifeX/HttpMeter) | 2022 | HTTPストレステスト |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | 2015 | Gitソース管理 |
| [SmartOS](https://github.com/NewLifeX/SmartOS) | 2014 | 組み込みOS — ARM Cortex-M |
| [SmartA2](https://github.com/NewLifeX/SmartA2) | 2019 | 組み込み産業用コンピュータ — IoTエッジゲートウェイ |
| FIoT IoT プラットフォーム | 2020 | IoT トータルソリューション — ノードあたり10万ポイント以上 |
| UWB 屋内測位 | 2020 | センチメートル級測位（10〜20cm） |

---

## 新生命開発チーム / Team

![XCode](https://newlifex.com/logo.png)

NewLifeチームは2002年設立、新時代のIoTソリューションプロバイダーです。  
80以上のオープンソースプロジェクト、NuGet 400万以上ダウンロード。

| ウェブサイト | オープンソース | コミュニティ |
|-------------|--------------|-------------|
| [newlifex.com](https://newlifex.com) | [github.com/newlifex](https://github.com/newlifex) | QQ: 1600800 / 1600838 |

![智能大石头](https://newlifex.com/stone.jpg)
