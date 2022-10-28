using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct DialogStruct {
    public string language;
    public string text;
}

[CustomPropertyDrawer(typeof(DialogStruct))]
public class DialogDrawer : PropertyDrawer {
    public void DrawDrawer() {
        GUILayout.BeginHorizontal();
		GUILayout.TextField("");
        GUILayout.EndHorizontal();
	}
}
