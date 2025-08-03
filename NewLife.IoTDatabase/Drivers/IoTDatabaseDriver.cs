using System.ComponentModel;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.ThingModels;
using NewLife.Reflection;
using XCode.DataAccessLayer;

namespace NewLife.IoTDatabase.Drivers;

/// <summary>IoT标准通用数据库驱动</summary>
/// <remarks>
/// IoT驱动，符合IoT标准的通用数据库驱动，连接后向目标发送数据即可收到数据。
/// </remarks>
[Driver("IoTSocket")]
[DisplayName("通用数据库驱动")]
public class IoTDatabaseDriver : DriverBase<DatabseNode, DatabaseParameter>
{
    #region 属性
    #endregion

    #region 方法
    /// <summary>
    /// 打开设备驱动，传入参数。一个物理设备可能有多个逻辑设备共用，需要以节点来区分
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">参数。不同驱动的参数设置相差较大，对象字典具有较好灵活性，其对应IDriverParameter</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override INode Open(IDevice device, IDriverParameter parameter)
    {
        if (parameter is not DatabaseParameter p) throw new ArgumentException("参数不能为空");
        if (p.ConnectionString.IsNullOrEmpty()) throw new ArgumentException("数据库地址不能为空");

        var node = new DatabseNode
        {
            Driver = this,
            Device = device,
            Parameter = p,
            DatabaseParameter = p,
        };

        var connName = p.ConnectionString.GetBytes().Crc().GetBytes().ToHex();
        if (!DAL.ConnStrs.ContainsKey(connName))
        {
            DAL.AddConnStr(connName, p.ConnectionString, null, p.DatabaseType + "");
        }

        node.Dal = DAL.Create(connName);

        return node;
    }

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合，Address属性地址示例：D100、C100、W100、H100</param>
    /// <returns></returns>
    public override IDictionary<String, Object> Read(INode node, IPoint[] points)
    {
        var result = new Dictionary<String, Object>();
        //if (points == null) return result;

        var client = (node as DatabseNode)?.Dal;
        if (client == null || node.Parameter is not DatabaseParameter parameter) return result;

        var sql = parameter.QuerySql;
        var dt = client.Query(sql);

        if (dt.Rows != null && dt.Rows.Count > 0)
        {
            // 只要第一行数据
            var row = dt.Rows[0];
            for (var i = 0; i < dt.Columns.Length; i++)
            {
                var name = dt.Columns[i];
                var value = row[i];

                // 如果点位存在，赋值
                var point = points?.FirstOrDefault(p => name.EqualIgnoreCase(p.Name, p.Address));
                if (point != null)
                {
                    // 如果点位有类型，转换类型
                    var type = point.GetNetType();
                    if (type != null) value = value.ChangeType(type);

                    result[point.Name] = value;
                }
                else if (parameter.CaptureAll)
                {
                    // 如果点位没有指定，且允许捕获所有字段，则直接返回
                    result[name] = value;
                }
            }
        }

        return result;
    }

    /// <summary>写入数据</summary>
    public override Object Write(INode node, IPoint point, Object value) => throw new NotSupportedException();

    /// <summary>设备控制</summary>
    /// <param name="node"></param>
    /// <param name="parameters"></param>
    public override Object Control(INode node, IDictionary<String, Object> parameters) => throw new NotSupportedException();
    #endregion
}