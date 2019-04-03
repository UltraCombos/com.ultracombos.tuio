using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class CreateUCEventSystem
{
    [MenuItem("GameObject/Ultra Combos/UC Event System",false , 0)]
    private static void CreateUceventSystem()
    {
        //GameObject temp = AssetDatabase.LoadAssetAtPath("Assets/UC/TUIO/UCEventSystem.prefab", typeof(GameObject)) as GameObject;
        GameObject temp = AssetDatabase.LoadAssetAtPath("Packages/com.ultracombos.tuio/Runtime/UCEventSystem.prefab", typeof(GameObject)) as GameObject;

        if (PrefabUtility.GetPrefabType(temp) == PrefabType.Prefab || PrefabUtility.GetPrefabType(temp) == PrefabType.ModelPrefab)
            PrefabUtility.InstantiatePrefab(temp);
    }
}