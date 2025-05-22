namespace NewLife.IoT.Controllers;

/// <summary>开关量输出口</summary>
public interface IOutputPort
{
    /// <summary>读取开关值</summary>
    /// <returns></returns>
    Boolean Read();

    /// <summary>写入开关值</summary>
    /// <param name="value"></param>
    void Write(Boolean value);
}

/// <summary>文件驱动输出口</summary>
/// <remarks>实例化文件驱动输出口</remarks>
/// <param name="fileName"></param>
public class FileOutputPort(String fileName) : DisposeBase, IOutputPort
{
    /// <summary>文件路径</summary>
    public String FileName { get; set; } = fileName;

    private FileStream? _fs;

    /// <summary>销毁</summary>
    /// <param name="disposing"></param>
    protected override void Dispose(Boolean disposing)
    {
        base.Dispose(disposing);

        _fs.TryDispose();
    }

    /// <summary>获取文件流</summary>
    /// <returns></returns>
    protected virtual FileStream GetFile() => _fs ??= new FileStream(FileName.GetFullPath(), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

    /// <summary>读取开关值</summary>
    /// <returns></returns>
    public virtual Boolean Read()
    {
        var fs = GetFile();

        return fs.ReadByte() == '1';
    }

    /// <summary>写入开关值</summary>
    /// <param name="value"></param>
    public virtual void Write(Boolean value)
    {
        var fs = GetFile();
        fs.WriteByte((Byte)(value ? '1' : '0'));
        fs.Flush();
    }
}