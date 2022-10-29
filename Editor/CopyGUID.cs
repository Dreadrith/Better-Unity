using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CopyGUID : MonoBehaviour
{
    [MenuItem("Assets/Copy GUID")]
    private static void CopyGUIDMethod()
    {
        if (Selection.assetGUIDs.Length > 0)
            EditorGUIUtility.systemCopyBuffer = Selection.assetGUIDs[0];
    }
}
