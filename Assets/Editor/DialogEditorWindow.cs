using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogEditorWindow : EditorWindow {
	private Texture2D headerTexture;
	private Texture2D prefabSelectionTexture;
	private Texture2D dialogTexture;

	private Rect headerSection;
	private Rect prefabSelectionSection;
	private Rect dialogSection;

	[MenuItem("Window/Dialog Editor")]
	private static void OpenWindow() {
		DialogEditorWindow window = (DialogEditorWindow)GetWindow(typeof(DialogEditorWindow));
		window.minSize = new Vector2(300, 600);
		window.Show();
	}

	private void OnEnable() {
		InitTextures();
	}

	private void InitTextures() {
		headerTexture = new Texture2D(1, 1);
		prefabSelectionTexture = new Texture2D(1, 1);
		dialogTexture = new Texture2D(1, 1);
	}

	private void OnGUI() {
		DrawLayouts();
		DrawHeader();
		DrawPrefabSelction();
		DrawDialog();
	}

	private void DrawLayouts() {
		headerSection.x = 0;
		headerSection.y = 0;
		headerSection.width = position.width;
		headerSection.height = 60;

		prefabSelectionSection.x = 0;
		prefabSelectionSection.y = 60;
		prefabSelectionSection.width = position.width;
		prefabSelectionSection.height = 0;

		dialogSection.x = 0;
		dialogSection.y = 90;
	}

	private void DrawHeader() {
		GUILayout.BeginArea(headerSection);

		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
	}

	private void DrawPrefabSelction() {
		GUILayout.BeginArea(prefabSelectionSection);

		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
	}

	private void DrawDialog() {
		GUILayout.BeginArea(dialogSection);

		GUILayout.BeginHorizontal();
		GUILayout.EndHorizontal();
	}

	private void SaveDialogData() {

	}
}
