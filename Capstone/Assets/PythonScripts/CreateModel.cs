using UnityEditor;
using UnityEditor.Scripting.Python;

public class MenuItem_CreateModel_Class
{
   [MenuItem("Python Scripts/Create Model")]
   public static void CreateModel()
   {
       PythonRunner.RunFile("Assets/PythonScripts/CreateModel.py");
       }
};
