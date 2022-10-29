using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueEditorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D languageSelectionTexture;
	private Texture2D dialogueTexture;
	private Color color = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1);

	private Rect headerSection;
	private Rect languageSelectionSection;
	private Rect dialogueSection;

	private GUISkin skin;

	public Language language;
	private List<string> dialogueNames, textList;

	private bool isLoaded = false;
	private Vector2 scrollPos;
	private float languageSelectionHeight = 64;
	private int dialogueNum;
	private int saveOption = 0;
	private string languageName = "";

	[MenuItem("Window/Dialogue Editor")]
	private static void OpenWindow() {
		DialogueEditorWindow window = (DialogueEditorWindow)GetWindow(typeof(DialogueEditorWindow));
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

		languageSelectionTexture = new Texture2D(1, 1);
		languageSelectionTexture.SetPixel(0, 0, color);
		languageSelectionTexture.Apply();

		dialogueTexture = new Texture2D(1, 1);
		dialogueTexture.SetPixel(0, 0, color);
		dialogueTexture.Apply();
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeaderSection();
		DrawLanguageSelectionSection();

		if (isLoaded)
			DrawDialogSection();
	}

	private void DrawLayouts() {
		headerSection.x = 0;
		headerSection.y = 0;
		headerSection.width = position.width;
		headerSection.height = 80;

		languageSelectionSection.x = 5;
		languageSelectionSection.y = headerSection.height;
		languageSelectionSection.width = position.width - 10;
		languageSelectionSection.height = languageSelectionHeight;

		dialogueSection.x = 5;
		dialogueSection.y = headerSection.height + languageSelectionSection.height + 4;
		dialogueSection.width = position.width - 10;
		dialogueSection.height = position.height - headerSection.height - languageSelectionSection.height - 9;

		GUI.DrawTexture(headerSection, headerTexture);
		GUI.DrawTexture(languageSelectionSection, languageSelectionTexture);
		GUI.DrawTexture(dialogueSection, dialogueTexture);
	}

	private void DrawHeaderSection() {
		GUILayout.BeginArea(headerSection);
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label("Dialogue Editor", skin.GetStyle("Header"));

		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawLanguageSelectionSection() {
		bool isSelected;

		GUILayout.BeginArea(languageSelectionSection);

		GUILayout.BeginHorizontal();

		language = (Language)EditorGUILayout.ObjectField(language, typeof(Language), false);

		// if dialog isn't selected, resize section to allow for popup
		if (!isLoaded && language == null) {
			languageSelectionHeight = 64;
			isSelected = false;
		}
		// else, resize section to remove extra space
		else {
			languageSelectionHeight = 18;
			isSelected = true;

			if (GUILayout.Button("Load", GUILayout.Height(18))) {
				languageName = language.name;
				dialogueNames = new List<string>(language.dialogueNames);
				textList = new List<string>(language.textList);
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

	private void DrawDialogSection() {
		GUILayout.BeginArea(dialogueSection);

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

		GUILayout.Label("Editing '" + languageName + "'");

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
			string languagePath = AssetDatabase.GetAssetPath(this.language);
			Language language = (Language)AssetDatabase.LoadAssetAtPath(languagePath, typeof(Language));
			language.textList = new List<string>(textList);

			EditorUtility.SetDirty(language);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}
