using UnityEditor;
using UnityEditor.Scripting.Python;

public class MenuItem_ExecuteTestScript_Class
{
   [MenuItem("Python Scripts/Execute Test Script")]
   public static void ExecuteTestScript()
   {
       PythonRunner.RunFile("Assets/PythonScripts/ExecuteTestScript.py");
       }
};
