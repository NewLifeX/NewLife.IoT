namespace NewLife.IoT.Controllers;

/// <summary>开关量输入口</summary>
public interface IInputPort
{
    /// <summary>读取开关值</summary>
    /// <returns></returns>
    Boolean Read();

    /// <summary>按下事件</summary>
    event EventHandler<KeyEventArgs> KeyDown;

    /// <summary>弹起事件</summary>
    event EventHandler<KeyEventArgs> KeyUp;
}

/// <summary>按键事件参数</summary>
public class KeyEventArgs(Boolean value) : EventArgs
{
    /// <summary>是否已处理</summary>
    public Boolean Handled { get; set; }

    /// <summary>当前值（按下为 true，松开为 false）</summary>
    public Boolean Value { get; set; } = value;
}

/// <summary>文件驱动输入口</summary>
/// <remarks>实例化文件驱动输入口</remarks>
/// <param name="fileName"></param>
public class FileInputPort(String fileName) : DisposeBase, IInputPort
{
    #region 属性
    /// <summary>文件路径</summary>
    public String FileName { get; set; } = fileName;

    /// <summary>轮询间隔。默认100毫秒</summary>
    public Int32 Period { get; set; } = 100;

    private FileStream? _fs;
    #endregion

    /// <summary>销毁</summary>
    /// <param name="disposing"></param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        StopMonitor();
        _fs.TryDispose();
    }

    /// <summary>获取文件流</summary>
    /// <returns></returns>
    protected virtual FileStream GetFile() => _fs ??= new FileStream(FileName.GetFullPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    /// <summary>读取开关值</summary>
    /// <returns></returns>
    public virtual Boolean Read()
    {
        var fs = GetFile();

        return fs.ReadByte() == '1';
    }

    #region 事件处理
    private EventHandler<KeyEventArgs>? _keyDown;
    /// <summary>按下事件</summary>
    public event EventHandler<KeyEventArgs> KeyDown
    {
        add
        {
            _keyDown += value;
            StartMonitor();
        }
        remove
        {
            _keyDown -= value;
            StopMonitor();
        }
    }

    private EventHandler<KeyEventArgs>? _keyUp;
    /// <summary>弹起事件</summary>
    public event EventHandler<KeyEventArgs> KeyUp
    {
        add
        {
            _keyUp += value;
            StartMonitor();
        }
        remove
        {
            _keyUp -= value;
            StopMonitor();
        }
    }

    private Timer? _timer;
    private Boolean _lastValue;
    private void StartMonitor()
    {
        if (_timer != null) return;

        _timer = new Timer(DoMonitor, null, 0, Period);
    }

    private void StopMonitor()
    {
        _timer?.Dispose();
        _timer = null;
    }

    private void DoMonitor(Object? state)
    {
        var value = Read();
        if (value != _lastValue)
        {
            var args = new KeyEventArgs(value);
            if (value)
                _keyDown?.Invoke(this, args);
            else
                _keyUp?.Invoke(this, args);

            _lastValue = value;
        }
    }
    #endregion
}