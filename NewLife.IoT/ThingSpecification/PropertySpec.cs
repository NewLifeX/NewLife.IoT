using System.Reflection;
using NewLife.Collections;
using NewLife.IoT.ThingModels;

namespace NewLife.IoT.ThingSpecification;

/// <summary>属性规范</summary>
/// <remarks>
/// 用于描述设备运行时具体信息和状态。
/// 例如，环境监测设备所读取的当前环境温度、智能灯开关状态、电风扇风力等级等。
/// 属性可分为读写和只读两种类型。读写类型支持读取和设置属性值，只读类型仅支持读取属性值。
/// </remarks>
public class PropertySpec : SpecBase, IDictionarySource
{
    #region 属性
    /// <summary>
    /// 访问模式
    /// </summary>
    public String? AccessMode { get; set; }

    /// <summary>
    /// 数据类型
    /// </summary>
    public TypeSpec? DataType { get; set; }

    /// <summary>采集点位置信息。常规地址6，Modbus地址 4x0023，位域地址D12.05，虚拟点位地址#</summary>
    public String? Address { get; set; }

    /// <summary>缩放因子。不能是0，默认1，n*scaling+constant</summary>
    public Single Scaling { get; set; } = 1;

    /// <summary>常量因子。默认0，n*scaling+constant</summary>
    public Single Constant { get; set; }

    /// <summary>读取规则。数据解析规则，表达式或脚本</summary>
    public String? ReadRule { get; set; }

    /// <summary>写入规则。数据反解析规则，表达式或脚本</summary>
    public String? WriteRule { get; set; }

    /// <summary>事件模式。在客户端或服务端生成属性变更事件</summary>
    public EventModes EventMode { get; set; }
    #endregion

    #region 创建
    /// <summary>快速创建属性</summary>
    /// <param name="id">标识</param>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="length">长度</param>
    /// <param name="address">地址</param>
    /// <returns></returns>
    public static PropertySpec Create(String id, String name, String type, Int32 length = 0, String? address = null)
    {
        var ps = new PropertySpec
        {
            Id = id,
            Name = name,
            Address = address,
        };

        if (type != null)
            ps.DataType = new TypeSpec { Type = type };

        if (length > 0)
        {
            ps.DataType ??= new TypeSpec();
            ps.DataType.Specs ??= new DataSpecs();
            ps.DataType.Specs.Length = length;
        }
        //if (!address.IsNullOrEmpty())
        //{
        //    ps.DataType ??= new TypeSpec();
        //    ps.DataType.Specs ??= new DataSpecs();
        //    ps.DataType.Specs.Address = address;
        //}

        return ps;
    }

    /// <summary>快速创建属性</summary>
    /// <param name="id">标识</param>
    /// <param name="type">类型</param>
    /// <param name="order">字节序</param>
    /// <returns></returns>
    public static PropertySpec Create(String id, String type, ByteOrder order = ByteOrder.ABCD)
    {
        var ps = new PropertySpec
        {
            Id = id,
        };

        if (type != null)
            ps.DataType = new TypeSpec { Type = type };

        if (order > 0)
        {
            ps.DataType ??= new TypeSpec();
            ps.DataType.Specs = new DataSpecs { Order = order };
        }

        return ps;
    }

    /// <summary>快速创建属性</summary>
    /// <param name="member"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static PropertySpec Create(MemberInfo member, Int32 length = 0)
    {
        //if (member == null) return null;

        var ps = new PropertySpec
        {
            Id = member.Name,
            Name = member.GetDisplayName() ?? member.GetDescription(),
        };

        if (member is PropertyInfo pi)
            ps.DataType = new TypeSpec { Type = pi.PropertyType.Name };
        if (member is FieldInfo fi)
            ps.DataType = new TypeSpec { Type = fi.FieldType.Name };

        if (length > 0 && ps.DataType != null)
            ps.DataType.Specs = new DataSpecs { Length = length };

        return ps;
    }
    #endregion

    #region 方法
    /// <summary>转字典。根据不同类型，提供不一样的序列化能力</summary>
    /// <returns></returns>
    public IDictionary<String, Object?> ToDictionary()
    {
        var dic = new Dictionary<String, Object?>();
        if (!Id.IsNullOrEmpty())
            dic.Add("identifier", Id);
        if (!Name.IsNullOrEmpty())
            dic.Add(nameof(Name), Name);
        if (Required)
            dic.Add(nameof(Required), Required);

        if (!AccessMode.IsNullOrEmpty())
            dic.Add(nameof(AccessMode), AccessMode);

        var dt = DataType?.ToDictionary();
        if (dt != null)
            dic.Add(nameof(DataType), dt);

        if (!Address.IsNullOrEmpty())
            dic.Add(nameof(Address), Address);

        if (Scaling != 1)
            dic[nameof(Scaling)] = Scaling;
        if (Constant != 0)
            dic[nameof(Constant)] = Constant;

        if (!ReadRule.IsNullOrEmpty())
            dic[nameof(ReadRule)] = ReadRule;
        if (!WriteRule.IsNullOrEmpty())
            dic[nameof(WriteRule)] = WriteRule;
        if (EventMode != EventModes.None)
            dic[nameof(EventMode)] = EventMode;

        return dic;
    }

    /// <summary>
    /// 已重载。友好显示
    /// </summary>
    /// <returns></returns>
    public override String ToString() => $"{Id} {Name} {DataType}";
    #endregion
}