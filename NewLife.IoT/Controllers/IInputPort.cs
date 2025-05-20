namespace NewLife.IoT.Controllers;

/// <summary>开关量输入口</summary>
public interface IInputPort
{
    /// <summary>读取开关值</summary>
    /// <returns></returns>
    Boolean Read();
}

/// <summary>文件驱动输入口</summary>
/// <remarks>实例化文件驱动输入口</remarks>
/// <param name="fileName"></param>
public class FileInputPort(String fileName) : DisposeBase, IInputPort
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

    private FileStream GetFile() => _fs ??= new FileStream(FileName.GetFullPath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    /// <summary>读取开关值</summary>
    /// <returns></returns>
    public virtual Boolean Read()
    {
        var fs = GetFile();

        return fs.ReadByte() == '1';
    }
}