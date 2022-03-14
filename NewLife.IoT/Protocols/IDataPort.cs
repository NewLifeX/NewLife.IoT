using NewLife.Log;

namespace NewLife.IoT.Protocols
{
    /// <summary>数据端口</summary>
    public interface IDataPort
    {
        #region 属性
        /// <summary>性能追踪器</summary>
        ITracer Tracer { get; set; }
        #endregion

        #region 核心方法
        /// <summary>初始化。传入配置字符串</summary>
        /// <param name="config"></param>
        void Init(String config);

        /// <summary>读取数据</summary>
        /// <returns></returns>
        Byte[] Read();

        /// <summary>写入数据</summary>
        /// <param name="data"></param>
        void Write(Byte[] data);
        #endregion

        #region 日志
        /// <summary>日志</summary>
        ILog Log { get; set; }
        #endregion
    }
}