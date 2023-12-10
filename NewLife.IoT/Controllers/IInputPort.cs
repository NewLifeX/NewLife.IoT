namespace NewLife.IoT.Controllers;

/// <summary>开关量输入口</summary>
public interface IInputPort
{
    /// <summary>读取开关值</summary>
    /// <returns></returns>
    Boolean Read();
}

/// <summary>文件驱动输入口</summary>
public class FileInputPort : IInputPort
{
    /// <summary>文件路径</summary>
    public String FileName { get; set; } = null!;

    /// <summary>实例化文件驱动输入口</summary>
    /// <param name="fileName"></param>
    public FileInputPort(String fileName) => FileName = fileName;

    /// <summary>读取开关值</summary>
    /// <returns></returns>
    public virtual Boolean Read() => File.ReadAllText(FileName.GetFullPath())?.Trim() == "1";
}