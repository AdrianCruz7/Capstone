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
using GLTFast.Schema;
using Toggle = UnityEngine.UIElements.Toggle;

public class TextToModel : EditorWindow
{
    /*TextField tf;
    Texture2D refImage;
    bool firstTime = true;
    Image imageHold;
    VisualElement imageContainer;*/

    Transform testGO;
    Label labelImage;

    bool multipleMeshes = false;
    Foldout uiMultipleMeshes;
    
    int meshInstances = 1;
    IntegerField uiMeshInstances;
    
    float scale = 1.0f;
    FloatField uiScale;
    
    float radius = 1.0f;
    FloatField uiRadius;

    string prompt;
    
    Texture2D image;

    //visual elements
    VisualTreeAsset uiAsset;
    VisualElement ui;

    //used for functions of buttons
    Action Action1;
    Action Action2;

    bool testToggle = false;
    Toggle uiTestToggle;


    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>("TextToModel");

        Debug.Log("Happens second");
    }

    private void OnEnable()
    {
        Action1 += Confirmation;
        Action2 += uitestfunction1;
        SetUpUI();
    }

    private void OnGUI()
    {
        uiScale.RegisterValueChangedCallback(evt => { uiScale.value = Mathf.Clamp(uiScale.value, 1, 100); });
        uiMeshInstances.RegisterValueChangedCallback(evt => { uiMeshInstances.value = Mathf.Clamp(uiMeshInstances.value, 1, 100); });
        uiRadius.RegisterValueChangedCallback(evt => { uiRadius.value =  Mathf.Clamp(uiRadius.value, 1, 100); });
    }

    public void uitestfunction1()
    {
        testToggle = uiTestToggle.value;

        if(testToggle)
        {
            var a = ui.Q<VisualElement>("grow0element");
            a.SetEnabled(true);
        }
        else
        {
            var b = ui.Q<VisualElement>("grow0element");
            b.SetEnabled(false);
        }
        /*if(testToggle)
        {
            VisualElement ui = new VisualElement();
            ui.name = "labelcontainer";
            ui.Add(new Label("sef"));
            ui.Add(new Label("aefef"));
            ui.Add(new Label("saaf"));
            uiTestToggle.Add(ui);
        }
        else
        {
            var removal = uiTestToggle.Q<VisualElement>("labelcontainer");
            removal.SetEnabled(false);
            uiTestToggle.Remove(removal);
        }*/
    }

    public void SetUpUI()
    {
        uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UXMLTest/trial.uxml");
        ui = uiAsset.Instantiate();
        rootVisualElement.Add(ui);        

        labelImage = ui.Q<Label>("labelImage");
        labelImage.style.backgroundImage = EditorGUIUtility.Load("Assets/ImagesFolder/placeholder.png") as Texture2D;

        uiScale = ui.Q<FloatField>("scaleField");
        uiMeshInstances = ui.Q<IntegerField>("meshNumbers");
        uiMultipleMeshes = ui.Q<Foldout>("multipleFoldout");
        uiRadius = ui.Q<FloatField>("radiusField");

        uiTestToggle = ui.Q<Toggle>("testToggle");
        uiTestToggle.RegisterCallback<MouseUpEvent>((evt) => Action2());


        var buttontest = ui.Q<Button>("promptButton");
        buttontest.RegisterCallback<MouseUpEvent>((evt) => Action1());
    }

    void AssignValues()
    {
        prompt = ui.Q<TextField>("promptField").value;
        //Debug.Log(field.text);   

        testGO = (Transform)ui.Q<ObjectField>("transformField").value;
        Debug.Log(testGO);

        scale = uiScale.value;

        multipleMeshes = uiMultipleMeshes.value;
        if(multipleMeshes == true)
        {
            meshInstances = uiMeshInstances.value;
            radius = uiRadius.value;
        }

        MeshyCreatePrompt();
    }

    /*private void OnEnable()
    {
        //Title and main container
        container = new VisualElement();
        rootVisualElement.Add(container);

        //Loading style sheet
        StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("TextToModelStyle.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
    }*/

    /*private void OnGUI()
    {
        //can't go below 1 for mesh instances field
        if (meshInstances < 1)
        {
            meshInstances = 1;
        }

        //placeholder
        if (firstTime)
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
            //CreatePrompt();
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

        if (GUILayout.Button("Other Settings"))
        {
            WindowSettings();
        }
        EditorGUILayout.EndVertical();
    }*/

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

        Debug.Log(meshInstances);

        PlaceAsset();
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

        Debug.Log(meshInstances);

        //loads the mesh
        var test = (GameObject)EditorGUIUtility.Load(filepath.textFilePath);

        //if GO transform is null it makes on by default
        if (testGO == null)
        {
            GameObject go = new GameObject();
            testGO = go.transform;
        }    

        if(meshInstances == 1)
        {
            //instantiates the object in the world
            var instance = Instantiate(test);
            instance.transform.localScale = new Vector3(scale, 10, scale);

            if (testGO != null)
            {
                //sets the object transform where the gameobject is located
                instance.transform.position = testGO.transform.position;
            }
        }
        else
        {
            if (multipleMeshes)
            {
                for (int i = 0; i < meshInstances; i++)
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
            labelImage.style.backgroundImage = image;
        }

        Debug.Log("UpdateImage done");
    }

    public void Confirmation()
    {
        bool confirmation = EditorUtility.DisplayDialog("Confirmation", "Model generation can take several minutes and can even fail depending on the prompt given. Do you wish to proceed?", "Yes", "No");

        if (confirmation)
        {
            Debug.Log("Confirmation is a yes");
            AssignValues();
        }
        else
        {
            Debug.Log("Confirmation is a no");
        }
    }

    public void WindowSettings()
    {
        var windowType = typeof(TextToModel);
        TTMSettings window = GetWindow<TTMSettings>(windowType);
        window.Show();
    }

    //Test and Debug functions from this point
    /*public void TestFunction1()
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
    }*/



    /*private void CreateGUI()
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