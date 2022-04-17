using UnityEngine;
using UnityEditor;
using System.IO;

//Adds 3 new Assets context menu items: Copy, Cut & Paste

namespace BetterUnity
{
    public class CopyCutPaste
    {
        private static bool copy;
        private static string[] _tempGUID;
        private static string[] assetsGUID;

        [MenuItem("Assets/Copy", false, 20)]
        private static void Copy()
        {
            copy = true;
            assetsGUID = _tempGUID;
        }

        [MenuItem("Assets/Cut", false, 20)]
        private static void Cut()
        {
            copy = false;
            assetsGUID = _tempGUID;
        }

        [MenuItem("Assets/Paste", false, 20)]
        private static void Paste()
        {
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            Move(folderPath, assetsGUID, copy);
        }

        public static void Move(string destination, string[] assetsGUID, bool copy)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(destination)) && !AssetDatabase.IsValidFolder(destination))
                destination = Path.GetDirectoryName(destination);

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (string s in assetsGUID)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(s);
                    string fileName = Path.GetFileName(filePath);


                    if (copy)
                    {
                        AssetDatabase.CopyAsset(filePath, AssetDatabase.GenerateUniqueAssetPath(destination + "/" + fileName));
                    }
                    else
                    {
                        if (destination + "/" + fileName != filePath)
                            AssetDatabase.MoveAsset(filePath, AssetDatabase.GenerateUniqueAssetPath(destination + "/" + fileName));
                    }
                }
            }
            catch(System.Exception e) { Debug.LogError(e);}
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        [MenuItem("Assets/Copy", true, 20)]
        [MenuItem("Assets/Cut", true, 20)]
        private static bool ValidateSelect()
        {
            _tempGUID = Selection.assetGUIDs;
            return _tempGUID.Length > 0;
        }

        [MenuItem("Assets/Paste", true, 20)]
        private static bool ValidateMove()
        {
            return (assetsGUID != null && assetsGUID.Length > 0 && Selection.activeObject);
        }

       

    }
}