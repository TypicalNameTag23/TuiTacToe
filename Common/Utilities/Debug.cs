using System.Reflection;
using System.Text;

namespace Common;

public static class Debug
{
    public static string GetObjectString(this object obj)
    {
        var type = obj.GetType();
        var sb = new StringBuilder($"{type.Name} {{ ");
        foreach (var fieldInfo in obj.GetType().GetProperties())
        {
            sb.Append($"{fieldInfo.Name}: {fieldInfo.GetValue(obj)}, ");
        }

        sb.Append('}');
        return sb.ToString();
    }
}