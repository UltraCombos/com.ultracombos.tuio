using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TUIOManager))]
public class TUIOManagerEditor : Editor
{
    TUIOManager manager;
    
    SerializedProperty tuioPortProperty;

    SerializedProperty blobFiltersProperty;
    ReorderableList blobFiltersList;

    SerializedProperty objectFiltersProperty;
    ReorderableList objectFiltersList;

    SerializedProperty addTuioObjectEventProperty;
    SerializedProperty updateTuioObjectEventProperty;
    SerializedProperty removeTuioObjectEventProperty;

    public void OnEnable()
    {
        manager = target as TUIOManager;

        tuioPortProperty = serializedObject.FindProperty("TuioPort");

        blobFiltersProperty = serializedObject.FindProperty("blobFilters");
        blobFiltersList = new ReorderableList(serializedObject, blobFiltersProperty, true, true, true, true);
        blobFiltersList.drawHeaderCallback = DrawHeaderCallback("Blob Filters");
        blobFiltersList.drawElementCallback = DrawElementCallback(blobFiltersProperty, manager.blobFilters);

        objectFiltersProperty = serializedObject.FindProperty("objectFilters");
        objectFiltersList = new ReorderableList(serializedObject, objectFiltersProperty, true, true, true, true);
        objectFiltersList.drawHeaderCallback = DrawHeaderCallback("Object Filters");
        objectFiltersList.drawElementCallback = DrawElementCallback(objectFiltersProperty, manager.objectFilters);

        addTuioObjectEventProperty = serializedObject.FindProperty("AddTuioObjectEvent");
        updateTuioObjectEventProperty = serializedObject.FindProperty("UpdateTuioObjectEvent");
        removeTuioObjectEventProperty = serializedObject.FindProperty("RemoveTuioObjectEvent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(tuioPortProperty);
        EditorGUILayout.LabelField($"Touch Fps: {manager.TouchFps:F1}");
        EditorGUILayout.LabelField($"Object Fps: {manager.ObjectFps:F1}");
        EditorGUILayout.Space();

        blobFiltersList.DoLayoutList();
        objectFiltersList.DoLayoutList();

        EditorGUILayout.PropertyField(addTuioObjectEventProperty);
        EditorGUILayout.PropertyField(updateTuioObjectEventProperty);
        EditorGUILayout.PropertyField(removeTuioObjectEventProperty);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private ReorderableList.HeaderCallbackDelegate DrawHeaderCallback(string label)
    {
        return rect => { EditorGUI.LabelField(rect, label); };
    }

    private ReorderableList.ElementCallbackDelegate DrawElementCallback<T>(SerializedProperty property, List<T> container) where T : Object
    {
        return (Rect rect, int index, bool active, bool focused) =>
        {
            var prefab = property.GetArrayElementAtIndex(index);
            var go = (T)prefab.objectReferenceValue;

            GUIContent label = (go == null) ? new GUIContent("Empty", "Drag something here.") : new GUIContent(go.name);

            const float shrink = 0.2f;
            rect.y += rect.height * shrink * 0.4f;
            rect.height *= (1.0f - shrink);

            var new_go = (T)EditorGUI.ObjectField(rect, label, go, typeof(T), true);
            if (new_go == null)
            {
                container[index] = null;
                EditorUtility.SetDirty(target);
                return;
            }
            else if (container[index] != new_go)
            {
                container[index] = new_go;
                EditorUtility.SetDirty(target);
            }
        };
    }
}
