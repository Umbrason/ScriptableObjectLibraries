using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class ComponentObjectLibraryEditor<Key, Value> : UnityEditor.Editor where Key : MonoBehaviour where Value : UnityEngine.Object
{
    private ComponentObjectLibrary<Key, Value> library;

    void OnEnable()
    {
        library = target as ComponentObjectLibrary<Key, Value>;
    }

    public override void OnInspectorGUI()
    {
        if (library == null)
            return;
        var derivedTypes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x=>x.GetTypes()).Distinct().Where((t) => (!t.IsAbstract) && (!t.IsGenericType) && t.IsSubclassOf(typeof(Key))).ToArray();        
        derivedTypes.OrderBy(x => x.Name);
        foreach (var type in derivedTypes)
            library[type] = EditorGUILayout.ObjectField(type.Name, library[type], typeof(Key), allowSceneObjects: false) as Value;
        if (GUI.changed)
            EditorUtility.SetDirty(library);

    }
}
