using UnityEditor;
using System.IO;

//Adds new context menu item: Assets > Create > Text File
//Creates a new text file in the destination folder

namespace BetterUnity
{
    public class CreateTxt
    {
        [MenuItem("Assets/Create/Text File", false, 20)]
        private static void CreateMyHeckingTextFile()
        {
            //Get the path of what was used on right click
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            //If using Unity's toolbar context menu. There may be no selection. So use the main Assets folder.
            if (string.IsNullOrWhiteSpace(path)) path = "Assets";

            //If it's a folder, use it. If it's a file, get the parent folder. Name it "New Text File".
            string txtPath = (AssetDatabase.IsValidFolder(path) ? path : Path.GetDirectoryName(path)) + "/New Text File.txt";

            //Make it unique
            txtPath = AssetDatabase.GenerateUniqueAssetPath(txtPath);

            //Create it and Dispose of the StreamWriter
            File.CreateText(txtPath).Dispose();

            //Import it
            AssetDatabase.ImportAsset(txtPath);

            //Highlight it
            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(txtPath));
        }
    }
}