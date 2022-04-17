using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Copy the path from the root transform to the target GameObject

namespace BetterUnity
{
    public class GOCopyPath
    {
        private const bool log = true;

        [MenuItem("GameObject/Copy Path", false, -1000)]
        private static void GameObjectCopyPath()
        {
            var go = Selection.activeGameObject;
            if (!go)
            {
                Debug.LogWarning("No GameObject selected");
                return;
            }

            string path = AnimationUtility.CalculateTransformPath(go.transform, go.transform.root);
            if (log) Debug.Log("Path: " + path);
            GUIUtility.systemCopyBuffer = path;
        }
    }
}
