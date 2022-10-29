using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueCreatorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D keysetSelectionTexture;
	private Texture2D dialogueTexture;
	private Color color = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1);

	private Rect headerSection;
	private Rect keysetSelectionSection;
	private Rect dialogueSection;

	private GUISkin skin;

	public Keyset keyset;
	private List<string> dialogueNames, textList;

	private bool isLoaded = false;
	private Vector2 scrollPos;
	private float keysetSelectionHeight = 64;
	private int dialogueNum;
	private int saveOption = 0;
	private string keysetName = "";
	private string languageName = "";

	[MenuItem("Window/Dialogue Creator")]
	private static void OpenWindow() {
		DialogueCreatorWindow window = (DialogueCreatorWindow)GetWindow(typeof(DialogueCreatorWindow));
		window.minSize = new Vector2(600, 300);
		window.Show();
	}

	private void OnEnable() {
		InitTextures();
		skin = Resources.Load<GUISkin>("GUISkins/DialogWindowSkin");
	}

	private void InitTextures() {
		headerTexture = new Texture2D(1, 1);
		headerTexture.SetPixel(0, 0, color);
		headerTexture.Apply();

		keysetSelectionTexture = new Texture2D(1, 1);
		keysetSelectionTexture.SetPixel(0, 0, color);
		keysetSelectionTexture.Apply();

		dialogueTexture = new Texture2D(1, 1);
		dialogueTexture.SetPixel(0, 0, color);
		dialogueTexture.Apply();
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeaderSection();
		DrawKeysetSelectionSection();

		if (isLoaded)
			DrawDialogueSection();
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

		dialogueSection.x = 5;
		dialogueSection.y = headerSection.height + keysetSelectionSection.height + 4;
		dialogueSection.width = position.width - 10;
		dialogueSection.height = position.height - headerSection.height - keysetSelectionSection.height - 9;

		GUI.DrawTexture(headerSection, headerTexture);
		GUI.DrawTexture(keysetSelectionSection, keysetSelectionTexture);
		GUI.DrawTexture(dialogueSection, dialogueTexture);
	}

	private void DrawHeaderSection() {
		GUILayout.BeginArea(headerSection);
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label("Dialogue Creator", skin.GetStyle("Header"));

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
				dialogueNames = keyset.dialogueNames;
				dialogueNum = dialogueNames.Count;

				textList = new List<string>();
				for (int i = 0; i < dialogueNum; i++)
					textList.Add("");

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

	private void DrawDialogueSection() {
		GUILayout.BeginArea(dialogueSection);

		EditorGUILayout.Space(4);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Language");
		GUILayout.FlexibleSpace();
		languageName = EditorGUILayout.TextField(languageName, GUILayout.Width(position.width - 109));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(4);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Dialogue Name", GUILayout.Width(180));
		GUILayout.Label("Text");
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginVertical();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		for (int i = 0; i < dialogueNum; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(dialogueNames[i], GUILayout.Width(180));
			string text = EditorGUILayout.TextField(textList[i]);
			textList[i] = text;

			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(24))) {
			SaveLanguage();
		}

		GUILayout.FlexibleSpace();

		GUILayout.Label("Using '" + keysetName + "'");

		EditorGUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void SaveLanguage() {
		if (languageName == "")
			EditorUtility.DisplayDialog("Missing Language", "Language for dialog required.", "Ok", "");
		else {
			for (int i = 0; i < dialogueNum; i++) {
				if (saveOption != 2 && textList[i] == "") {
					saveOption = EditorUtility.DisplayDialogComplex("Missing Text", "Dialog '" + dialogueNames[i] + "' is missing some text. Would you like to add text?", "Yes", "No", "Don't Show Again");

					if (saveOption == 0 || saveOption == 2)
						return;
				}
			}

			string languagePath = "Assets/Resources/Languages/" + keyset.name + languageName + ".asset";
			Language language = (Language)CreateInstance(typeof(Language));
			language.dialogueNames = new List<string>(keyset.dialogueNames);
			language.textList = new List<string>(textList);

			if (File.Exists(languagePath)) {
				EditorUtility.DisplayDialog("Keyset Already Exists", "A keyset with the name '" + keysetName + "' already exists.", "Ok", "");
				return;
			}

			AssetDatabase.CreateAsset(language, languagePath);
		}
	}
}
