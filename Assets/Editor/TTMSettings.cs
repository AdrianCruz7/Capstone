using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TTMSettings : EditorWindow
{
    string openAI = "";
    string meshy = "";
    string CSM = "";

    public void NewWindow()
    {
        TTMSettings wnd = GetWindow<TTMSettings>("TTMSettings");

        wnd.maxSize = new Vector2(470f, 1000f);
        wnd.minSize = wnd.maxSize;
        var position = wnd.position;

        position.center = new Rect(215f, 215f, Screen.currentResolution.width / 2, Screen.currentResolution.height / 2).center;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        openAI = EditorGUILayout.TextField("OpenAI Key:", openAI);
        CSM = EditorGUILayout.TextField("CSM Key:", CSM);
        meshy = EditorGUILayout.TextField("Meshy Key:", meshy);

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save"))
        {
            Confirmation();
        }
        if(GUILayout.Button("Close"))
        {
            Close();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    public void Confirmation()
    {
        bool confirmation = EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to save these new keys?", "Yes", "No");

        if (confirmation)
        {
            Debug.Log("Confirmation is a yes");
            Save();
            Close();
        }
        else
        {
            Debug.Log("Confirmation is a no");
        }
    }

    public void Save()
    {
        string openAI_FP = Application.dataPath + "/Keys/OpenAIKey.json";
        string CSM_FP = Application.dataPath + "/Keys/CSMKey.json";
        string meshy_FP = Application.dataPath + "/Keys/MeshyKey.json";

        KeyClass key1 = new KeyClass(openAI);
        KeyClass key2 = new KeyClass(CSM);
        KeyClass key3 = new KeyClass(meshy);

        string key1JSON = JsonUtility.ToJson(key1);
        string key2JSON = JsonUtility.ToJson(key2);
        string key3JSON = JsonUtility.ToJson(key3);

        File.WriteAllText(openAI_FP, key1JSON);
        File.WriteAllText(CSM_FP, key2JSON);
        File.WriteAllText(meshy_FP, key3JSON);

        AssetDatabase.Refresh();
    }
}
