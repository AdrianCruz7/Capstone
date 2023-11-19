using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

//no longer using

public class QueueWindow : EditorWindow
{
    List<SaveClass> classes;
    ScrollView scrollView;
    List<VisualElement> visableQueue;
    Button startQueue;
    Action StartGeneration;
    //Action<Button> button;

    public static void NewWindow()
    {
        QueueWindow wnd = GetWindow<QueueWindow>("Queue");

        wnd.maxSize = new Vector2(470f, 1000f);
        wnd.minSize = wnd.maxSize;
        var position = wnd.position;

        position.center = new Rect(215f, 215f, Screen.currentResolution.width / 2, Screen.currentResolution.height / 2).center;
    }

    public void SetupUI()
    {
        Debug.Log("setup ui reached");
        scrollView = new ScrollView();
        rootVisualElement.Clear();
        rootVisualElement.Add(scrollView);
        startQueue = new Button();
        startQueue.text = "Begin Queue";
        scrollView.Add(startQueue);
    }

    private void OnEnable()
    {
        StartGeneration += Generate;
    }

    private void OnGUI()
    {
        startQueue.RegisterCallback<MouseUpEvent>(evt => StartGeneration());
    }

    public void Generate()
    {
        var mainWindow = GetWindow<TextToModel>();
        mainWindow.MeshyCreatePromptFromQueue(classes);

        if (classes.Count == 0)
        {
            Close();
        }
    }

    public void TestFunction(List<SaveClass> list)
    {
        classes = list;
        visableQueue = new List<VisualElement>();

        int test = 1;
        foreach (var c in classes)
        {
            Debug.Log(c + " is number " + test++);
        }

        SetupUI();

        for (int i = 0; i < classes.Count; i++)
        {
            Debug.Log(classes.ElementAt(i));
            CardProtocol(classes.ElementAt(i), i);
        }

        AddCards();
    }

    public void AddCards()
    {
        foreach(var c in visableQueue)
        {
            scrollView.Add(c);
        }
    }

