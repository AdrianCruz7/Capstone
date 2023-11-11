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

    //choice
    DropdownField uiChoices;
    string userChoice = "";

    //object transformation settings
    Foldout uiSingleSettings;

    Toggle uiOptionalObj;
    bool optionalObj;

    ObjectField uiTestGO;
    Transform testGO;

    //float defaultScale = 1.0f;
    Vector3Field uiScale;
    Vector3Field uiPosition;
    Vector3Field uiRotation;

    //this might be replaced
    bool multipleMeshes = false;
    Foldout uiMultipleMeshes;

    //multiple mesh generation settings
    Foldout uiMultipleSettings;

    DropdownField uiPlacementMethod;
    string placement;

    TextField uiTagField;
    string tag;

    IntegerField uiMeshInstances;
    int meshInstances = 1;
    
    //options for spawnpoint
    Toggle uiToggleSpawnPoint;
    bool toggleSpawnPoint;

    ObjectField uiSpawnPointGO;
    Transform spawnPointGO;

    Vector3Field uiSpawnPosition;
    Vector3 spawnPosition;
    
    FloatField uiRadius;
    float radius = 1.0f;

    //miscellaneous
    string prompt;
    Label labelImage;
    Texture2D image;

    int operation = 0;
    //1 = Single generation with an object used for placement
    //2 = Single generation with user manually inputing the transform
    //3 = Multiple generation using tags from objects in scene
    //4 = Mulitple generation
    //5 = Multiple generation

    //visual elements
    VisualTreeAsset uiAsset;
    VisualElement ui;

    //used for functions of buttons
    Action Settings;
    Action Action1;
    Action Action2;
    Action Action3;
    Action Action4;
    Action Action5;
    Action Action6;

    //debugging
    bool testToggle = false;
    Toggle uiTestToggle;


    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>("TextToModel");
        //Debug.Log("Happens second");
    }

    private void OnEnable()
    {
        Settings += WindowSettings;
        Action1 += Confirmation;
        Action2 += uitestfunction1;
        Action3 += UIObjectInSceneDisplay;
        Action4 += UIOptionsDisplay;
        Action5 += UIPlacementMethodDisplay;
        Action6 += UIToggleGOSpawnPoint;
        SetUpUI();
        uitestfunction1();
        UIObjectInSceneDisplay();
    }

    private void OnGUI()
    {
        uiChoices.RegisterValueChangedCallback(evt => Action4());
        uiPlacementMethod.RegisterValueChangedCallback(evt => Action5());
        uiToggleSpawnPoint.RegisterValueChangedCallback(evt => Action6());
        uiMeshInstances.RegisterValueChangedCallback(evt => { uiMeshInstances.value = Mathf.Clamp(uiMeshInstances.value, 1, 100); });
        uiTagField.RegisterValueChangedCallback(evt => { tag =  uiTagField.value; });
        uiRadius.RegisterValueChangedCallback(evt => { uiRadius.value =  Mathf.Clamp(uiRadius.value, 1, 100); });
    }

    //displays based on choice of single or multiple generation
    public void UIOptionsDisplay()
    {
        switch (uiChoices.value)
        {
            case "Single Object":
                Debug.Log("Single");
                uiSingleSettings.style.display = StyleKeyword.Auto;
                uiMultipleSettings.style.display = StyleKeyword.None;
                userChoice = "Single";
                break;

            case "Multiple Objects":
                Debug.Log("Multiple");
                uiSingleSettings.style.display = StyleKeyword.None;
                uiMultipleSettings.style.display = StyleKeyword.Auto;
                userChoice = "Multiple";
                break;

            default:
                Debug.Log("default");
                break;
        }
    }

    //toggle for using objects in scene for single generation
    public void UIObjectInSceneDisplay()
    {
        optionalObj = uiOptionalObj.value;

        if(optionalObj)
        {
            uiTestGO.style.display = StyleKeyword.Auto;
            uiPosition.style.display = StyleKeyword.None;
            uiRotation.style.display = StyleKeyword.None;
            uiScale.style.display = StyleKeyword.None;
        }
        else
        {
            uiTestGO.style.display = StyleKeyword.None;
            uiPosition.style.display = StyleKeyword.Auto;
            uiRotation.style.display = StyleKeyword.Auto;
            uiScale.style.display = StyleKeyword.Auto;
        }
    }

    public void UIToggleGOSpawnPoint()
    {
        toggleSpawnPoint = uiToggleSpawnPoint.value;

        if(toggleSpawnPoint)
        {
            uiSpawnPointGO.style.display = StyleKeyword.Auto;
            uiSpawnPosition.style.display = StyleKeyword.None;
        }
        else
        {
            uiSpawnPointGO.style.display = StyleKeyword.None;
            uiSpawnPosition.style.display = StyleKeyword.Auto;
        }
    }

    public void UIPlacementMethodDisplay()
    {
        switch (uiPlacementMethod.value)
        {
            case "Object Tag":
                uiTagField.style.display = StyleKeyword.Auto;
                uiMeshInstances.style.display = StyleKeyword.None;
                uiToggleSpawnPoint.style.display = StyleKeyword.None;
                uiSpawnPointGO.style.display = StyleKeyword.None;
                uiSpawnPosition.style.display = StyleKeyword.None;
                uiRadius.style.display = StyleKeyword.None;
                placement = uiPlacementMethod.value;
                break;

            case "Spawn Radius":
                uiTagField.style.display = StyleKeyword.None;
                uiMeshInstances.style.display = StyleKeyword.Auto;
                uiToggleSpawnPoint.style.display = StyleKeyword.Auto;
                UIToggleGOSpawnPoint();
                uiRadius.style.display = StyleKeyword.Auto;
                placement = uiPlacementMethod.value;
                break;

            default:
                Debug.Log("somethings wrong");
                break;
        }

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
    }

    //UI Setup
    //set up initial display as none "tag" in css later
    public void SetUpUI()
    {
        uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UXMLTest/trial.uxml");
        ui = uiAsset.Instantiate();
        rootVisualElement.Add(ui);

        var settingsBtn = ui.Q<Button>("settings");
        settingsBtn.RegisterCallback<MouseUpEvent>(evt => Settings()); 

        labelImage = ui.Q<Label>("labelImage");
        labelImage.style.backgroundImage = EditorGUIUtility.Load("Assets/ImagesFolder/placeholder.png") as Texture2D;

        uiChoices = ui.Q<DropdownField>("choiceField");

        //single settings
        uiSingleSettings = ui.Q<Foldout>("singleSettings");
        uiSingleSettings.style.display = StyleKeyword.None;

        //optional object
        uiOptionalObj = ui.Q<Toggle>("optionalObject");
        uiOptionalObj.RegisterCallback<MouseUpEvent>((evt) => Action3());

        uiTestGO = ui.Q<ObjectField>("transformField");
        uiPosition = ui.Q<Vector3Field>("positionField");
        uiRotation = ui.Q<Vector3Field>("rotationField");
        uiScale = ui.Q<Vector3Field>("scaleField");

        //multiple settings
        uiMultipleSettings = ui.Q<Foldout>("multipleSettings");
        uiMultipleSettings.style.display = StyleKeyword.None;

        uiPlacementMethod = ui.Q<DropdownField>("placementMethod");
        uiTagField = ui.Q<TextField>("tagField");
        uiTagField.style.display = StyleKeyword.None;
        uiMeshInstances = ui.Q<IntegerField>("meshNumbers");
        uiMeshInstances.style.display = StyleKeyword.None;
        uiToggleSpawnPoint = ui.Q<Toggle>("spToggle");
        uiToggleSpawnPoint.style.display = StyleKeyword.None;
        uiSpawnPointGO = ui.Q<ObjectField>("spObject");
        uiSpawnPointGO.style.display = StyleKeyword.None;
        uiSpawnPosition = ui.Q<Vector3Field>("spVector");
        uiSpawnPosition.style.display = StyleKeyword.None;
        uiRadius = ui.Q<FloatField>("radiusField");
        uiRadius.style.display = StyleKeyword.None;

        //debugging
        uiTestToggle = ui.Q<Toggle>("testToggle");
        uiTestToggle.RegisterCallback<MouseUpEvent>((evt) => Action2());

        //prompt button
        var buttontest = ui.Q<Button>("promptButton");
        buttontest.RegisterCallback<MouseUpEvent>((evt) => Action1());
    }

    void AssignValues()
    {
        prompt = ui.Q<TextField>("promptField").value;

        //checks what options the user chose and assigns values based on it
        switch (userChoice)
        {
            //single object generation
            case "Single":
                if(uiOptionalObj.value)
                {
                    //if the user chose to input an object for transform, it uses it
                    testGO = (Transform)uiTestGO.value;
                    Debug.Log(testGO);
                    operation = 1;
                }
                else
                {
                    //if GO transform is null it makes on by default
                    GameObject go = new GameObject();
                    testGO = go.transform;

                    //assigns what the user input to position, rotation, and scale
                    testGO.position = uiPosition.value;
                    Debug.Log(testGO.transform.position);
                    
                    testGO.rotation = Quaternion.Euler(uiRotation.value);
                    Debug.Log(testGO.transform.rotation);

                    testGO.localScale = uiScale.value;
                    Debug.Log(testGO.transform.localScale);
                    operation = 2;
                }
                break;

            //multiple same object generation
            case "Multiple":
                switch (placement)
                {
                    //places objects based on tags
                    case "Object Tag":
                        tag = uiTagField.value;
                        operation = 3;
                        break;

                    //places objects in a spawn radius
                    case "Spawn Radius":
                        meshInstances = uiMeshInstances.value;
                        toggleSpawnPoint = uiToggleSpawnPoint.value;

                        //checks if the user want to use a scene object for spawn point
                        if(toggleSpawnPoint)
                        {
                            spawnPointGO = (Transform)uiSpawnPointGO.value;
                        }
                        else
                        {
                            spawnPosition = uiSpawnPosition.value;
                        }

                        radius = uiRadius.value;
                        break;

                    default:
                        Debug.Log("Something is wrong with assigning multiple placement method values");
                        break;
                }
                break;

            default:
                Debug.Log("Something broke");
                break;
        }

        //scale = uiScale.value;

        /*multipleMeshes = uiMultipleMeshes.value;
        if(multipleMeshes == true)
        {
            meshInstances = uiMeshInstances.value;
            radius = uiRadius.value;
        }*/

        MeshyCreatePrompt();
    }

    

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

        //Debug.Log(meshInstances);

        PlaceAsset();
    }

    //creation prompt for CSM
    /*public void CreatePrompt()
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
    }*/

    public void PlaceAsset()
    {
        AssetDatabase.Refresh();

        //reads the json file and puts text into string
        string text = File.ReadAllText("Assets/Models/AssetFilePath.json");

        //creates a assetfilepath class and overwrites the class with a new class from the json file
        AssetFilePath filepath = new AssetFilePath();
        JsonUtility.FromJsonOverwrite(text, filepath);

        //Debug.Log(meshInstances);

        //loads the mesh
        var test = (GameObject)EditorGUIUtility.Load(filepath.textFilePath);

        Debug.Log(userChoice);

        switch (operation)
        {
            case 1:
                var instance1 = Instantiate(test);
                instance1.transform.position = testGO.position;
                instance1.transform.rotation = testGO.rotation;
                instance1.transform.localScale = testGO.localScale;
                break;

            case 2:
                var instance2 = Instantiate(test);
                instance2.transform.position = testGO.position;
                instance2.transform.rotation = testGO.rotation;
                instance2.transform.localScale = testGO.localScale;
                break;

            case 3:
                var arr = GameObject.FindGameObjectsWithTag(tag);

                foreach (GameObject obj in arr)
                {
                    //makes the created object a child of the object in scene
                    //var tagInstance = Instantiate(test, obj.transform);
                    var tagInstance = Instantiate(test);
                    tagInstance.transform.position = obj.transform.position;
                    tagInstance.transform.rotation = obj.transform.rotation;
                    tagInstance.transform.localScale = obj.transform.localScale;
                }
                break;

            case 4:
                break;

            case 5:
                break;

            /*case "Single":
                var instance = Instantiate(test);
                instance.transform.position = testGO.position;
                instance.transform.rotation = testGO.rotation;
                instance.transform.localScale = testGO.localScale;
                break;

            case "Multiple":
                switch (placement)
                {
                    case "Object Tag":
                        var arr = GameObject.FindGameObjectsWithTag(tag);
                        
                        foreach(GameObject obj in arr)
                        {
                            var tagInstance = Instantiate(test, obj.transform);
                        }
                        
                        break;

                    case "Spawn Radius":
                        Debug.Log("spawnradius");
                        for (int i = 0; i < meshInstances; i++)
                        {
                            Vector2 spawnField = UnityEngine.Random.insideUnitCircle * radius;
                            Vector3 position = new Vector3(spawnField.x, 0f, spawnField.y);
                            position.x += testGO.position.x;
                            position.y += testGO.position.y;
                            position.z += testGO.position.z;

                            var anInstance = Instantiate(test);
                            anInstance.transform.position = position;
                            //anInstance.transform.localScale = new Vector3(scale, scale, scale);
                        }
                        break;

                    default:
                        Debug.Log("something went wrong. placement method chosen is: " + placement);
                        break;
                }
                break;*/

            default:
                Debug.Log("something went wrong in placement");
                break;
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
}