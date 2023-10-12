using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

public class TextToModel : EditorWindow
{
    private string inputText;

    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>();
        wnd.titleContent = new GUIContent("TextToModel");
    }

    public void CreateGUI()
    {
        VisualElement container = new VisualElement();
        rootVisualElement.Add(container);

        Label title = new Label("Insert your prompt below");
        container.Add(title);

        Debug.Log(container.panel);
        Debug.Log(rootVisualElement.panel);
    }

    public void OnGUI()
    {
        GUILayout.Space(20);
        inputText = EditorGUILayout.TextField("", inputText);

        if (GUILayout.Button("Text To 3D Asset")) Close();
        if (GUILayout.Button("Print Text")) CreatePrompt(inputText);
    }

    public void CreatePrompt(string text)
    {
        //Makes prompt class
        PromptClass p = new PromptClass(text);

        //Takes the prompt class and turns it into json
        string strPrompt = JsonUtility.ToJson(p);

        Debug.Log("Test");

        //Writes and saves
        File.WriteAllText(Application.dataPath + "/LastPrompt.txt", strPrompt);

        Debug.Log("End?");
    }
}