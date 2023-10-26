using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Scripting.Python;
using System.IO;
using System.Text.Json;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;
using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Drawing.Printing;

public class TextToModel : EditorWindow
{
    TextField tf;
    Texture2D refImage = null;
    bool firstTime = true;
    //private string inputText;

    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>("TextToModel");
        wnd.maxSize = new Vector2(470f, 750f);
        wnd.minSize = wnd.maxSize;
        var position = wnd.position;

        position.center = new Rect(215f, 215f, Screen.currentResolution.width / 2, Screen.currentResolution.height / 2).center;
        wnd.position = position;

        Debug.Log("Happens second");
    }

    public void CreateGUI()
    {   
        Debug.Log("Happens first");
        Debug.Log(refImage);

        string debug = File.ReadAllText("Assets/Models/RefImageFilePath.json");
        Debug.Log(debug);

        //places placeholder image
        if (firstTime)
        {
            refImage = (Texture2D)EditorGUIUtility.Load("Assets/ImagesFolder/placeholder.png");
            firstTime = false;
        }

        Debug.Log(refImage);

        //Title and main container
        VisualElement container = new VisualElement();
        rootVisualElement.Add(container);

        //Loading style sheet
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("TextToModelStyle.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        //some text
        Label title = new Label("Insert your prompt into the text field below");
        //title.AddToClassList("label-title");
        container.Add(title);

        //button and input section
        VisualElement buttonContainer = new VisualElement();

        //buttons and inputs created
        Button makeModel = new Button() { text = "Create Model" };
        //Button closeWindow = new Button() { text = "Create Model Faster" };
        TextField inputField = new TextField();
        tf = inputField;

        //adds styling to the buttons
        makeModel.AddToClassList("dark-button");
        //closeWindow.AddToClassList("dark-button");
        inputField.AddToClassList("text-field");

        //events for buttons clicked
        makeModel.clicked += CreatePrompt;
        //closeWindow.clicked += TestFunction;

        //adding to button container
        buttonContainer.Add(inputField);
        buttonContainer.Add(makeModel);
        //buttonContainer.Add(closeWindow);

        //adding button container to container
        container.Add(buttonContainer);

        //image section
        VisualElement imageContainer = new VisualElement();
        
        //creates reference image
        Image imageHold = new Image();
        imageHold.image = refImage;
        
        //adds styling to image
        imageHold.AddToClassList("resize-image");

        //adds image to container
        imageContainer.Add(imageHold);

        //adds image container to container
        container.Add(imageHold);
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

        //Writes and saves
        File.WriteAllText(Application.dataPath + "/Editor/LastPrompt.json", strPrompt);

        //Runs the python script
        PythonRunner.RunFile("Assets/PythonScripts/CreateModel.py");

        Debug.Log("End?");

        PlaceAsset();
    }

    public void PlaceAsset()
    {
        AssetDatabase.Refresh();

        //reads the json file and puts text into string
        string text = File.ReadAllText("Assets/Models/AssetFilePath.json");

        //creates a assetfilepath class and overwrites the class with a new class from the json file
        AssetFilePath filepath = new AssetFilePath();
        JsonUtility.FromJsonOverwrite(text, filepath);

        Debug.Log(filepath.textFilePath);

        //loads the mesh
        var test = (GameObject)EditorGUIUtility.Load(filepath.textFilePath);

        Debug.Log(test);

        //scaling stuff
        test.transform.localScale = new Vector3(4, 4, 4);
        
        //instantiates the object
        PrefabUtility.InstantiatePrefab(test);

        //updates image
        UpdateImage();

        Debug.Log("Success");
    }

    public void TestFunction()
    {
        PythonRunner.RunFile("Assets/PythonScripts/NewTestScript.py");
        string text = File.ReadAllText("Assets/Test/AssetFilePath.json");
        Debug.Log(text);


        AssetFilePath filePath = new AssetFilePath();
        JsonUtility.FromJsonOverwrite(text, filePath);
        
        Debug.Log(filePath.textFilePath);

        var test = (GameObject)EditorGUIUtility.Load(filePath.textFilePath);

        Debug.Log(test);
        test.transform.localScale = new Vector3(4, 4, 4);

        PrefabUtility.InstantiatePrefab(test);

        //Debug.Log("Wprl?");
    }

    public void UpdateImage()
    {
        //looks for image and sets it in editor
        if (File.Exists("Assets/Models/RefImageFilePath.json"))
        {
            string text = File.ReadAllText("Assets/Models/RefImageFilePath.json");
            AssetFilePath filepath = new AssetFilePath();
            JsonUtility.FromJsonOverwrite(text, filepath);
            refImage = (Texture2D)EditorGUIUtility.Load("Assets/ImagesFolder/YCNFys4TChQAclNfolUqKQ.pg");
        }

        CreateGUI();
    }
}