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
public class FileOutputPort : IOutputPort
{
    /// <summary>文件路径</summary>
    public String FileName { get; set; }

    /// <summary>实例化文件驱动输出口</summary>
    /// <param name="fileName"></param>
    public FileOutputPort(String fileName) => FileName = fileName;

    /// <summary>读取开关值</summary>
    /// <returns></returns>
    public virtual Boolean Read() => File.ReadAllText(FileName.GetFullPath())?.Trim() == "1";

    /// <summary>写入开关值</summary>
    /// <param name="value"></param>
    public virtual void Write(Boolean value) => File.WriteAllText(FileName.GetFullPath(), value ? "1" : "0");
}