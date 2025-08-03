using System.ComponentModel;
using NewLife.IoT.Drivers;
using XCode.DataAccessLayer;

namespace NewLife.IoTDatabase.Drivers;

/// <summary>通用数据库驱动参数</summary>
public class DatabaseParameter : IDriverParameter, IDriverParameterKey
{
    /// <summary>数据库连接字符串。例如：server=.;database=iot;user=iot;password=iot</summary>
    [Description("数据库连接字符串。例如：server=.;database=iot;user=iot;password=iot")]
    public String ConnectionString { get; set; } = null!;

    /// <summary>数据库类型</summary>
    [Description("数据库类型")]
    public DatabaseType DatabaseType { get; set; }

    /// <summary>查询语句。Select查询返回每个字段名对应点位名</summary>
    [Description("查询语句。Select查询返回每个字段名对应点位名")]
    public String QuerySql { get; set; }

    /// <summary>捕获所有字段。捕获所有字段作为返回，而不管指定哪些点位，默认false</summary>
    [Description("捕获所有字段。捕获所有字段作为返回，而不管指定哪些点位，默认false")]
    public Boolean CaptureAll { get; set; }

    /// <summary>获取唯一标识</summary>
    /// <returns></returns>
    public String GetKey() => ConnectionString;
}
