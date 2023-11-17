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
    }

    private void OnGUI()
    {
        
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
                break;
            case 4:
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
