/// <summary>
/// Copy inspector.
/// 
/// Original by:	AngryAnt on November 4th, 2009
/// ------------
/// https://gist.github.com/AngryAnt/3806123#file-copyinspector-cs
/// http://web.archive.org/web/20120521131717/http://eej.dk/angryant/general/tipsandtricks/copyinspector/
/// 
/// IMPLEMENTATION: 
/// --------------
/// Add CopyInspector.cs to Assets/Editor.
/// Add CopyTransformInspector.cs to Assets/Editor.
/// 
/// CUSTOMIZATION:
/// -------------
/// For each other component to be made copy/paste-able:
/// Add CopyYourComponentTypeInspector.cs to Assets/Editor.
/// 	- Rename it appropriately.
/// 	- Rename the class to match the file name.
/// 	- Change YourComponentType in “[CustomEditor (typeof (YourComponentType))]” to the type of component you wish to affect.
/// 
/// 
/// Modified by:	Jared Glass (mushu@live.co.za) @ SIJO STUDIOS, 19/09/2013
/// ------------
/// - Added support for copying multiple components' values.
/// - General Housekeeping (error trapping, neatening etc).
/// 
/// </summary>
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CopyInspector : Editor
{

	static Dictionary <int, CopyData> instanceProperties	= new Dictionary<int, CopyData>();

	private List<PropertyInfo> GetProperties(Component component) {
		List<string> ignoredProperties	= new List<string>();
		List<PropertyInfo> properties	= new List<PropertyInfo> ();
		foreach(PropertyInfo propertyInfo in typeof(Component).GetProperties()) {
			ignoredProperties.Add (propertyInfo.Name);
		}
		foreach(PropertyInfo propertyInfo in component.GetType().GetProperties()) {
			if (ignoredProperties.Contains (propertyInfo.Name)) continue;
			properties.Add(propertyInfo);
		}
		return properties;
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector();
		OnCopyInspectorGUI();
	}

	public void OnCopyInspectorGUI () {
		bool enabled;
		List<PropertyInfo> properties;
		Component component = target as Component;
		if (component == null) return;
		int instanceID		= component.GetInstanceID();
		
		if( 	!instanceProperties.ContainsKey( instanceID ) && 
				!(EditorApplication.isPaused || EditorApplication.isPlaying) ) {
			return;
		}

		GUILayout.Space (10.0f);
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = new Color (0.8f, 0.8f, 0.8f);
		GUILayout.BeginVertical ("Toolbar");
		GUI.backgroundColor = backgroundColor;
		GUILayout.BeginHorizontal ();
		GUILayout.Space (10.0f);	
		
		if( instanceProperties.ContainsKey( instanceID ) ) {
			GUILayout.Label ("Copied: " + (instanceProperties[instanceID].type != null ? instanceProperties[instanceID].type.Name : "Nothing"), "MiniLabel");
		}
		
		GUILayout.FlexibleSpace ();
					
		if (GUILayout.Button (new GUIContent ("Copy", "Copy component values"), "MiniLabel"))
		{
			System.Type type		= target.GetType();
			properties				= GetProperties(component);	
			Dictionary<PropertyInfo, object> m_Values = new Dictionary <PropertyInfo, object> ();
			foreach (PropertyInfo property in properties) {
				m_Values[property]	= property.GetValue (component, null);;
			}
			instanceProperties[instanceID]	= new CopyData(m_Values, type);
			
		}
		enabled			= GUI.enabled;
		if( instanceProperties.ContainsKey( instanceID ) ) {
			enabled		= target.GetType() == instanceProperties[instanceID].type;
		}
		GUI.enabled		= enabled;		
		GUILayout.Space (10.0f);
		if( instanceProperties.ContainsKey( instanceID ) ) {
			if (GUILayout.Button (new GUIContent ("Paste", "Paste component values"), "MiniLabel")) {			
				properties	= GetProperties(component);
				foreach (PropertyInfo property in properties) {
					if( !property.CanWrite ) continue;
					CopyData copyData	= instanceProperties[instanceID];
					property.SetValue(component, copyData.propertyValues[property], null);
				}
				// Remove data from dictionary since it's already served its purpose
				instanceProperties.Remove(instanceID);
			}
		}
		GUILayout.Space(10.0f);
		GUI.enabled		= enabled;
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.Space(-2.0f);
	}
	
	#region CUSTOM CLASSES
	public class CopyData {
		public Dictionary<PropertyInfo, object> propertyValues;
		public System.Type type;
		
		public CopyData( Dictionary<PropertyInfo, object> _propertyValues, System.Type _type ) {
			propertyValues	= _propertyValues;
			type 			= _type;
		}			
	}
	#endregion CUSTOM CLASSES
}