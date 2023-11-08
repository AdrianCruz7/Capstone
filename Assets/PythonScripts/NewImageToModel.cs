using UnityEditor;
using UnityEditor.Scripting.Python;

public class MenuItem_NewImageToModel_Class
{
   [MenuItem("Python Scripts/NewImageToModel")]
   public static void NewImageToModel()
   {
       PythonRunner.RunFile("Assets/PythonScripts/NewImageToModel.py");
       }
};
