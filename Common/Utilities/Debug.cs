using System.Reflection;
using System.Text;

namespace Common;

public static class Debug
{
    public static string GetObjectString(this object obj)
    {
        var type = obj.GetType();
        var sb = new StringBuilder($"{type.Name} {{ ");
        var properties = type.GetProperties();
        for (var i = 0; i < properties.Length; i++)
        {
            var fieldInfo = properties[i];
            sb.Append($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}");
            if (i < properties.Length - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append('}');
        return sb.ToString();
    }
}