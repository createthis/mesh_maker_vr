using UnityEngine;
using UnityEditor;

namespace MMVR.Factory.UI.Panel {
    [CustomEditor(typeof(MMVR_ToolsPanelFactory))]
    [CanEditMultipleObjects]

    public class MMVR_ToolsPanelFactoryEditor : MMVR_BasePanelFactoryEditor {
        SerializedProperty fileOpen;
        SerializedProperty fileSaveAs;

        protected override void OnEnable() {
            base.OnEnable();
            fileOpen = serializedObject.FindProperty("fileOpen");
            fileSaveAs = serializedObject.FindProperty("fileSaveAs");
        }

        protected override void BuildGenerateButton() {
            // Take out this if statement to set the value using setter when ever you change it in the inspector.
            // But then it gets called a couple of times when ever inspector updates
            // By having a button, you can control when the value goes through the setter and getter, your self.
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(MMVR_ToolsPanelFactory)) {
                    MMVR_ToolsPanelFactory factory = (MMVR_ToolsPanelFactory)target;
                    factory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(fileOpen);
            EditorGUILayout.PropertyField(fileSaveAs);
        }
    }
}