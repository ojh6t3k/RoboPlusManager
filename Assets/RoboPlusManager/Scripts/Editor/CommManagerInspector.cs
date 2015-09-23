using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CommManager))]
public class CommManagerInspector : Editor
{
    SerializedProperty devices;
    SerializedProperty device;
    SerializedProperty baudrate;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnForceClosed;
    SerializedProperty OnFoundDevice;
    SerializedProperty OnSearchCompleted;

    void OnEnable()
    {
        devices = serializedObject.FindProperty("devices");
        device = serializedObject.FindProperty("device");
        baudrate = serializedObject.FindProperty("baudrate");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnForceClosed = serializedObject.FindProperty("OnForceClosed");
        OnFoundDevice = serializedObject.FindProperty("OnFoundDevice");
        OnSearchCompleted = serializedObject.FindProperty("OnSearchCompleted");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        CommManager manager = (CommManager)target;

        EditorGUILayout.PropertyField(device, new GUIContent("Device"));
        EditorGUILayout.BeginHorizontal();
        int index = -1;
        string[] list = new string[devices.arraySize];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = devices.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
            if (device.FindPropertyRelative("name").stringValue.Equals(list[i]) == true)
                index = i;
        }
        index = EditorGUILayout.Popup("Devices", index, list);
        if (index >= 0)
            device = devices.GetArrayElementAtIndex(index);
        if (GUILayout.Button("Search", GUILayout.Width(60f)) == true)
            manager.Search();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
        EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
        EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
        EditorGUILayout.PropertyField(OnForceClosed, new GUIContent("OnForceClosed"));
        EditorGUILayout.PropertyField(OnFoundDevice, new GUIContent("OnFoundDevice"));
        EditorGUILayout.PropertyField(OnSearchCompleted, new GUIContent("OnSearchCompleted"));

        this.serializedObject.ApplyModifiedProperties();
    }
}
