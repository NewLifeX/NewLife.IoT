using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Serialization;

namespace NewLife.IoT;

public class DataMemberResolver : DefaultJsonTypeInfoResolver
{
    public static DataMemberResolver Default { get; } = new DataMemberResolver();

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object && !type.IsArray)
        {
            var pis = jsonTypeInfo.Properties;
            for (var i = pis.Count - 1; i >= 0; i--)
            {
                var jpi = pis[i];
                var provider = jpi.AttributeProvider;
                if (provider.IsDefined(typeof(IgnoreDataMemberAttribute), false) ||
                    provider.IsDefined(typeof(XmlIgnoreAttribute), false))
                {
                    pis.RemoveAt(i);
                    continue;
                }
                else
                {
                    var attr = provider.GetCustomAttributes(typeof(DataMemberAttribute), false)?.FirstOrDefault() as DataMemberAttribute;
                    if (attr != null && !attr.Name.IsNullOrEmpty())
                    {
                        jpi.Name = attr.Name;
                    }
                }
            }
        }

        return jsonTypeInfo;
    }
}
