using System;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetStringValue<T>(this T enumValue) where T : Enum
    {
        var type = enumValue.GetType();
        var fieldInfo = type.GetField(enumValue.ToString());
        var attributes = (StringValueAttribute[])fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);

        return attributes.Length > 0 ? attributes[0].Value : enumValue.ToString();
    }
}

public class StringValueAttribute : Attribute
{
    public string Value { get; }

    public StringValueAttribute(string value)
    {
        Value = value;
    }
}
