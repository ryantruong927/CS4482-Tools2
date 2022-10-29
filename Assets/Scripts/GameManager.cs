using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Keyset keyset;
    public TextMeshProUGUI[] textObjects;

    public TMP_Dropdown languageDropdown;

    public string languagePath = "Assets/Resources/Languages/";
	public static string languageName = "English";

    private void Start() {
        DirectoryInfo languageDir = new DirectoryInfo(languagePath);
		FileInfo[] languageInfo = languageDir.GetFiles(keyset.name + "*.asset");
        List<string> languageOptions = new List<string>();

        foreach (FileInfo languageFile in languageInfo) {
            string languageFilename = languageFile.Name;
			languageFilename = languageFilename.Replace(keyset.name, "");
			languageFilename = languageFilename.Replace(".asset", "");

			if (languageFilename != "English")
                languageOptions.Add(languageFilename);
		}

		languageDropdown.AddOptions(languageOptions);

		languageDropdown.onValueChanged.AddListener(delegate { SetLanguageName(languageDropdown.options[languageDropdown.value].text); });

        ChangeLanguage();
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
        ChangeLanguage();
    }

    public void SetLanguageName(string languageName) {
        GameManager.languageName = languageName;
    }

    public void ChangeLanguage() {
        Language language = (Language)AssetDatabase.LoadAssetAtPath(languagePath + keyset.name + languageName + ".asset", typeof(Language));

        List<string> dialogueNames = language.dialogueNames;
        List<string> textList = language.textList;
        int count = dialogueNames.Count;

		for (int i = 0; i < count; i++) {
            if (textObjects[i].name.Equals(dialogueNames[i]))
                textObjects[i].text = textList[i];
        }
	}
}
