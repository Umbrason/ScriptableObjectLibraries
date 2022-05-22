using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game
{
    public class ComponentObjectLibrary<KeyBaseClass, Value> : ScriptableObject, ISerializationCallbackReceiver where KeyBaseClass : MonoBehaviour where Value : UnityEngine.Object
    {
        [SerializeField] public string[] _keys;
        [SerializeField] public Value[] _values;

        public Dictionary<Type, Value> componentObjects;
        public Value this[Type t]
        {
            get
            {
                return componentObjects.TryGetValue(t, out Value value) ? value : default;
            }
            set
            {
                if (t.IsSubclassOf(typeof(KeyBaseClass)) && !t.IsGenericType && !t.IsAbstract)
                    componentObjects[t] = value;
            } 
        }

        public void OnAfterDeserialize()
        {
            componentObjects = new Dictionary<Type, Value>();
            if (_keys != null && _values != null)
                for (int i = 0; i < Mathf.Min(_keys.Length, _values.Length); i++)
                    componentObjects.Add(Type.GetType(_keys[i]), _values[i]);
        }

        public void OnBeforeSerialize()
        {
            var keyTypes = componentObjects.Keys.ToArray();
            _keys = componentObjects.Keys.Select(t => $"{t.AssemblyQualifiedName}|{t.Assembly.FullName}").ToArray();
            _values = keyTypes.Select(key => componentObjects[key]).ToArray();
        }
    }
}