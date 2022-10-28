using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogEditorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D dialogSelectionTexture;
	private Texture2D dialogTexture;
	private Color color = new Color(56f / 255f, 56f / 255f, 56f / 255f, 1);

	private Rect headerSection;
	private Rect dialogSelectionSection;
	private Rect dialogSection;

	private GUISkin skin;

	private GameObject dialog;
	private Language dialogData, editedDialogData;

	private float dialogSelectionHeight = 64;

	[MenuItem("Window/Dialog Editor")]
	private static void OpenWindow() {
		DialogEditorWindow window = (DialogEditorWindow)GetWindow(typeof(DialogEditorWindow));
		window.minSize = new Vector2(300, 600);
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

		dialogSelectionTexture = new Texture2D(1, 1);
		dialogSelectionTexture.SetPixel(0, 0, color);
		dialogSelectionTexture.Apply();

		dialogTexture = new Texture2D(1, 1);
		dialogTexture.SetPixel(0, 0, color);
		dialogTexture.Apply();
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeader();
		DrawDialogSelction();
		DrawDialog();
	}

	private void DrawLayouts() {
		headerSection.x = 0;
		headerSection.y = 0;
		headerSection.width = position.width;
		headerSection.height = 100;

		dialogSelectionSection.x = 0;
		dialogSelectionSection.y = headerSection.height;
		dialogSelectionSection.width = position.width;
		dialogSelectionSection.height = dialogSelectionHeight;

		dialogSection.x = 0;
		dialogSection.y = dialogSelectionSection.height;
		dialogSection.width = position.width;
		dialogSection.height = 100;

		GUI.DrawTexture(headerSection, headerTexture);
		GUI.DrawTexture(dialogSelectionSection, dialogSelectionTexture);
		GUI.DrawTexture(dialogSection, dialogTexture);
	}

	private void DrawHeader() {
		GUILayout.BeginArea(headerSection);
		GUILayout.BeginHorizontal();

		GUILayout.Label("Dialog Editor", skin.GetStyle("Header"));

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void DrawDialogSelction() {
		bool isSelected;

		GUILayout.BeginArea(dialogSelectionSection);

		GUILayout.BeginHorizontal();
		GUILayout.Space(5);

		dialog = (GameObject)EditorGUILayout.ObjectField(dialog, typeof(GameObject), false);

		// if dialog isn't selected, resize section to allow for popup
		if (dialog == null) {
			dialogSelectionHeight = 64;
			isSelected = false;
		}
		// else, resize section to remove extra space
		else {
			dialogSelectionHeight = 20;
			isSelected = true;

			if (GUILayout.Button("Load", GUILayout.Height(18))) {

			}
		}

		GUILayout.Space(5);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Space(5);

		// placed afterward to not be included in same horizontal as field/button
		if (!isSelected)
			EditorGUILayout.HelpBox("A [Language] needs to be selected before it can be edited.", MessageType.Warning);


		GUILayout.Space(5);
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	private void DrawDialog() {
		GUILayout.BeginArea(dialogSection);

		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void SaveDialogData() {

	}
}
