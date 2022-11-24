using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class TypeObjectLibraryEditor<Key, Value> : UnityEditor.Editor where Value : UnityEngine.Object
{
    private TypeObjectLibrary<Key, Value> library;
    private Type[] derivedTypes;

    void OnEnable()
    {
        library = target as TypeObjectLibrary<Key, Value>;
        derivedTypes = TypeCache.GetTypesDerivedFrom<Key>().Where(t => !t.IsAbstract && !t.IsInterface).OrderBy(t => t.Name).ToArray();        
    }

    public virtual bool IsValidType(Type type) => true;

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
