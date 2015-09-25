using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CommSocket))]
public class CommSocketInspector : Editor
{
    SerializedProperty device;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnFoundDevice;
    SerializedProperty OnSearchCompleted;

    bool foldoutDevice = false;

    void OnEnable()
    {
        device = serializedObject.FindProperty("device");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
        OnFoundDevice = serializedObject.FindProperty("OnFoundDevice");
        OnSearchCompleted = serializedObject.FindProperty("OnSearchCompleted");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        CommSocket socket = (CommSocket)target;

        foldoutDevice = EditorGUILayout.Foldout(foldoutDevice, string.Format("Device: {0}", socket.device.displayName));
        if(foldoutDevice)
        {
            if (Application.isPlaying == true)
                GUI.enabled = !socket.isOpen;

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(device.FindPropertyRelative("name"), new GUIContent("Name"));
            EditorGUILayout.PropertyField(device.FindPropertyRelative("type"), new GUIContent("Type"));
            SerializedProperty devArgs = device.FindPropertyRelative("args");
            if (socket.device.type == CommDevice.Type.Serial)
            {
                EditorGUILayout.PropertyField(device.FindPropertyRelative("address"), new GUIContent("Port"));
                devArgs.arraySize = 1;
                EditorGUILayout.PropertyField(devArgs.GetArrayElementAtIndex(0), new GUIContent("Baudrate"));
            }
            else if(socket.device.type == CommDevice.Type.BT)
            {
                EditorGUILayout.PropertyField(device.FindPropertyRelative("address"), new GUIContent("Address"));
                devArgs.arraySize = 0;
            }
            else if (socket.device.type == CommDevice.Type.BLE)
            {
                EditorGUILayout.PropertyField(device.FindPropertyRelative("address"), new GUIContent("Address"));
                devArgs.arraySize = 1;
                EditorGUILayout.PropertyField(devArgs.GetArrayElementAtIndex(0), new GUIContent("UUID"));
            }

            EditorGUILayout.BeginHorizontal();
            int oldIndex = -1;
            string[] list = new string[socket.foundDevices.Count];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = socket.foundDevices[i].displayName;
                if (socket.device.Equals(socket.foundDevices[i]))
                    oldIndex = i;
            }
            int newIndex = EditorGUILayout.Popup("", oldIndex, list);
            if (newIndex != oldIndex)
            {
                socket.device = new CommDevice(socket.foundDevices[newIndex]);
                device = serializedObject.FindProperty("device");
            }
            if (GUILayout.Button("Search", GUILayout.Width(60f)))
                socket.Search();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
            GUI.enabled = true;
        }        

        if (Application.isPlaying == true)
        {
            if(socket.isOpen)
            {
                if (GUILayout.Button("Close") == true)
                    socket.Close();
            }
            else
            {
                if (GUILayout.Button("Open") == true)
                    socket.Open();
            }

            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
        EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
        EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
        EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
        EditorGUILayout.PropertyField(OnFoundDevice, new GUIContent("OnFoundDevice"));
        EditorGUILayout.PropertyField(OnSearchCompleted, new GUIContent("OnSearchCompleted"));

        this.serializedObject.ApplyModifiedProperties();
    }
}
