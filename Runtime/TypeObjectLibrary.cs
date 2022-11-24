using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TypeObjectLibrary<KeyBaseClass, Value> : ScriptableObject, ISerializationCallbackReceiver where Value : UnityEngine.Object
{
    public static TypeObjectLibrary<KeyBaseClass, Value> m_instance;
    public static TypeObjectLibrary<KeyBaseClass, Value> Instance
    {
        get
        {
            if (m_instance) return m_instance;
            var occurences = Resources.LoadAll<TypeObjectLibrary<KeyBaseClass, Value>>("");
            if (occurences.Length > 1)
                Debug.LogWarning($"more than one library of the requested type <{typeof(KeyBaseClass).Name}, {typeof(Value).Name}> was found!\n Remove duplicate library types to prevent unexpected behaviour.");
            if (occurences.Length == 0)
                Debug.LogError($"no library of the requested type <{typeof(KeyBaseClass).Name}, {typeof(Value).Name}> was found!\n Add one to any resource folder.");
            return m_instance ??= occurences.FirstOrDefault();
        }
    }


    [SerializeField] private string[] _keys;
    [SerializeField] private Value[] _values;

    public Dictionary<Type, Value> componentObjects = new Dictionary<Type, Value>();
    public Value this[Type t]
    {
        get
        {
            return componentObjects.TryGetValue(t, out Value value) ? value : default;
        }
        set
        {
            if ((t.IsSubclassOf(typeof(KeyBaseClass)) || t.GetInterfaces().Contains(typeof(KeyBaseClass))) && !t.IsGenericType && !t.IsAbstract)
                componentObjects[t] = value;
            else Debug.Log(t.Name);
        }
    }

    public void OnAfterDeserialize()
    {
        componentObjects = new Dictionary<Type, Value>();
        if (_keys != null && _values != null)
            for (int i = 0; i < Mathf.Min(_keys.Length, _values.Length); i++)
                if (_keys[i] != null)
                    componentObjects.Add(Type.GetType(_keys[i]), _values[i]);
    }

    public void OnBeforeSerialize()
    {
        var keyTypes = componentObjects.Keys.ToArray();
        _keys = componentObjects.Keys.Select(t => $"{t.AssemblyQualifiedName}|{t.Assembly.FullName}").ToArray();
        _values = keyTypes.Select(key => componentObjects[key]).ToArray();
    }    
}
