using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public abstract class ComponentPrefabLibraryEditor<T> : ComponentObjectLibraryEditor<T,T> where T : MonoBehaviour { }
}