using System.Net.Sockets;
using NewLife.Log;
using NewLife.Net;

namespace NewLife.IoT.Protocols
{
    /// <summary>网络透传</summary>
    public class NetPort : IDataPort
    {
        #region 属性
        /// <summary>服务端地址</summary>
        public String Server { get; set; }

        /// <summary>性能追踪器</summary>
        public ITracer Tracer { get; set; }

        private TcpClient _client;
        private NetworkStream _stream;
        #endregion

        #region 方法
        /// <summary>初始化。传入配置字符串</summary>
        /// <param name="config"></param>
        public virtual void Init(String config)
        {
            var ss = config.SplitAsDictionary("=", ";", true);
            if (ss.TryGetValue("Server", out var str))
                Server = str;
            else if (ss.TryGetValue("Address", out str))
                Server = str;
        }

        /// <summary>打开</summary>
        public virtual void Open()
        {
            if (_client == null || !_client.Connected)
            {
                var uri = new NetUri(Server);

                var client = new TcpClient
                {
                    SendTimeout = 3_000,
                    ReceiveTimeout = 3_000
                };
                client.Connect(uri.Host, uri.Port);

                _client = client;
                _stream = client.GetStream();

                WriteLog("Open {0}:{1}", Server, uri.Port);
            }
        }

        /// <summary>读取</summary>
        /// <returns></returns>
        public Byte[] Read()
        {
            using var span = Tracer?.NewSpan("netport:Read");

            Open();

            var buf = new Byte[1024];
            var count = _stream.Read(buf, 0, buf.Length);

            return buf.ReadBytes(0, count);
        }

        /// <summary>写入</summary>
        /// <param name="data"></param>
        public void Write(Byte[] data)
        {
            if (data == null || data.Length == 0) return;

            using var span = Tracer?.NewSpan("netport:Write");

            Open();

            _stream.Write(data, 0, data.Length);
        }
        #endregion

        #region 日志
        /// <summary>日志</summary>
        public ILog Log { get; set; }

        /// <summary>写日志</summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLog(String format, params Object[] args) => Log?.Info(format, args);
        #endregion
    }
}