using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//no longer using

public class CreationSettings : EditorWindow
{
    Transform testGO;
    bool multipleMeshes = false;
    int meshInstances = 1;
    float scale = 1.0f;
    float radius = 5.0f;
    string prompt;

    public static void NewWindow()
    {
        CreationSettings wnd = GetWindow<CreationSettings>("Creation Settings");
        wnd.maxSize = new Vector2(470f, 1000f);
        wnd.minSize = wnd.maxSize;
        var position = wnd.position;

        position.center = new Rect(215f, 215f, Screen.currentResolution.width / 2, Screen.currentResolution.height / 2).center;
        //wnd.position = position;

        Debug.Log("Happens second");
    }

    private void OnGUI()
    {
        /*if (meshInstances < 1)
        {
            meshInstances = 1;
        }
        var image = EditorGUIUtility.Load("Assets/ImagesFolder/placeholder.png") as Texture2D;

        EditorGUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 200;

        GUILayout.Label("Insert your prompt into the field below", EditorStyles.boldLabel);
        prompt = GUILayout.TextField(prompt);
        if(GUILayout.Button("Create Object"))
        {
            TestFunction();
        }

        //YEAHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH
        GUILayout.Label(image, GUILayout.Width(463), GUILayout.Height(463));
        
        GUILayout.Label("Object Spawning Settings", EditorStyles.boldLabel);
        testGO = EditorGUILayout.ObjectField("Object Transform", testGO, typeof(Transform), true) as Transform;
        scale = EditorGUILayout.FloatField("Object Scale", scale);
        multipleMeshes = EditorGUILayout.Toggle("Multiple Objects", multipleMeshes);
        if(multipleMeshes)
        {
            meshInstances = EditorGUILayout.IntField("Number of objects to create", meshInstances);
            radius = EditorGUILayout.FloatField("Spawning Radius", radius);
        }

        GUILayout.Label("");
        EditorGUILayout.EndVertical();*/
    }

    public void TestFunction()
    {
        Debug.Log("Test output");
    }
}
