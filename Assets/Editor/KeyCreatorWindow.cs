using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeysetCreatorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D keysetTexture;
	private Color color = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1);

	private Rect headerSection;
	private Rect keysetSection;

	private GUISkin skin;

	private List<string> keys;

	private Vector2 scrollPos;
	private int dialogHeight = 80;
	private int keyNum = 1;
	private string keysetName = "";

	[MenuItem("Window/Keyset Creator")]
	private static void OpenWindow() {
		KeysetCreatorWindow window = (KeysetCreatorWindow)GetWindow(typeof(KeysetCreatorWindow));
		window.minSize = new Vector2(200, 300);
		window.Show();
	}

	private void OnEnable() {
		InitTextures();
		skin = Resources.Load<GUISkin>("GUISkins/DialogWindowSkin");

		keys = new List<string>();
		keys.Add("");
	}

	private void InitTextures() {
		headerTexture = new Texture2D(1, 1);
		headerTexture.SetPixel(0, 0, color);
		headerTexture.Apply();

		keysetTexture = new Texture2D(1, 1);
		keysetTexture.SetPixel(0, 0, color);
		keysetTexture.Apply();
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeaderSection();
		DrawDialogSection();
	}

	private void DrawLayouts() {
		headerSection.x = 0;
		headerSection.y = 0;
		headerSection.width = position.width;
		headerSection.height = 80;

		keysetSection.x = 5;
		keysetSection.y = headerSection.height;
		keysetSection.width = position.width - 10;
		keysetSection.height = position.height - headerSection.height - 5;

		GUI.DrawTexture(headerSection, headerTexture);
		GUI.DrawTexture(keysetSection, keysetTexture);
	}

	private void DrawHeaderSection() {
		GUILayout.BeginArea(headerSection);
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label("Keyset Creator", skin.GetStyle("Header"));

		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawDialogSection() {
		GUILayout.BeginArea(keysetSection);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space(5);
		GUILayout.Label("Keyset Name", skin.GetStyle("Dialog"));
		GUILayout.FlexibleSpace();
		keysetName = EditorGUILayout.TextField(keysetName, GUILayout.Width(position.width - 110));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(4);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space(5);
		GUILayout.Label("Keys", skin.GetStyle("Dialog"), GUILayout.Width(50));
		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginVertical();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		for (int i = 0; i < keyNum; i++) {
			EditorGUILayout.BeginHorizontal();
			string language = EditorGUILayout.TextField(keys[i], GUILayout.Width(position.width - 110));

			keys[i] = language;

			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space(4);

		if (GUILayout.Button("Save", skin.GetStyle("Button"), GUILayout.Width(50), GUILayout.Height(24))) {
			SaveDialogData();
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("+", skin.GetStyle("Button"), GUILayout.Width(24), GUILayout.Height(24))) {
			keys.Add("");
			dialogHeight += 20;
			keyNum++;
		}

		if (GUILayout.Button("-", skin.GetStyle("Button"), GUILayout.Width(24), GUILayout.Height(24))) {
			if (keyNum > 1) {
				dialogHeight -= 20;
				keyNum--;
				keys.RemoveAt(keyNum);
			}
		}

		EditorGUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void SaveDialogData() {
		if (keysetName == "")
			EditorUtility.DisplayDialog("Missing Name", "Name for keyset required.", "Ok", "");
		else {
			keys = keys.Distinct().ToList();
			keyNum = keys.Count();

			for (int i = 0; i < keyNum; i++) {
				if (keys[i] == "") {
					if (keyNum == 1) {
						EditorUtility.DisplayDialog("No Keys Entered", "There are no keys entered.", "Ok", "");
						return;
					}

					keys.RemoveAt(i);
					dialogHeight -= 20;
					keyNum--;
				}
			}

			Keyset keyset = (Keyset)CreateInstance(typeof(Keyset));
			keyset.keys = keys;
			AssetDatabase.CreateAsset(keyset, "Assets/Resources/Keysets/" + keysetName + ".asset");
		}
	}
}
