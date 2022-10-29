using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeysetEditorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D keysetSelectionTexture;
	private Texture2D keysetTexture;
	private Color color = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1);

	private Rect headerSection;
	private Rect keysetSelectionSection;
	private Rect keysetSection;

	private GUISkin skin;

	public Keyset keyset;
	private List<string> dialogueNames;

	private bool isLoaded = false;
	private Vector2 scrollPos;
	private float keysetSelectionHeight = 64;
	private int dialogueNum = 1;
	private string keysetName = "";

	[MenuItem("Window/Keyset Editor")]
	private static void OpenWindow() {
		KeysetEditorWindow window = (KeysetEditorWindow)GetWindow(typeof(KeysetEditorWindow));
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

		keysetSelectionTexture = new Texture2D(1, 1);
		keysetSelectionTexture.SetPixel(0, 0, color);
		keysetSelectionTexture.Apply();

		keysetTexture = new Texture2D(1, 1);
		keysetTexture.SetPixel(0, 0, color);
		keysetTexture.Apply();
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeaderSection();
		DrawKeysetSelectionSection();

		if (isLoaded)
			DrawKeysetSection();
	}

	private void DrawLayouts() {
		headerSection.x = 0;
		headerSection.y = 0;
		headerSection.width = position.width;
		headerSection.height = 80;

		keysetSelectionSection.x = 5;
		keysetSelectionSection.y = headerSection.height;
		keysetSelectionSection.width = position.width - 10;
		keysetSelectionSection.height = keysetSelectionHeight;

		keysetSection.x = 5;
		keysetSection.y = headerSection.height + keysetSelectionSection.height;
		keysetSection.width = position.width - 10;
		keysetSection.height = position.height - headerSection.height - keysetSelectionSection.height - 9;

		GUI.DrawTexture(headerSection, headerTexture);
		GUI.DrawTexture(keysetSelectionSection, keysetSelectionTexture);
		GUI.DrawTexture(keysetSection, keysetTexture);
	}

	private void DrawHeaderSection() {
		GUILayout.BeginArea(headerSection);
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label("Keyset Editor", skin.GetStyle("Header"));

		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawKeysetSelectionSection() {
		bool isSelected;

		GUILayout.BeginArea(keysetSelectionSection);

		GUILayout.BeginHorizontal();

		keyset = (Keyset)EditorGUILayout.ObjectField(keyset, typeof(Keyset), false);

		// if dialog isn't selected, resize section to allow for popup
		if (!isLoaded && keyset == null) {
			keysetSelectionHeight = 64;
			isSelected = false;
		}
		// else, resize section to remove extra space
		else {
			keysetSelectionHeight = 18;
			isSelected = true;

			if (GUILayout.Button("Load", GUILayout.Height(18))) {
				keysetName = keyset.name;
				dialogueNames = new List<string>(keyset.dialogueNames);
				dialogueNum = dialogueNames.Count;

				isLoaded = true;
			}
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		// placed afterward to not be included in same horizontal as field/button
		if (!isSelected)
			EditorGUILayout.HelpBox("A [Keyset] needs to be selected before it can be edited.", MessageType.Warning);


		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void DrawKeysetSection() {
		GUILayout.BeginArea(keysetSection);

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

		GUILayout.Label("Editing '" + keysetName + "'");

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
			int keysetNum = this.keyset.dialogueNames.Count;

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

			if (dialogueNum > keysetNum) {
				if (!EditorUtility.DisplayDialog("Extra Keys Added", "More keys have been added. To ensure that all languages are using the same keys, all languages will have the extra keys added. Do you want to add the extra keys to all languages using this keyset?", "Yes", "No"))
					return;
				else
					EditorUtility.DisplayDialog("Extra Keys Added", "Extra keys will be added to the end of all languages using this keyset. Remember to add text to the new keys", "Ok", "");
			}
			else if (dialogueNum < keysetNum) {
				if (!EditorUtility.DisplayDialog("Some Keys Removed", "Some keys have been removed. To ensure that all languages are using the same keys, all languages will have the the same keys removed. Do you want to remove these keys from all languages using this keyset?", "Yes", "No"))
					return;
				else
					EditorUtility.DisplayDialog("Some Keys Removed", "These keys wiil be removed from all languages using this keyset.", "Ok", "");
			}

			if (dialogueNum != keysetNum) {
				string[] languageGUIDs = AssetDatabase.FindAssets(keysetName + " t:Language", new[] { "Assets/Resources/Languages/" });

				foreach (string languageGUID in languageGUIDs) {
					Language language = (Language)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(languageGUID), typeof(Language));

					if (dialogueNum > keysetNum) {
						foreach (string dialogueName in dialogueNames) {
							if (!language.dialogueNames.Contains(dialogueName)) {
								language.dialogueNames.Add(dialogueName);
								language.textList.Add("");
							}
						}
					}
					else {
						foreach (string dialogueName in language.dialogueNames) {
							if (!dialogueNames.Contains(dialogueName))
								language.textList.RemoveAt(language.dialogueNames.IndexOf(dialogueName));
						}

						language.dialogueNames = new List<string>(dialogueNames);
					}

					EditorUtility.SetDirty(language);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}
			}

			string keysetPath = "Assets/Resources/Keysets/" + keysetName + ".asset";
			Keyset keyset = (Keyset)AssetDatabase.LoadAssetAtPath(keysetPath, typeof(Keyset));
			keyset.dialogueNames = new List<string>(dialogueNames);
			EditorUtility.SetDirty(keyset);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}
