# NewLife.IoT — IoT 표준 라이브러리 &nbsp;🌐

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
🌐 [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [العربية](README.ar.md) | [বাংলা](README.bn.md) | [Español](README.es.md) | [Français](README.fr.md) | [हिन्दी](README.hi.md) | [日本語](README.ja.md) | [Português](README.pt.md) | [Русский](README.ru.md) | [Deutsch](README.de.md)
<!-- lang:end -->

---

**NewLife.IoT**는 IoT 표준 라이브러리로, IoT 분야의 통신 프로토콜 **통합 계약**을 정의하며 구체적인 프로토콜 구현은 포함하지 않습니다. IoT 플랫폼과 하드웨어 드라이버 사이의 **단일 추상화 계층**을 목표로 합니다.

| 플랫폼 측 | 드라이버 측 | 핵심 설계 |
|:---|:---|:---|
| `IDriver`/`INode`/`IDevice` 사용 — 하나의 API로 모든 하드웨어 제어 | `DriverBase` 상속 — 플랫폼 생태계에 연결 | 비동기 우선 `Task<T>` + `CancellationToken` |
| Stardust, ZeroIoT, FIoT, 사용자 정의 플랫폼 | NewLife.Modbus, Siemens, OPC-UA, BACnet | 품질 코드/타임스탬프를 담은 `ReadResult`, 할당 없는 `WriteResult` 구조체 |
| `ThingSpec` 사물 모델 — 속성/이벤트/서비스 | `ITransport` — 시리얼/TCP/UDP/HTTP 통합 | 진단 프레임 스위치, 운영 환경 무오버헤드 |

| 소스 | NuGet | 문서 |
|------|-------|------|
| [GitHub](https://github.com/NewLifeX/NewLife.IoT) | [NewLife.IoT](https://www.nuget.org/packages/NewLife.IoT) | [요구사항](Doc/需求文档.md) · [기능 목록](Doc/功能清单.md) · [아키텍처](Doc/架构设计.md) |

**런타임 기능**: 31개 언어 지역화(NewLife.Core 사전 집합과 정렬) · 인스턴스 접두사 로깅 · NativeAOT 검증 완료(20개 시나리오, Windows/Linux/macOS) · 8개 대상 프레임워크(net45 → net10.0).

---

## 생태계 지도

NewLife는 독립적으로 버전이 관리되는 오픈 소스 프로젝트의 계층형 생태계입니다. 아래 지도는 모든 NewLife 저장소에서 동일하게 유지되며, 각 프로젝트는 해당 저장소와 대표 NuGet 패키지로 연결됩니다.

| 프로젝트 | 한 줄 소개 | 다운로드 |
| --- | --- | --- |
| **기반** | | |
| [NewLife.Core](https://github.com/NewLifeX/X) | 로깅, 구성, 캐싱, 네트워킹, 직렬화, 스케줄링 및 APM 추적. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Core?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Core) |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 데이터 미들웨어: 단일 테이블 수백억 건, 자동 샤딩, 읽기/쓰기 분리, 10개 이상 데이터베이스 지원. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCode?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCode) |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 고성능 네트워크 라이브러리: 단일 머신 177만 pps / 14.5 Gbps(TCP 1KB, 2026-10 실측), 400만 TCP 장기 연결. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Net?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Net) |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 클라이언트/서버 애플리케이션과 IoT 기기를 위한 프로토콜 통신 및 RPC 프레임워크. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Remoting?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Remoting) |
| [NewLife.Agent](https://github.com/NewLifeX/NewLife.Agent) | .NET 애플리케이션을 운영 체제 데몬으로 설치: Windows 서비스와 Linux systemd. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Agent?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Agent) |
| [NewLife.Templates](https://github.com/NewLifeX/NewLife.Zero) | 콘솔, 웹, WebApi, 서비스, 데이터, IoT용 `dotnet new` 프로젝트 템플릿. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Templates?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Templates) |
| **데이터 및 메시징** | | |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 마이크로초 지연, 초당 수백만 작업, 내장 메시지 큐를 갖춘 Redis 클라이언트. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Redis?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Redis) |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | Apache RocketMQ와 알리바바 클라우드 메시지 큐를 지원하는 순수 관리형 RocketMQ 클라이언트. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.RocketMQ?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.RocketMQ) |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | MQTT 클라이언트와 서버, 알리바바 클라우드 IoT 연결 지원. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MQTT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MQTT) |
| [NewLife.NovaDb](https://github.com/NewLifeX/NewLife.NovaDb) | C# 하이브리드 데이터베이스: 임베디드/네트워크 이중 모드, 관계형, 시계열, 메시지 큐, KV 통합. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.NovaDb?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.NovaDb) |
| [NewLife.MySql](https://github.com/NewLifeX/NewLife.MySql) | NewLife 데이터 스택 기반 MySql 드라이버와 데이터 접근 보조 계층. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.MySql?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.MySql) |
| **웹, AI 및 분산** | | |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 빠른 개발 어드민 플랫폼: 권한, SSO, OAuth 서버, CRUD 스캐폴딩. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Cube?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Cube) |
| [Stardust](https://github.com/NewLifeX/Stardust) | 분산 서비스 플랫폼: 노드 관리, APM, 구성 센터, 레지스트리, 릴리스 센터. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Stardust?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Stardust) |
| [AntJob](https://github.com/NewLifeX/AntJob) | 샤딩, 재시도, 백필을 지원하는 분산 작업 및 실시간 데이터 플랫폼. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AntJob?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AntJob) |
| [NewLife.AI](https://github.com/NewLifeX/NewLife.AI) | 46개 LLM 공급자를 위한 통합 클라이언트와 바로 사용할 수 있는 ChatAI 웹 채팅 애플리케이션. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.AI?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.AI) |
| **산업 및 IoT** | | |
| [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT) | 프로토콜 계약과 장치 추상화를 정의하는 IoT 표준 라이브러리. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IoT?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IoT) |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | NewLife.IoT 기반 Modbus TCP/RTU/ASCII 구현. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Modbus?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Modbus) |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | NewLife.IoT 기반 지멘스 PLC 프로토콜. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Siemens?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Siemens) |
| [NewLife.Omron](https://github.com/NewLifeX/NewLife.Omron) | NewLife.IoT 기반 오므론 PLC 프로토콜. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Omron?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Omron) |
| [NewLife.Melsec](https://github.com/NewLifeX/NewLife.Melsec) | NewLife.IoT 기반 미쓰비시 Melsec PLC 프로토콜. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Melsec?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Melsec) |
| [NewLife.OPC](https://github.com/NewLifeX/NewLife.OPC) | 산업 데이터 수집용 OPC 클라이언트. | — |
| [NewLife.BACnet](https://github.com/NewLifeX/NewLife.BACnet) | BACnet 빌딩 자동화 프로토콜 스택. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.BACnet?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.BACnet) |
| [ZeroIoT](https://github.com/NewLifeX/ZeroIoT) | 게이트웨이와 장치 관리를 포함한 경량 오픈 소스 IoT 플랫폼. | — |
| **차량 관제** | | |
| [NewLife.JT808](https://github.com/NewLifeX/NewLife.JT808) | JT/T 808 차량 위치 프로토콜: 70개 이상 메시지 본문, 2011/2013/2019 버전. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT808?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT808) |
| [NewLife.JT809](https://github.com/NewLifeX/NewLife.JT809) | 운송 관리 부서를 위한 JT/T 809 플랫폼 간 연동 프로토콜. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT809?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT809) |
| [NewLife.JT1078](https://github.com/NewLifeX/NewLife.JT1078) | JT/T 1078 차량 영상: RTP 파싱, FLV/fMP4 리먹싱, 라이브 스트리밍. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.JT1078?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.JT1078) |
| **도메인 구성 요소** | | |
| [NewLife.Office](https://github.com/NewLifeX/NewLife.Office) | Excel, Word, PPT, PDF 등 15개 이상 문서 형식을 하나의 패키지로. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Office?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Office) |
| [NewLife.Audio](https://github.com/NewLifeX/NewLife.Audio) | 크로스 플랫폼 오디오: 코덱, 장치 I/O, DSP, 컨테이너, 스트리밍. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Audio?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Audio) |
| [NewLife.Video](https://github.com/NewLifeX/NewLife.Video) | 비디오 코덱, 컨테이너 형식, 프레임 처리, 스트리밍. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Video?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Video) |
| [NewLife.Map](https://github.com/NewLifeX/NewLife.Map) | 바이두/가오더/텐센트/톈디투 통합 인터페이스, 지오코딩과 좌표 변환 지원. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.Map?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.Map) |
| [NewLife.IP](https://github.com/NewLifeX/NewLife.IP) | IPv4 위치 조회: 오프라인 메모리 매핑 라이브러리와 온라인 API 이중 소스. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.IP?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.IP) |
| **도구 및 제품** | | |
| [CrazyCoder](https://github.com/NewLifeX/XCoder) | 개발자 도구: 네트워크, 시리얼 포트, 암호화, 정규식, Modbus, MQTT. | [![NuGet](https://img.shields.io/nuget/dt/NewLife.XCoder?style=flat-square&logo=nuget&label=downloads)](https://www.nuget.org/packages/NewLife.XCoder) |
| [GitCandy](https://github.com/NewLifeX/GitCandy) | 자체 호스팅 Git 소스 코드 관리 시스템. | — |
| [LuckyClover](https://github.com/NewLifeX/LuckyClover) | 하위 버전 호스트에서 시작할 때 일치하는 .NET 런타임을 자동으로 설치. | — |

> 배지는 각 프로젝트의 대표 패키지를 보여 주며, 테마·확장·위성 패키지는 해당 NuGet 페이지에서 확인할 수 있습니다. 여기에 나열되지 않은 30여 개 패키지(FTP, LoRa, Office 확장, 각종 프로토콜 드라이버 등)는 [NewLifeX 조직](https://github.com/NewLifeX)과 [newlifex.com](https://newlifex.com)에서 확인하세요.

## 팀

<img src="https://newlifex.com/logo.png" width="48" align="right" alt="NewLife" />

NewLife 팀은 2002년에 설립되어 20년 넘게 .NET과 IoT 기술 생태계에 집중해 왔으며, 약 100개의 오픈 소스 프로젝트(이 지도에 포함된 34개 핵심 프로젝트 포함)를 주도하고 전력, 물류, 의료, 산업 제어, 인터넷 산업에 소프트웨어와 하드웨어 통합 솔루션을 제공합니다. “배움에는 선후가 없고, 이룬 사람이 스승이다”를 신조로 삼고 있습니다.

오픈 소스 구성 요소는 NuGet에서 누적 <!--nuget-total:start-->6.48M<!--nuget-total:end--> 회 다운로드되었습니다(전체 생태계 기준, 2026-10 현재).

웹사이트: https://newlifex.com · 오픈 소스 조직: https://github.com/NewLifeX

<details>
<summary>위챗 공식 계정: 智能大石头 (스캔하여 팔로우)</summary>

![智能大石头](https://newlifex.com/stone.jpg)

</details>
