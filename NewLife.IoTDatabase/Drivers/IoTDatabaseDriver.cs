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
[Driver("IoTDatabase")]
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
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点对象，可存储站号等信息，仅驱动自己识别</returns>
    public override Task<INode> OpenAsync(IDevice device, IDriverParameter? parameter, CancellationToken cancellationToken = default)
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

        return Task.FromResult(node as INode)!;
    }

    /// <summary>读取数据</summary>
    /// <param name="node">节点对象，可存储站号等信息，仅驱动自己识别</param>
    /// <param name="points">点位集合，Address属性地址示例：D100、C100、W100、H100</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public override Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken = default)
    {
        var client = (node as DatabseNode)?.Dal;
        if (client == null || node.Parameter is not DatabaseParameter parameter)
            return Task.FromResult(ReadResult.Success(points ?? [], new Object?[points?.Length ?? 0]));

        var sql = parameter.QuerySql;
        var dt = client.Query(sql);

        if (dt.Rows == null || dt.Rows.Count == 0)
            return Task.FromResult(ReadResult.Success(points ?? [], new Object?[points?.Length ?? 0]));

        // 只要第一行数据
        var row = dt.Rows[0];

        // 按列名建立查找字典
        var colDict = new Dictionary<String, Object?>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < dt.Columns.Length; i++)
            colDict[dt.Columns[i]] = row[i];

        if (points != null && points.Length > 0)
        {
            var values = new Object?[points.Length];
            for (var i = 0; i < points.Length; i++)
            {
                var pt = points[i];
                var matched = colDict.TryGetValue(pt.Name, out var val);
                if (!matched && !pt.Address.IsNullOrEmpty())
                    matched = colDict.TryGetValue(pt.Address!, out val);
                if (!matched) continue;

                var type = pt.GetNetType();
                values[i] = type != null ? val.ChangeType(type) : val;
            }
            return Task.FromResult(ReadResult.Success(points, values));
        }
        else if (parameter.CaptureAll)
        {
            // 无点位且允许捕获所有：动态建立点位
            var dynPoints = new IPoint[colDict.Count];
            var dynValues = new Object?[colDict.Count];
            var idx = 0;
            foreach (var kv in colDict)
            {
                dynPoints[idx] = new PointModel { Name = kv.Key };
                dynValues[idx] = kv.Value;
                idx++;
            }
            return Task.FromResult(ReadResult.Success(dynPoints, dynValues));
        }

        return Task.FromResult(ReadResult.Success([], []));
    }

    /// <summary>写入数据。数据库驱动不支持写入，始终返回失败</summary>
    /// <param name="node">节点对象</param>
    /// <param name="requests">写入请求数组</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>写入结果</returns>
    public override Task<WriteResult> WriteAsync(INode node, WriteRequest[] requests, CancellationToken cancellationToken = default)
        => Task.FromResult(WriteResult.Fail(IoTErrorCode.NotSupported, "数据库驱动不支持写入操作"));

    /// <summary>设备控制</summary>
    /// <param name="node">节点对象</param>
    /// <param name="request">服务调用请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>服务调用结果</returns>
    public override Task<ServiceResult> ControlAsync(INode node, ServiceCall request, CancellationToken cancellationToken = default)
        => Task.FromResult(ServiceResult.Fail(IoTErrorCode.NotSupported, "数据库驱动不支持服务控制操作"));
    #endregion
}