using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class TypeObjectLibraryEditor<Key, Value> : UnityEditor.Editor where Value : UnityEngine.Object
{
    static readonly string[] IGNORED_ASSEMBLY_PREFIXES = {
        "UnityEditor",
        "UnityEngine",
        "Unity",
        "System",
        "mscorlib"
    };

    private TypeObjectLibrary<Key, Value> library;
    private Type[] derivedTypes;

    void OnEnable()
    {
        library = target as TypeObjectLibrary<Key, Value>;
        derivedTypes = System.AppDomain.CurrentDomain.GetAssemblies()
        .Where(assembly => !IGNORED_ASSEMBLY_PREFIXES.Any(prefix => assembly.FullName.StartsWith(prefix)))
        .SelectMany(x => x.GetTypes())
        .Where((t) => (!t.IsAbstract) && (!t.IsGenericType) && t.IsSubclassOf(typeof(Key)))
        .Where(IsValidType)
        .ToArray();        
        derivedTypes.OrderBy(x => x.Name);
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
