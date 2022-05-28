using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class TypeObjectLibraryEditor<Key, Value> : UnityEditor.Editor where Value : UnityEngine.Object
{
    readonly string[] IGNORED_ASSEMBLY_PREFIXES = {
        "UNITYEDITOR",
        "UNITYENGINE",
        "UNITY",
        "SYSTEM",
        "MSCORLIB"
    };

    private TypeObjectLibrary<Key, Value> library;
    private Type[] derivedTypes;

    void OnEnable()
    {
        library = target as TypeObjectLibrary<Key, Value>;
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !IGNORED_ASSEMBLY_PREFIXES.Any(prefix => assembly.FullName.ToLower().StartsWith(prefix.ToLower()))).ToArray();
        var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
        var subtypes = types.Where((t) => (!t.IsAbstract) && (!t.IsGenericType) && t.IsSubclassOf(typeof(Key))).ToArray();
        derivedTypes = subtypes;
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
