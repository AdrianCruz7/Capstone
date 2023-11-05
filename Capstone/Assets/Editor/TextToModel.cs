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
using Unity.EditorCoroutines.Editor;
using System.Collections;
using NUnit.Framework.Internal;
using Codice.Client.Common;

public class TextToModel : EditorWindow
{
    /*TextField tf;
    Texture2D refImage;
    bool firstTime = true;
    Image imageHold;
    VisualElement imageContainer;*/

    Transform testGO;
    bool multipleMeshes = false;
    int meshInstances = 1;
    float scale = 1.0f;
    float radius = 5.0f;
    string prompt;
    Texture2D image;
    bool firstTime = true;

    VisualElement container;

    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>("TextToModel");
        wnd.maxSize = new Vector2(470f, 1000f);
        wnd.minSize = wnd.maxSize;
        var position = wnd.position;

        position.center = new Rect(215f, 215f, Screen.currentResolution.width / 2, Screen.currentResolution.height / 2).center;
        //wnd.position = position;

        Debug.Log("Happens second");
    }

    private void OnEnable()
    {
        //Title and main container
        container = new VisualElement();
        rootVisualElement.Add(container);

        //Loading style sheet
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("TextToModelStyle.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
    }

    private void OnGUI()
    {
        //can't go below 1 for mesh instances field
        if (meshInstances < 1)
        {
            meshInstances = 1;
        }

        //placeholder
        if(firstTime)
        {
            image = EditorGUIUtility.Load("Assets/ImagesFolder/placeholder.png") as Texture2D;
            firstTime = false;
        }

        //styling
        GUIStyle labelStyle;
        GUIStyle imageStyle;
        Styling(out labelStyle, out imageStyle);

        //layout for all gui elements
        EditorGUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 220;

        //label
        GUILayout.Label("Insert your prompt into the field below", labelStyle);

        //prompt field for user input
        prompt = GUILayout.TextField(prompt);

        //create object button
        if (GUILayout.Button("Create Object"))
        {
            CreatePrompt();
            //MeshyCreatePrompt();
            //UpdateImage();
            //PlaceAsset();
        }

        //reference image
        GUILayout.Box(image, imageStyle);

        //settings to place the mesh in the world
        //label
        GUILayout.Label("Object Spawning Settings", EditorStyles.boldLabel);

        //transform to place the object in the world
        testGO = EditorGUILayout.ObjectField("Object Transform", testGO, typeof(Transform), true) as Transform;

        //scale for object
        scale = EditorGUILayout.FloatField("Object Scale", scale);

        //toggle if user wants to make multiple meshes
        multipleMeshes = EditorGUILayout.Toggle("Multiple Objects", multipleMeshes);
        if (multipleMeshes)
        {
            //number of meshes
            meshInstances = EditorGUILayout.IntField("Number of objects to create", meshInstances);
            
            //radius where the meshes will be spawned in
            radius = EditorGUILayout.FloatField("Spawning Radius", radius);
        }

        GUILayout.Label("");

        if(GUILayout.Button("Other Settings"))
        {
            WindowSettings();
        }
        EditorGUILayout.EndVertical();
    }

    //test function
    public void MeshyCreatePrompt()
    {
        //Makes prompt class
        PromptClass p = new PromptClass(prompt);

        //Takes the prompt class and turns it into json
        string strPrompt = JsonUtility.ToJson(p);

        //Writes and saves
        File.WriteAllText(Application.dataPath + "/Editor/LastPrompt.json", strPrompt);

        //Runs the python script
        PythonRunner.RunFile("Assets/PythonScripts/NewImageToModel.py");

        Debug.Log("End?");

        //PlaceAsset();
    }

    public void CreatePrompt()
    {
        //Makes prompt class
        PromptClass p = new PromptClass(prompt);

        //Takes the prompt class and turns it into json
        string strPrompt = JsonUtility.ToJson(p);

        //Writes and saves
        File.WriteAllText(Application.dataPath + "/Editor/LastPrompt.json", strPrompt);

        //Runs the python script
        PythonRunner.RunFile("Assets/PythonScripts/CreateModel.py");

        Debug.Log(strPrompt);
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

        //Debug
        //Debug.Log(filepath.textFilePath);

        //loads the mesh
        var test = (GameObject)EditorGUIUtility.Load(filepath.textFilePath);

        //Debug
        //Debug.Log(test);

        if(multipleMeshes)
        {
            for(int i = 0; i <= meshInstances; i++)
            {
                Vector2 spawnField = UnityEngine.Random.insideUnitCircle * radius;
                Vector3 position = new Vector3(spawnField.x, 0f, spawnField.y);
                position.x += testGO.position.x;
                position.y += testGO.position.y;
                position.z += testGO.position.z;

                var anInstance = Instantiate(test);
                anInstance.transform.position = position;
                anInstance.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        else
        {
            //instantiates the object in the world
            var instance = Instantiate(test);

            //checks if GO transform is null
            if (testGO != null)
            {
                //sets the object transform where the gameobject is located
                instance.transform.position = testGO.transform.position;
            }

            //scales up the object transform to scale value
            instance.transform.localScale = new Vector3(scale, scale, scale);
        }
        

        //updates image
        UpdateImage();

        Debug.Log("Asset placement success");
    }

    public void UpdateImage()
    {
        //looks for image and sets it in editor
        if (File.Exists("Assets/Models/RefImageFilePath.json"))
        {
            string text = File.ReadAllText("Assets/Models/RefImageFilePath.json");
            AssetFilePath filepath = new AssetFilePath();
            JsonUtility.FromJsonOverwrite(text, filepath);
            image = (Texture2D)EditorGUIUtility.Load(filepath.textFilePath);
            //imageHold.image = refImage;
        }

        Debug.Log("UpdateImage done");
    }

    public void Confirmation()
    {
        bool confirmation = EditorUtility.DisplayDialog("Confirmation", "Model generation can take several minutes and might even fail depending on the prompt given. Do you wish to proceed?", "Yes", "No");

        if (confirmation)
        {
            Debug.Log("Confirmation is a yes");
            //CreatePrompt();
        }
        else
        {
            Debug.Log("Confirmation is a no");
            //this.Close();
        }
    }

    //change into api key window for later
    public void WindowSettings()
    {
        var windowType = typeof(TextToModel);
        TTMSettings window = GetWindow<TTMSettings>(windowType);
        window.Show();
    }

    public void Styling(out GUIStyle labelStyle, out GUIStyle imageStyle)
    {
        labelStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        labelStyle.normal.textColor = Color.white;
        labelStyle.padding.top = 10;
        labelStyle.padding.bottom = 10;

        imageStyle = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 463
        };
    }

    //Test and Debug functions from this point
    public void TestFunction1()
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

    public void TestFunction2()
    {
        //this.StartCoroutine(Confirm());
    }

    

    /*public void CreateGUI()
    {
        Debug.Log("Happens first");
        Debug.Log(refImage + " is the reference image and firstTime is set to: " + firstTime);

        string debug = File.ReadAllText("Assets/Models/RefImageFilePath.json");
        Debug.Log(debug);

        //places placeholder image
        if (firstTime)
        {
            refImage = (Texture2D)EditorGUIUtility.Load("Assets/ImagesFolder/placeholder.png");
            firstTime = false;
        }

        //Debug.Log(refImage + " is now the reference image and firstTime is set to: " + firstTime);

        //some text
        Label title = new Label("Insert your prompt into the text field below");
        //title.AddToClassList("label-title");
        container.Add(title);

        //button and input section
        VisualElement buttonContainer = new VisualElement();

        //buttons and inputs created
        Button makeModel = new Button() { text = "Create Model" };
        Button closeWindow = new Button() { text = "Create Model Faster" };
        Button openWindow = new Button() { text = "Creation Settings" };
        TextField inputField = new TextField();
        tf = inputField;

        //adds styling to the buttons
        makeModel.AddToClassList("dark-button");
        closeWindow.AddToClassList("dark-button");
        inputField.AddToClassList("text-field");

        //events for buttons clicked
        makeModel.clicked += Confirmation;
        closeWindow.clicked += UpdateImage;
        openWindow.clicked += WindowSettings;

        //adding to button container
        buttonContainer.Add(inputField);
        buttonContainer.Add(makeModel);
        buttonContainer.Add(closeWindow);
        buttonContainer.Add(openWindow);

        //adding button container to container
        container.Add(buttonContainer);

        //image section
        imageContainer = new VisualElement();
        imageHold = new Image();

        //creates reference image
        imageHold.image = refImage;

        //adds styling to image
        imageHold.AddToClassList("resize-image");

        //adds image to container
        imageContainer.Add(imageHold);

        //adds image container to container
        container.Add(imageContainer);
    }*/
}