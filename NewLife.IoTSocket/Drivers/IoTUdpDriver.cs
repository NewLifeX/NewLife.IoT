using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NewLife.Data;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.Net;
using NewLife.Serialization;

namespace NewLife.IoTSocket.Drivers;

/// <summary>IoT标准通用UDP网络驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用网络驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTUdp")]
[DisplayName("通用UDP网络驱动")]
public class IoTUdpDriver : IoTSocketDriver, IDiscoverableDriver
{
    #region 方法
    /// <summary>创建网络</summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected override ISocketClient CreateClient(SocketParameter parameter)
    {
        var uri = new NetUri(NetType.Udp, parameter.Server, parameter.Port);
        var client = uri.CreateRemote();

        client.Timeout = parameter.Timeout;

        return client;
    }

    /// <summary>异步扫描和发现网络上的兼容设备</summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task<IEnumerable<IDeviceInfo>> DiscoverAsync(Dictionary<String, Object> parameters, CancellationToken cancellationToken = default)
    {
        if (parameters == null || parameters.Count == 0)
            throw new ArgumentNullException(nameof(parameters), "参数不能为空");

        if (!parameters.TryGetValue("Port", out var portObj) || portObj is not Int32 port)
            throw new ArgumentException("参数中必须包含端口号", nameof(parameters));

        var body = "hello";
        if (parameters.TryGetValue("body", out var obj)) body = obj + "";

        var timeout = 3000;
        if (parameters.TryGetValue("Timeout", out var timeoutObj) && timeoutObj is Int32 t) timeout = t;

        var devices = new List<IDeviceInfo>();

        // 获取本机所有网络接口
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .ToArray();

        var tasks = new List<Task>();

        foreach (var ni in networkInterfaces)
        {
            var ipProperties = ni.GetIPProperties();
            var unicastAddresses = ipProperties.UnicastAddresses
                .Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();

            foreach (var ua in unicastAddresses)
            {
                var task = DiscoverOnSubnet(ua.Address, ua.IPv4Mask, port, body, timeout, devices, cancellationToken);
                tasks.Add(task);
            }
        }

        // 等待所有子网发现完成
        await Task.WhenAll(tasks).ConfigureAwait(false);

        return devices;
    }

    private async Task DiscoverOnSubnet(IPAddress localIP, IPAddress subnetMask, Int32 port, String body, Int32 timeout, List<IDeviceInfo> devices, CancellationToken cancellationToken)
    {
        try
        {
            // 计算广播地址
            var localBytes = localIP.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();
            var broadcastBytes = new Byte[4];
            for (var i = 0; i < 4; i++)
            {
                broadcastBytes[i] = (Byte)(localBytes[i] | (~maskBytes[i]));
            }
            var broadcastAddress = new IPAddress(broadcastBytes);

            using var udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(localIP, 0));
            udpClient.EnableBroadcast = true;
            udpClient.Client.ReceiveTimeout = timeout;

            // 发送广播消息
            var data = body.GetBytes();
            var broadcastEndPoint = new IPEndPoint(broadcastAddress, port);
            await udpClient.SendAsync(data, data.Length, broadcastEndPoint).ConfigureAwait(false);

            WriteLog("UDP广播发送到 {0}:{1}, 内容: {2}", broadcastAddress, port, body);

            // 监听响应
            var endTime = DateTime.Now.AddMilliseconds(timeout);
            while (DateTime.Now < endTime && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var remainingTime = (Int32)(endTime - DateTime.Now).TotalMilliseconds;
                    if (remainingTime <= 0) break;

                    udpClient.Client.ReceiveTimeout = Math.Min(remainingTime, 1000);
                    var result = await udpClient.ReceiveAsync().ConfigureAwait(false);

                    var responseIP = result.RemoteEndPoint.Address.ToString();
                    var responsePort = result.RemoteEndPoint.Port;
                    var responseData = result.Buffer;

                    WriteLog("收到来自 {0}:{1} 的响应, 长度: {2}", responseIP, responsePort, responseData.Length);

                    // 构造设备信息
                    var deviceInfo = new DeviceInfo
                    {
                        Code = $"UDP_{responseIP}_{responsePort}",
                        Name = $"UDP设备 {responseIP}:{responsePort}",
                        ProductCode = "IoTUdp",
                        Protocol = "IoTUdp"
                    };

                    // 构造SocketParameter作为设备参数
                    var socketParameter = new SocketParameter
                    {
                        Server = responseIP,
                        Port = responsePort,
                        Timeout = 3000,
                        RequestCommand = body,
                        ResponseEncoding = "UTF8"
                    };

                    // 将参数序列化为JSON字符串
                    deviceInfo.Parameter = socketParameter.ToJson();

                    lock (devices)
                    {
                        // 避免重复添加相同的设备
                        if (!devices.Any(d => d.Code == deviceInfo.Code))
                        {
                            devices.Add(deviceInfo);
                        }
                    }
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    // 超时是正常的，继续等待
                    continue;
                }
                catch (Exception ex)
                {
                    WriteLog("接收UDP响应时出错: {0}", ex.Message);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            WriteLog("在子网 {0} 上进行UDP发现时出错: {1}", localIP, ex.Message);
        }
    }
    #endregion
}