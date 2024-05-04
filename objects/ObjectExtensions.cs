using System;
using System.Reflection;

public static class ObjectExtensions {
    public static T GetProperty<T>(this object obj, string prop) {
        var propertyInfo = obj.GetType().GetProperty(prop) ?? 
            throw new ArgumentException($"Property '{prop}' not found on object of type '{obj.GetType().FullName}'.");
        return (T)propertyInfo.GetValue(obj);
    }

    public static bool HasProperty(this object obj, string prop) {
        return obj.GetType().GetRuntimeProperty(prop) != null;
    }
}