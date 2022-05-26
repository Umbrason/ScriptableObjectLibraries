using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class ComponentObjectLibraryEditor<Key, Value> : UnityEditor.Editor where Key : MonoBehaviour where Value : UnityEngine.Object
{
    private TypeObjectLibrary<Key, Value> library;
    private Type[] derivedTypes;

    void OnEnable()
    {
        library = target as TypeObjectLibrary<Key, Value>;
        derivedTypes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x=>x.GetTypes()).Where((t) => (!t.IsAbstract) && (!t.IsGenericType) && t.IsSubclassOf(typeof(Key))).Distinct().ToArray();
        derivedTypes.OrderBy(x => x.Name);
    }

    public override void OnInspectorGUI()
    {
        if (library == null)
            return;

        foreach (var type in derivedTypes)
            library[type] = EditorGUILayout.ObjectField(type.Name, library[type], typeof(Value), allowSceneObjects: false) as Value;
        if (GUI.changed)
            EditorUtility.SetDirty(library);

    }
}
