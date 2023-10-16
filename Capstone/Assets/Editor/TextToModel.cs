using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Scripting.Python;
using System.IO;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class TextToModel : EditorWindow
{
    TextField tf;
    private string inputText;

    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>("TextToModel");
        //wnd.titleContent = new GUIContent("TextToModel");
        wnd.maxSize = new Vector2(530f, 530f);
        wnd.minSize = wnd.maxSize;
        var position = wnd.position;

        position.center = new Rect(215f, 215f, Screen.currentResolution.width / 2, Screen.currentResolution.height / 2).center;
        wnd.position = position;
    }

    public void CreateGUI()
    {
        //Title and main container
        VisualElement container = new VisualElement();
        rootVisualElement.Add(container);

        //Loading style sheet
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("TextToModelStyle.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        //some text
        Label title = new Label("Insert your prompt below");
        container.Add(title);

        //button and input section
        VisualElement buttonContainer = new VisualElement();

        //buttons and inputs created
        Button makeModel = new Button() { text = "Create Model" };
        Button closeWindow = new Button() { text = "Close" };
        TextField inputField = new TextField();
        tf = inputField;

        closeWindow.AddToClassList("dark-button");

        //events for buttons clicked
        makeModel.clicked += CreatePrompt;
        closeWindow.clicked += CloseWindow;

        //adding to button container
        buttonContainer.Add(inputField);
        buttonContainer.Add(makeModel);
        buttonContainer.Add(closeWindow);

        //adding button container to container
        container.Add(buttonContainer);

        //Debug
        /*Debug.Log(container.panel);
        Debug.Log(rootVisualElement.panel);*/
    }

    public void OnGUI()
    {
        /*GUILayout.Space(20);
        inputText = EditorGUILayout.TextField("", inputText);*/

        /*if (GUILayout.Button("Create Model")) CreatePrompt(inputText);
        if (GUILayout.Button("Close")) Close();*/
    }

    public void CreatePrompt()
    {
        //Makes prompt class
        PromptClass p = new PromptClass(tf.value);

        //Takes the prompt class and turns it into json
        string strPrompt = JsonUtility.ToJson(p);

        Debug.Log("Test");

        //Writes and saves
        File.WriteAllText(Application.dataPath + "/Editor/LastPrompt.json", strPrompt);

        //Runs the python script
        PythonRunner.RunFile("Assets/PythonScripts/CreateModel.py");

        Debug.Log("End?");
    }

    public void CloseWindow()
    {
        //Debug.Log(tf.value);
        Close();
    }
}