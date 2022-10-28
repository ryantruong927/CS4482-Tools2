using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogCreatorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D keysetSelectionTexture;
	private Texture2D dialogTexture;
	private Color color = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1);

	private Rect headerSection;
	private Rect keysetSelectionSection;
	private Rect dialogSection;

	private GUISkin skin;

	public Keyset keyset;
	private List<string> keys, textList;

	private bool isLoaded = false;
	private Vector2 scrollPos;
	private float keysetSelectionHeight = 64;
	private int dialogNum;
	private string language = "";

	[MenuItem("Window/Dialog Creator")]
	private static void OpenWindow() {
		DialogCreatorWindow window = (DialogCreatorWindow)GetWindow(typeof(DialogCreatorWindow));
		window.minSize = new Vector2(200, 300);
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

		dialogTexture = new Texture2D(1, 1);
		dialogTexture.SetPixel(0, 0, color);
		dialogTexture.Apply();
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeaderSection();
		DrawKeysetSelectionSection();

		if (isLoaded)
			DrawDialogSection();
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

		dialogSection.x = 5;
		dialogSection.y = headerSection.height + keysetSelectionSection.height;
		dialogSection.width = position.width - 10;
		dialogSection.height = position.height - headerSection.height - keysetSelectionSection.height - 5;

		GUI.DrawTexture(headerSection, headerTexture);
		GUI.DrawTexture(keysetSelectionSection, keysetSelectionTexture);
		GUI.DrawTexture(dialogSection, dialogTexture);
	}

	private void DrawHeaderSection() {
		GUILayout.BeginArea(headerSection);
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label("Dialog Creator", skin.GetStyle("Header"));

		EditorGUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawKeysetSelectionSection() {
		bool isSelected;

		GUILayout.BeginArea(keysetSelectionSection);

		GUILayout.BeginHorizontal();

		keyset = (Keyset)EditorGUILayout.ObjectField(keyset, typeof(Keyset), false);

		// if dialog isn't selected, resize section to allow for popup
		if (keyset == null) {
			keysetSelectionHeight = 64;
			isSelected = false;
		}
		// else, resize section to remove extra space
		else {
			keysetSelectionHeight = 18;
			isSelected = true;

			if (GUILayout.Button("Load", GUILayout.Height(18))) {
				keys = keyset.keys;
				dialogNum = keys.Count;

				textList = new List<string>();
				for (int i = 0; i < dialogNum; i++)
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

	private void DrawDialogSection() {
		GUILayout.BeginArea(dialogSection);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Language", skin.GetStyle("Dialog"));
		GUILayout.FlexibleSpace();
		language = EditorGUILayout.TextField(language, GUILayout.Width(position.width - 109));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(4);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Key", skin.GetStyle("Dialog"), GUILayout.Width(100));
		GUILayout.Label("Text", skin.GetStyle("Dialog"));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginVertical();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		for (int i = 0; i < dialogNum; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(keys[i], GUILayout.Width(100));
			string text = EditorGUILayout.TextField(textList[i]);
			textList[i] = text;

			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space(4);

		if (GUILayout.Button("Save", skin.GetStyle("Button"), GUILayout.Width(50), GUILayout.Height(24))) {
			SaveDialogData();
		}

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("+", skin.GetStyle("Button"), GUILayout.Width(24), GUILayout.Height(24))) {
			keys.Add("");
			textList.Add("");
			dialogNum++;
		}

		if (GUILayout.Button("-", skin.GetStyle("Button"), GUILayout.Width(24), GUILayout.Height(24))) {
			if (dialogNum > 1) {
				dialogNum--;
				keys.RemoveAt(dialogNum);
				textList.RemoveAt(dialogNum);
			}
		}

		EditorGUILayout.Space(2);
		EditorGUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void SaveDialogData() {
		if (language == "")
			EditorUtility.DisplayDialog("Missing Name", "Name for dialog required.", "Ok", "");
		else {
			Dictionary<string, string> dialogDict = new Dictionary<string, string>();

			for (int i = 0; i < dialogNum; i++) {
				if (keys[i] == "") {
					if (textList[i] != "") {
						EditorUtility.DisplayDialog("Missing Language", "One of your texts is missing a language.", "Ok", "");
						return;
					}
					else {
						if (dialogNum == 1) {
							EditorUtility.DisplayDialog("", "There is nothing to save.", "Ok", "");
							return;
						}

						keys.RemoveAt(i);
						textList.RemoveAt(i);
						dialogNum--;
					}
				}
				else if (textList[i] == "") {
					if (!EditorUtility.DisplayDialog("Missing Text", "One of your languages is missing some text. Would you like to add text?", "Yes", "No"))
						return;
				}
				else {
					try {
						dialogDict.Add(keys[i], textList[i]);
					}
					catch (ArgumentException e) {
						EditorUtility.DisplayDialog("Duplicate Language", "Two or more of your dialogs have the same language.", "Ok", "");
						return;
					}
				}
			}

			Language dialog = new Language();
		}
	}
}
