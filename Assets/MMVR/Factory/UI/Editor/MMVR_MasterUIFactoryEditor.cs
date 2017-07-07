using UnityEngine;
using UnityEditor;
using CreateThis.Factory;

namespace MMVR.Factory.UI {
    [CustomEditor(typeof(MMVR_MasterUIFactory))]
    [CanEditMultipleObjects]

    public class MMVR_MasterUIFactoryEditor : BaseFactoryEditor {
        SerializedProperty parent;
        SerializedProperty skyboxManager;
        SerializedProperty touchPadMenuController;
        SerializedProperty fileManager;
        SerializedProperty unsavedPanelLocalPosition;
        SerializedProperty notificationPanelLocalPosition;
        SerializedProperty settingsLocalPosition;
        SerializedProperty toolsLocalPosition;
        SerializedProperty keyboardLocalPosition;
        SerializedProperty fileOpenLocalPosition;
        SerializedProperty fileSaveAsLocalPosition;

        protected override void OnEnable() {
            base.OnEnable();
            parent = serializedObject.FindProperty("parent");
            skyboxManager = serializedObject.FindProperty("skyboxManager");
            touchPadMenuController = serializedObject.FindProperty("touchPadMenuController");
            fileManager = serializedObject.FindProperty("fileManager");
            unsavedPanelLocalPosition = serializedObject.FindProperty("unsavedPanelLocalPosition");
            notificationPanelLocalPosition = serializedObject.FindProperty("notificationPanelLocalPosition");
            settingsLocalPosition = serializedObject.FindProperty("settingsLocalPosition");
            toolsLocalPosition = serializedObject.FindProperty("toolsLocalPosition");
            keyboardLocalPosition = serializedObject.FindProperty("keyboardLocalPosition");
            fileOpenLocalPosition = serializedObject.FindProperty("fileOpenLocalPosition");
            fileSaveAsLocalPosition = serializedObject.FindProperty("fileSaveAsLocalPosition");
        }

        protected override void BuildGenerateButton() {
            // Take out this if statement to set the value using setter when ever you change it in the inspector.
            // But then it gets called a couple of times when ever inspector updates
            // By having a button, you can control when the value goes through the setter and getter, your self.
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(MMVR_MasterUIFactory)) {
                    MMVR_MasterUIFactory factory = (MMVR_MasterUIFactory)target;
                    factory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(parent);
            EditorGUILayout.PropertyField(skyboxManager);
            EditorGUILayout.PropertyField(touchPadMenuController);
            EditorGUILayout.PropertyField(fileManager);
            EditorGUILayout.PropertyField(unsavedPanelLocalPosition);
            EditorGUILayout.PropertyField(notificationPanelLocalPosition);
            EditorGUILayout.PropertyField(settingsLocalPosition);
            EditorGUILayout.PropertyField(toolsLocalPosition);
            EditorGUILayout.PropertyField(keyboardLocalPosition);
            EditorGUILayout.PropertyField(fileOpenLocalPosition);
            EditorGUILayout.PropertyField(fileSaveAsLocalPosition);
        }
    }
}