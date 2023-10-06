using UnityEditor;
using UnityEditor.Scripting.Python;

public class MenuItem_CreateImage_Class
{
   [MenuItem("Python Scripts/Create Image")]
   public static void CreateImage()
   {
       PythonRunner.RunFile("Assets/PythonScripts/CreateImage.py");
       }
};
