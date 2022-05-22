using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{    
    public class ComponentPrefabLibrary<T> : ComponentObjectLibrary<T, T> where T : MonoBehaviour { }
}