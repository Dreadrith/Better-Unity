using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

//Replicates Unity's Transform Inspector but adds new Context Menu to each field.
//Allows you to Copy, Paste and Reset any transform field individually.

namespace BetterUnity
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform))]
    public class TransformEditor : Editor
    {
        private static System.Type RotationGUIType;
        private static MethodInfo RotationGUIEnableMethod;
        private static MethodInfo DrawRotationGUIMethod;
        private object rotationGUI;

        private SerializedProperty m_LocalPosition;
        private SerializedProperty m_LocalRotation;
        private SerializedProperty m_LocalScale;

        private static Vector3 copiedValues;
        private static bool hasCopiedValues;
        private static int contextChoice;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_LocalPosition, positionContent);
            Rect posRect = GUILayoutUtility.GetLastRect();

            DrawRotationGUIMethod.Invoke(rotationGUI, null);
            Rect rotRect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.PropertyField(m_LocalScale, scaleContent);
            Rect scaleRect = GUILayoutUtility.GetLastRect();

            serializedObject.ApplyModifiedProperties();

            Event e = Event.current;
            Vector2 m = e.mousePosition;
            if (e.type == EventType.ContextClick)
            {
                contextChoice = 0;
                if (posRect.Contains(m)) contextChoice = 1;
                if (rotRect.Contains(m)) contextChoice = 2;
                if (scaleRect.Contains(m)) contextChoice = 3;
                if (contextChoice > 0)
                {
                    GenericMenu myMenu = new GenericMenu();
                    myMenu.AddItem(new GUIContent("Copy"), false, CopyFieldValues);
                    myMenu.AddItem(new GUIContent("Paste"), false, hasCopiedValues ? new GenericMenu.MenuFunction(PasteFieldValues) : null);
                    myMenu.AddItem(new GUIContent("Reset"), false, ResetFieldValues);
                    myMenu.ShowAsContext();
                }
            }
        }

        private void ResetFieldValues()
        {
            switch (contextChoice)
            {
                case 1:
                    m_LocalPosition.vector3Value = Vector3.zero;
                    break;
                case 2:
                    m_LocalRotation.quaternionValue = new Quaternion();
                    break;
                case 3:
                    m_LocalScale.vector3Value = Vector3.one;
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
        private void CopyFieldValues()
        {
            hasCopiedValues = true;
            switch (contextChoice)
            {
                case 1: copiedValues = m_LocalPosition.vector3Value;
                    break;
                case 2:
                    copiedValues = m_LocalRotation.quaternionValue.eulerAngles;
                    break;
                case 3:
                    copiedValues = m_LocalScale.vector3Value;
                    break;
            }
        }

        private void PasteFieldValues()
        {
            switch (contextChoice)
            {
                case 1:
                    m_LocalPosition.vector3Value = copiedValues;
                    break;
                case 2:
                    var tempQuaternion = new Quaternion {eulerAngles = copiedValues};
                    m_LocalRotation.quaternionValue = tempQuaternion;
                    break;
                case 3:
                     m_LocalScale.vector3Value = copiedValues ;
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            if (RotationGUIType == null) RotationGUIType = System.Type.GetType("UnityEditor.TransformRotationGUI, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (RotationGUIEnableMethod == null) RotationGUIEnableMethod = RotationGUIType.GetMethod("OnEnable", BindingFlags.Public | BindingFlags.Instance);
            if (DrawRotationGUIMethod == null) DrawRotationGUIMethod = RotationGUIType.GetMethod("RotationField", Array.Empty<System.Type>());

            m_LocalPosition = serializedObject.FindProperty("m_LocalPosition");
            m_LocalRotation = serializedObject.FindProperty("m_LocalRotation");
            m_LocalScale = serializedObject.FindProperty("m_LocalScale");

            if (rotationGUI == null) rotationGUI = Activator.CreateInstance(RotationGUIType);
            RotationGUIEnableMethod.Invoke(rotationGUI, new object[] {m_LocalRotation, rotationContent});
        }

        private GUIContent positionContent = new GUIContent("Position", "The local position of this GameObject relative to the parent.");
        private GUIContent rotationContent = new GUIContent("Rotation", "The local rotation of this GameObject relative to the parent.");
        private GUIContent scaleContent = new GUIContent("Scale", "The local scaling of this GameObject relative to the parent.");
        private const string floatingPointWarning = "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.";

    }
}
