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
using UnityEditor.PackageManager.UI;
using UnityEditor.TerrainTools;
using Unity.VisualScripting;
using System.Linq;

public class TextToModel : EditorWindow
{
    //choice
    DropdownField uiChoices;
    string userChoice = "";

    //object transformation settings
    Foldout uiSingleSettings;

    Toggle uiSingleChildToggle;
    bool singleChildToggle;

    Toggle uiOptionalObj;
    bool optionalObj;

    ObjectField uiTestGO;
    Transform testGO;

    Vector3Field uiScale;
    Vector3Field uiPosition;
    Vector3Field uiRotation;

    //multiple mesh generation settings
    Foldout uiMultipleSettings;

    Toggle uiMultipleChildrenToggle;
    bool multipleChildrenToggle;

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

    Toggle uiRadiusVisual;
    bool radiusVisual;

    FloatField uiRadius;
    float radius = 1.0f;

    Foldout uiRotationAndScale;

    Vector3Field uiRotationMin;
    Vector3 rotationMin;
    Vector3Field uiRotationMax;
    Vector3 rotationMax;
    Vector3Field uiScaleMin;
    Vector3 scaleMin;
    Vector3Field uiScaleMax;
    Vector3 scaleMax;

    //miscellaneous
    string prompt;
    Label labelImage;
    Texture2D image;

    int operation = 0;
    //remove redundant numbers later
    //1 = Single generation with an object used for placement
    //2 = Single generation with user manually inputing the transform
    //3 = Multiple generation using tags from objects in scene
    //4 = Mulitple generation 
    //5 = Multiple generation

    //test
    GameObject go;

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
    Action Action7;

    GameObject radiusGismo;

    [MenuItem("Capstone/Text To 3D Asset")]
    public static void ShowExample()
    {
        TextToModel wnd = GetWindow<TextToModel>("TextToModel");
        wnd.maxSize = new Vector2(470f, 1000f);
        wnd.minSize = wnd.maxSize;
    }

    private void OnEnable()
    {
        //Settings += WindowSettings;
        Settings += ToggleRadiusVisual;
        Action1 += Confirmation;
        Action2 += ToggleRadiusVisual;
        Action3 += UIObjectInSceneDisplay;
        Action4 += UIOptionsDisplay;
        Action5 += UIPlacementMethodDisplay;
        Action6 += UIToggleGOSpawnPoint;
        Action7 += UpdateRadiusUI;
        SetUpUI();
        UIObjectInSceneDisplay();
    }

    private void OnDestroy()
    {
        var kill = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "GUITestObject");

