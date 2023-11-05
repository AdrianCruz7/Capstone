using UnityEngine;
using UnityEditor;

public class WindowPopup : EditorWindow
{
    public bool test = false;

    void OnGUI()
    {
        EditorGUILayout.LabelField("Hello World!");
        GUILayout.Space(70);
        if(GUILayout.Button("button 1")) this.Close();
        if (GUILayout.Button("button 2"))
        {
            test = true;
            this.Close();
        }
    }
}
