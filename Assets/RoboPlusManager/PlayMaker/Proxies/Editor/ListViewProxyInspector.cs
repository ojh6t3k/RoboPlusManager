using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;


[CustomEditor(typeof(ListViewProxy))]
public class ListViewProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		ListViewProxy proxy = (ListViewProxy)target;
		
		if(proxy.GetComponent<ListView>() == null)
		{
			EditorGUILayout.HelpBox("There is no ListView!", MessageType.Error);
		}
		else
		{
			proxy.eventOnChangedSelection = ProxyInspectorUtil.EventField(target, "OnChangedSelection", proxy.eventOnChangedSelection, proxy.builtInOnChangedSelection);
		}
	}
}
