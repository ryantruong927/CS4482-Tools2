using System;
using System.Collections.Generic;
using System.IO;
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

	private List<string> dialogueNames;

	private Vector2 scrollPos;
	private int dialogueNum = 1;
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

		dialogueNames = new List<string>();
		dialogueNames.Add("");
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
		DrawKeysetSection();
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

	private void DrawKeysetSection() {
		GUILayout.BeginArea(keysetSection);

		EditorGUILayout.HelpBox("Create a name for a keyset (ex. MainMenu, Character1, etc.) and their names (dialogues). This keyset determines which dialogues can be added to a language.", MessageType.Info);

		EditorGUILayout.Space(4);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Keyset Name", GUILayout.Width(108));
		keysetName = EditorGUILayout.TextField(keysetName);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(4);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Dialogue Names", GUILayout.Width(105));

		EditorGUILayout.BeginVertical();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		for (int i = 0; i < dialogueNum; i++) {
			EditorGUILayout.BeginHorizontal();
			string language = EditorGUILayout.TextField(dialogueNames[i]);

			dialogueNames[i] = language;

			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(24))) {
			SaveDialogueData();
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("+", GUILayout.Width(24), GUILayout.Height(24))) {
			dialogueNames.Add("");
			dialogueNum++;
		}

		if (GUILayout.Button("-", GUILayout.Width(24), GUILayout.Height(24))) {
			if (dialogueNum > 1) {
				dialogueNum--;
				dialogueNames.RemoveAt(dialogueNum);
			}
		}

		EditorGUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void SaveDialogueData() {
		if (keysetName == "")
			EditorUtility.DisplayDialog("Missing Name", "Name for keyset required.", "Ok", "");
		else {
			dialogueNames = dialogueNames.Distinct().ToList();
			dialogueNum = dialogueNames.Count();

			for (int i = 0; i < dialogueNum; i++) {
				if (dialogueNames[i] == "") {
					if (dialogueNum == 1) {
						EditorUtility.DisplayDialog("No Keys Entered", "There are no keys entered.", "Ok", "");
						return;
					}

					dialogueNames.RemoveAt(i);
					dialogueNum--;
				}
			}

			string keysetPath = "Assets/Resources/Keysets/" + keysetName + ".asset";
			Keyset keyset = (Keyset)CreateInstance(typeof(Keyset));
			keyset.dialogueNames = dialogueNames;
			if (File.Exists(keysetPath)) {
				EditorUtility.DisplayDialog("Keyset Already Exists", "A keyset with the name '" + keysetName + "' already exists.", "Ok", "");
				return;
			}

			AssetDatabase.CreateAsset(keyset, keysetPath);
		}
	}
}
