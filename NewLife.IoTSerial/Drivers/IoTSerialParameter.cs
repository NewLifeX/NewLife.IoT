﻿using System.ComponentModel;

namespace NewLife.IoTSerial.Drivers;

/// <summary>通用串口驱动参数</summary>
public class IoTSerialParameter : SerialParameter
{
    /// <summary>请求命令模板。支持十六进制格式(以0x开头)和字符串格式</summary>
    [Description("请求命令模板。支持十六进制格式(以0x开头)和字符串格式")]
    public String RequestCommand { get; set; } = "";

    /// <summary>响应数据编码格式。支持HEX、ASCII、UTF8等</summary>
    [Description("响应数据编码格式。支持HEX、ASCII、UTF8等")]
    public String ResponseEncoding { get; set; } = "HEX";
}