    public VisualElement CardProtocol(SaveClass convert, int elementID)
    {
        VisualElement card = new VisualElement();
        card.style.width = 470;
        card.style.alignSelf = Align.Center;

        switch(convert.saveClassID)
        {
            case 1:
                Button testbtn1 = new Button();
                testbtn1.text = "Delete";

                card.Add(new Label("Prompt: " + convert.prompt));
                card.Add(new Label("Generation Type: " + convert.userChoice));
                card.Add(new Label("Child Object: " + convert.singleChildToggle.ToString()));
                card.Add(new Label("Use Object in Scene: " + convert.singleOptionalObj.ToString()));
                card.Add(testbtn1);

                Label id1 = new Label(elementID.ToString());
                id1.name = "elementID";
                id1.style.display = StyleKeyword.None;

                card.Add(id1);

                testbtn1.RegisterCallback<MouseUpEvent>(evt => DeleteButton(testbtn1.parent));
                visableQueue.Add(card);
                break;

            case 2:
                Button testbtn2 = new Button();
                testbtn2.text = "Delete";

                card.Add(new Label("Prompt: " + convert.prompt));
                card.Add(new Label("Generation Type: " + convert.userChoice));
                card.Add(new Label("Child Object: " + convert.singleChildToggle.ToString()));
                card.Add(new Label("Object Position: " + convert.singlePosition.ToString()));
                card.Add(new Label("Object Rotation: " + convert.singleRotation.ToString()));
                card.Add(new Label("Object Scale: " + convert.singleScale.ToString()));
                card.Add(testbtn2);

                Label id2 = new Label(elementID.ToString());
                id2.name = "elementID";
                id2.style.display = StyleKeyword.None;

                card.Add(id2);

                testbtn2.RegisterCallback<MouseUpEvent>(evt => DeleteButton(testbtn2.parent));
                visableQueue.Add(card);
                break;

            case 3:
                Button testbtn3 = new Button();
                testbtn3.text = "Delete";

                card.Add(new Label("Prompt: " + convert.prompt));
                card.Add(new Label("Generation Type: " + convert.userChoice));
                card.Add(new Label("Placement Type: " + convert.multiplePlacementMethod.ToString()));
                card.Add(new Label("Children Object: " + convert.multipleChildToggle.ToString()));
                card.Add(new Label("Tag: " + convert.multipleTag));
                card.Add(testbtn3);

                Label id3 = new Label(elementID.ToString());
                id3.name = "elementID";
                id3.style.display = StyleKeyword.None;

                card.Add(id3);

                testbtn3.RegisterCallback<MouseUpEvent>(evt => DeleteButton(testbtn3.parent));
                visableQueue.Add(card);
                break;

            case 4:
                Button testbtn4 = new Button();
                testbtn4.text = "Delete";

                card.Add(new Label("Prompt: " + convert.prompt));
                card.Add(new Label("Generation Type: " + convert.userChoice));
                card.Add(new Label("Placement Type: " + convert.multiplePlacementMethod.ToString()));
                card.Add(new Label("Children Object: " + convert.multipleChildToggle.ToString()));
                card.Add(new Label("Spawn Point Toggle: " + convert.spawnPointToggle.ToString()));
                card.Add(new Label("Number of Objects: " + convert.numberOfInstances.ToString()));
                card.Add(new Label("Object ID: " + convert.spawnPointGO.ToString()));
                //card.Add(new Label("Radius Visual: " + convert.radiusVisual.ToString()));
                card.Add(new Label("Radius Value: " + convert.radiusValue.ToString()));
                card.Add(new Label("Rotation Min: " + convert.rotationMin.ToString()));
                card.Add(new Label("Rotation Max: " + convert.rotationMax.ToString()));
                card.Add(new Label("Scale Min: " + convert.scaleMin.ToString()));
                card.Add(new Label("Scale Max: " + convert.scaleMax.ToString()));
                card.Add(testbtn4);

                Label id4 = new Label(elementID.ToString());
                id4.name = "elementID";
                id4.style.display = StyleKeyword.None;

                card.Add(id4);

                testbtn4.RegisterCallback<MouseUpEvent>(evt => DeleteButton(testbtn4.parent));
                visableQueue.Add(card);
                break;

            case 5:
                Button testbtn5 = new Button();
                testbtn5.text = "Delete";

                card.Add(new Label("Prompt: " + convert.prompt));
                card.Add(new Label("Generation Type: " + convert.userChoice));
                card.Add(new Label("Placement Type: " + convert.multiplePlacementMethod.ToString()));
                card.Add(new Label("Children Object: " + convert.multipleChildToggle.ToString()));
                card.Add(new Label("Spawn Point Toggle: " + convert.spawnPointToggle.ToString()));
                card.Add(new Label("Number of Objects: " + convert.numberOfInstances.ToString()));
                card.Add(new Label("Spawn Point Position: " + convert.spawnPointPosition.ToString()));
                //card.Add(new Label("Radius Visual: " + convert.radiusVisual.ToString()));
                card.Add(new Label("Radius Value: " + convert.radiusValue.ToString()));
                card.Add(new Label("Rotation Min: " + convert.rotationMin.ToString()));
                card.Add(new Label("Rotation Max: " + convert.rotationMax.ToString()));
                card.Add(new Label("Scale Min: " + convert.scaleMin.ToString()));
                card.Add(new Label("Scale Max: " + convert.scaleMax.ToString()));
                card.Add(testbtn5);

                Label id5 = new Label(elementID.ToString());
                id5.name = "elementID";
                id5.style.display = StyleKeyword.None;

                card.Add(id5);

                testbtn5.RegisterCallback<MouseUpEvent>(evt => DeleteButton(testbtn5.parent));
                visableQueue.Add(card);
                break;

            default:
                Debug.LogError("Error: saveClassID not assigned (How did you manage that?)");
                break;
        }

        return rootVisualElement;
    }

    public void DeleteButton(VisualElement test)
    {
        int elementID = int.Parse(test.Q<Label>("elementID").text);
        classes.RemoveAt(elementID);
        visableQueue.RemoveAt(elementID);
        test.RemoveFromHierarchy();
       
        TestFunction(classes);

        var mainWindow = GetWindow<TextToModel>();
        mainWindow.saveClasses = classes;

        if(classes.Count == 0)
        {
            Close();
        }
    }
}
