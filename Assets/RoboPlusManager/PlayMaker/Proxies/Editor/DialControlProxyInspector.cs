using UnityEngine;
using System.Collections;
using UnityEditor;
using HutongGames.PlayMaker;


[CustomEditor(typeof(DialControlProxy))]
public class DialControlProxyInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DialControlProxy proxy = (DialControlProxy)target;
		
		if(proxy.GetComponent<DialControl>() == null)
		{
			EditorGUILayout.HelpBox("There is no DialControl!", MessageType.Error);
		}
		else
		{
			proxy.eventOnChangedValue = ProxyInspectorUtil.EventField(target, "OnChangedValue", proxy.eventOnChangedValue, proxy.builtInOnChangedValue);
		}
	}
}
