/// <summary>
/// Copy transform inspector.
/// Original by:  AngryAnt on November 4th, 2009
/// https://gist.github.com/AngryAnt/3806123#file-copyinspector-cs
/// 
/// Modified by:	Jared Glass (mushu@live.co.za) @ SIJO STUDIOS, 19/09/2013
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(Transform))]
public class CopyTransformInspector : CopyInspector
{
	public override void OnInspectorGUI ()
	{
		Transform transform;
		Vector3 localPosition, localScale;
		Quaternion localRotation;

		transform = target as Transform;

		localPosition	= EditorGUILayout.Vector3Field ("Position", transform.localPosition);
		localRotation	= Quaternion.Euler(EditorGUILayout.Vector3Field ("Rotation", transform.localRotation.eulerAngles));
		localScale		= EditorGUILayout.Vector3Field ("Scale", transform.localScale);

		if(GUI.changed) {
			transform.localPosition	= localPosition;
			transform.localRotation	= localRotation;
			transform.localScale	= localScale;
		}
		
		OnCopyInspectorGUI ();
	}
}