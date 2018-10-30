using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class CreateUCEventSystem
{
    [MenuItem("GameObject/UC/UC Event System",false , 0)]
    private static void CreateUceventSystem()
    {
        GameObject temp = AssetDatabase.LoadAssetAtPath("Assets/UC/TUIO/UCEventSystem.prefab", typeof(GameObject)) as GameObject;

        if (PrefabUtility.GetPrefabType(temp) == PrefabType.Prefab || PrefabUtility.GetPrefabType(temp) == PrefabType.ModelPrefab)
            PrefabUtility.InstantiatePrefab(temp);
    }
}