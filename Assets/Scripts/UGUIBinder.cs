using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UGUIBinder
{
    static Dictionary<Type, Dictionary<FieldInfo, TransformBind>> BindCache;

    static UGUIBinder()
    {
        BindCache = new Dictionary<Type, Dictionary<FieldInfo, TransformBind>>();
    }

    public static void BuildCache(Type type)
    {
        if (!BindCache.TryGetValue(type, out var dict))
        {
            dict = new Dictionary<FieldInfo, TransformBind>();
            BindCache[type] = dict;
        }
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<TransformBind>();
            if (null == attribute)
            {
                continue;
            }

            dict[field] = attribute;
        }
    }

    public static void BuildAllCache(Type type)
    {
        var subs = type.Assembly.GetTypes().Where(t => t.IsSubclassOf(type));
        foreach (var t in subs)
        {
            BuildCache(t);
        }
    }
    
    public static void AutoBind(Component obj)
    {
        var type = obj.GetType();
        var transform = obj.transform;
        
        if (!BindCache.ContainsKey(type))
        {
            BuildCache(type);
            Debug.LogError("可以在游戏初始化阶段预先缓存，提高打开速度");
        }

        var dict = BindCache[type];

        foreach (var data in dict)
        {
            var field = data.Key;
            var attribute = data.Value;
            
            if (!field.FieldType.IsSubclassOf(typeof(UIBehaviour)))
            {
                Debug.LogWarning($"{field.DeclaringType}.{field.Name} 不是UI组件，不能自动绑定。");
                continue;
            }
            var tran = transform.Find(attribute.Path);
            if (!tran)
            {
                Debug.LogError($"{field.DeclaringType}.{field.Name} 没有找到Path[{attribute.Path}]");
                continue;
            }

            var component = tran.GetComponent(field.FieldType);
            if (!component)
            {
                Debug.LogError($"{field.DeclaringType}.{field.Name} 无法在[{attribute.Path}]找到[{field.FieldType}]组件");
                continue;
            }
            field.SetValue(obj, component);
        }
    }
}