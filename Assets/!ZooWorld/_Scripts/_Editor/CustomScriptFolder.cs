using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEngine;

public class ScriptOrganizer : UnityEditor.AssetModificationProcessor
{
    private static readonly string TargetFolder = "Assets/!ZooWorld/_Scripts"; // Set your target folder here

    [DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        // Check for newly created scripts in the root Assets folder
        string[] rootAssets = Directory.GetFiles("Assets", "*.cs");

        foreach (string scriptPath in rootAssets)
        {
            string fileName = Path.GetFileName(scriptPath);
            string destinationPath = Path.Combine(TargetFolder, fileName);

            if (!Directory.Exists(TargetFolder))
            {
                Directory.CreateDirectory(TargetFolder);
            }

            if (!File.Exists(destinationPath))
            {
                AssetDatabase.MoveAsset(scriptPath, destinationPath);
                Debug.Log($"Moved {fileName} to {TargetFolder}");
            }
        }

        AssetDatabase.Refresh();
    }
}