        foreach (var obj in kill)
        {
            DestroyImmediate(obj);
        }
    }

    //update position of sphere for when the value of spawn radius points changes (both manual input and object transform)
    //update position of sphere if object for spawn point is toggled
    public void UpdateRadiusUI()
    {
        uiRadius.value = Mathf.Clamp(uiRadius.value, 1, 100);
        UpdateRadiusVisual1();
    }

    //case if radius value changes
    public void UpdateRadiusVisual1()
    {
        radius = uiRadius.value;
        radiusVisual = uiRadiusVisual.value;

        if(radiusVisual)
        {
            GameObject rock = GameObject.Find("GUITestObject");
            rock.GetComponent<DrawScript>().radius = radius;
        }
    }

    public void ToggleRadiusVisual()
    {
        GameObject paper;

        if (GameObject.Find("GUITestObject"))
        {
            paper = GameObject.Find("GUITestObject");
            Debug.Log("Paper is " + paper);
        }
        else
        {
            paper = null;
            Debug.Log("Paper is null");
        }

        radius = uiRadius.value;
        radiusVisual = uiRadiusVisual.value;
        Debug.Log(radiusVisual);
        Debug.Log(paper);

        if (radiusVisual && paper == null)
        {
            paper = new GameObject("GUITestObject");
            paper.AddComponent<DrawScript>().radius = radius;
        }
        else if(!radiusVisual && paper != null)
        {
            DestroyImmediate(paper);
        }/*
        else
        {
            paper.GetComponent<DrawScript>().radius = radius;
        }*/

        //RaycastHit hit;
        //Gizmos.DrawSphere(paper.transform.position, radius);
        //Physics.SphereCast(paper.transform.position, radius, paper.transform.forward, out hit);
    }

    private void OnGUI()
    {
        uiChoices.RegisterValueChangedCallback(evt => Action4());
        uiPlacementMethod.RegisterValueChangedCallback(evt => Action5());
        uiToggleSpawnPoint.RegisterValueChangedCallback(evt => Action6());
        uiRadiusVisual.RegisterValueChangedCallback(evt => Action2());
        uiMeshInstances.RegisterValueChangedCallback(evt => { uiMeshInstances.value = Mathf.Clamp(uiMeshInstances.value, 1, 100); });
        uiTagField.RegisterValueChangedCallback(evt => { tag =  uiTagField.value; });
        uiRadius.RegisterValueChangedCallback(evt => Action7());
    } 

    //displays based on choice of single or multiple generation
    public void UIOptionsDisplay()
    {
        switch (uiChoices.value)
        {
            //single generation option
            case "Single Object":
                Debug.Log("Single");
                uiSingleSettings.style.display = StyleKeyword.Auto;
                uiMultipleSettings.style.display = StyleKeyword.None;
                userChoice = "Single";
                break;

            //multiple generation option
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

    //toggle for either using an object for spawn point or manually input values
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

    //toggles based on which placement method in multiple object generation is chosen
    public void UIPlacementMethodDisplay()
    {
        switch (uiPlacementMethod.value)
        {
            case "Object Tag":
                uiMultipleChildrenToggle.style.display = StyleKeyword.Auto;
                uiTagField.style.display = StyleKeyword.Auto;
                uiMeshInstances.style.display = StyleKeyword.None;
                uiToggleSpawnPoint.style.display = StyleKeyword.None;
                uiSpawnPointGO.style.display = StyleKeyword.None;
                uiSpawnPosition.style.display = StyleKeyword.None;
                uiRadiusVisual.style.display = StyleKeyword.None;
                uiRadius.style.display = StyleKeyword.None;
                uiRotationAndScale.style.display = StyleKeyword.None;
                placement = uiPlacementMethod.value;
                break;

            case "Spawn Radius":
                uiMultipleChildrenToggle.style.display = StyleKeyword.Auto;
                uiTagField.style.display = StyleKeyword.None;
                uiMeshInstances.style.display = StyleKeyword.Auto;
                uiToggleSpawnPoint.style.display = StyleKeyword.Auto;
                UIToggleGOSpawnPoint();
                uiRadiusVisual.style.display = StyleKeyword.Auto;
                uiRadius.style.display = StyleKeyword.Auto;
                uiRotationAndScale.style.display = StyleKeyword.Auto;
                placement = uiPlacementMethod.value;
                break;

            default:
                Debug.Log("somethings wrong");
                break;
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

        //optional turn object into child
        uiSingleChildToggle = ui.Q<Toggle>("childSingleObject");

        //optional object
        uiOptionalObj = ui.Q<Toggle>("optionalObject");
        uiOptionalObj.RegisterCallback<MouseUpEvent>((evt) => Action3());

        uiTestGO = ui.Q<ObjectField>("transformField");
        uiPosition = ui.Q<Vector3Field>("positionField");
        uiRotation = ui.Q<Vector3Field>("rotationField");
        uiScale = ui.Q<Vector3Field>("scaleField");

        //multiple settings
        uiMultipleSettings = ui.Q<Foldout>("multipleSettings");

        //placement method for multiple objects
        uiPlacementMethod = ui.Q<DropdownField>("placementMethod");

        //optional turn objects into children (in both tags and radius)
        uiMultipleChildrenToggle = ui.Q<Toggle>("childrenMultipleOption");

        //tag option
        uiTagField = ui.Q<TextField>("tagField");

        //radius option
        uiMeshInstances = ui.Q<IntegerField>("meshNumbers");
        uiToggleSpawnPoint = ui.Q<Toggle>("spToggle");
        uiSpawnPointGO = ui.Q<ObjectField>("spObject");
        uiSpawnPosition = ui.Q<Vector3Field>("spVector");
        uiRadiusVisual = ui.Q<Toggle>("radiusVisual");
        uiRadius = ui.Q<FloatField>("radiusField");
        uiRotationMin = ui.Q<Vector3Field>("rotationMinValues");
        uiRotationMax = ui.Q<Vector3Field>("rotationMaxValues");
        uiScaleMin = ui.Q<Vector3Field>("scaleMinValues");
        uiScaleMax = ui.Q<Vector3Field>("scaleMaxValues");
        uiRotationAndScale = ui.Q<Foldout>("objectRotationAndScale");

        //prompt button
        var buttontest = ui.Q<Button>("promptButton");
        buttontest.RegisterCallback<MouseUpEvent>((evt) => Action1());
    }

    void AssignValues()
    {
        //prompt value
        prompt = ui.Q<TextField>("promptField").value;

        //checks what options the user chose and assigns values based on it
        switch (userChoice)
        {
            //single object generation
            case "Single":
                singleChildToggle = uiSingleChildToggle.value;

                if(optionalObj)
                {
                    //if the user chose to input an object for transform, it uses it
                    testGO = (Transform)uiTestGO.value;
                    operation = 1;
                }
                else
                {
                    //if GO transform is null it makes on by default
                    go = new GameObject("testName");
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
                multipleChildrenToggle = uiMultipleChildrenToggle.value;

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
                            operation = 4;
                        }
                        else
                        {
                            GameObject placeGO = new GameObject();
                            spawnPointGO = placeGO.transform;
                            spawnPointGO.position = uiSpawnPosition.value;
                            operation = 5;
                        }

                        rotationMin = uiRotationMin.value;
                        rotationMax = uiRotationMax.value;
                        scaleMin = uiScaleMin.value;
                        scaleMax = uiScaleMax.value;

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

        //loads the mesh
        var test = (GameObject)EditorGUIUtility.Load(filepath.textFilePath);

        switch (operation)
        {
            case 1:
                if(singleChildToggle)
                {
                    //child
                    var instance1 = Instantiate(test, testGO.transform);
                }

                else
                {
                    //no child
                    var instance1 = Instantiate(test);
                    instance1.transform.position = testGO.position;
                    instance1.transform.rotation = testGO.rotation;
                    instance1.transform.localScale = testGO.localScale;
                    DestroyImmediate(go);
                }
                break;

            case 2:
                if (singleChildToggle)
                {
                    //child
                    var instance1 = Instantiate(test, testGO.transform);
                }

                else
                {
                    //no child
                    var instance1 = Instantiate(test);
                    instance1.transform.position = testGO.position;
                    instance1.transform.rotation = testGO.rotation;
                    instance1.transform.localScale = testGO.localScale;
                    DestroyImmediate(go);
                }
                break;

            case 3:
                var arr = GameObject.FindGameObjectsWithTag(tag);

                //all objects with the tag name
                foreach (GameObject obj in arr)
                {
                    GameObject tagInstance;

                    if(multipleChildrenToggle)
                    {
                        //child
                        tagInstance = Instantiate(test, obj.transform);
                    }
                    else
                    {
                        //no child
                        tagInstance = Instantiate(test);
                        tagInstance.transform.position = obj.transform.position;
                        tagInstance.transform.rotation = obj.transform.rotation;
                        tagInstance.transform.localScale = obj.transform.localScale;
                    }
                }
                break;

            case 4:
                for (int i = 0; i < meshInstances; i++)
                {
                    Vector2 spawnField = UnityEngine.Random.insideUnitCircle * radius;
                    Vector3 position = new Vector3(spawnField.x, 0f, spawnField.y);
                    position.x += spawnPointGO.position.x;
                    position.y += spawnPointGO.position.y;
                    position.z += spawnPointGO.position.z;

                    var rotX = UnityEngine.Random.Range(rotationMin.x, rotationMax.x);
                    Debug.Log(rotX);
                    var rotY = UnityEngine.Random.Range(rotationMin.y, rotationMax.y);
                    Debug.Log(rotY);
                    var rotZ = UnityEngine.Random.Range(rotationMin.z, rotationMax.z);
                    Debug.Log(rotZ);
                    var scaleX = UnityEngine.Random.Range(scaleMin.x, scaleMax.x);
                    Debug.Log(scaleX);
                    var scaleY = UnityEngine.Random.Range(scaleMin.y, scaleMax.y);
                    Debug.Log(scaleY);
                    var scaleZ = UnityEngine.Random.Range(scaleMin.z, scaleMax.z);
                    Debug.Log(scaleZ);

                    GameObject anInstance;

                    if(multipleChildrenToggle)
                    {
                        //child
                        anInstance = Instantiate(test, spawnPointGO);
                        
                    }
                    else
                    {
                        //no child
                        anInstance = Instantiate(test);
                    }

                    anInstance.transform.position = position;
                    anInstance.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                    anInstance.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                }
                break;

            case 5:
                for (int i = 0; i < meshInstances; i++)
                {
                    Vector2 spawnField = UnityEngine.Random.insideUnitCircle * radius;
                    Vector3 position = new Vector3(spawnField.x, 0f, spawnField.y);
                    position.x += spawnPointGO.position.x;
                    position.y += spawnPointGO.position.y;
                    position.z += spawnPointGO.position.z;

                    var rotX = UnityEngine.Random.Range(rotationMin.x, rotationMax.x + 1);
                    var rotY = UnityEngine.Random.Range(rotationMin.y, rotationMax.y + 1);
                    var rotZ = UnityEngine.Random.Range(rotationMin.z, rotationMax.z + 1);
                    var scaleX = UnityEngine.Random.Range(scaleMin.x, scaleMax.x + 1);
                    var scaleY = UnityEngine.Random.Range(scaleMin.y, scaleMax.y + 1);
                    var scaleZ = UnityEngine.Random.Range(scaleMin.z, scaleMax.z + 1);

                    GameObject anInstance;

                    if (multipleChildrenToggle)
                    {
                        //child
                        anInstance = Instantiate(test, spawnPointGO);
                    }
                    else
                    {
                        //no child
                        anInstance = Instantiate(test);
                    }

                    anInstance.transform.position = position;
                    anInstance.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                    anInstance.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                }
                break;

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

//#if UNITY_EDITOR



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

    /*public void testfun()
    {
        Debug.Log(UnityEngine.Random.Range(1, 2));

        *//*testGO = (Transform)uiTestGO.value;

        Debug.Log(testGO.rotation);
        Debug.Log(testGO.rotation.x);
        Debug.Log(testGO.rotation.y);
        Debug.Log(testGO.rotation.z);*/

    /*Debug.Log("R Min X " + uiRotationMin.value.x);
    Debug.Log("R Min Y " + uiRotationMin.value.y);
    Debug.Log("R Min Z " + uiRotationMin.value.z);

    Debug.Log("R Max X " + uiRotationMax.value.x);
    Debug.Log("R Max Y " + uiRotationMax.value.y);
    Debug.Log("R Max Z " + uiRotationMax.value.z);

    Debug.Log("S Min X " + uiScaleMin.value.x);
    Debug.Log("S Min Y " + uiScaleMin.value.y);
    Debug.Log("S Min Z " + uiScaleMin.value.z);

    Debug.Log("S Max X " + uiScaleMax.value.x);
    Debug.Log("S Max Y " + uiScaleMax.value.y);
    Debug.Log("S Max Z " + uiScaleMax.value.z);*//*
}*/
}