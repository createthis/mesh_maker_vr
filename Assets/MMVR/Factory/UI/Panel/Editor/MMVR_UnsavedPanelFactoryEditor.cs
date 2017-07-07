using UnityEngine;
using UnityEditor;

namespace MMVR.Factory.UI.Panel {
    [CustomEditor(typeof(MMVR_UnsavedPanelFactory))]
    [CanEditMultipleObjects]

    public class MMVR_UnsavedPanelFactoryEditor : MMVR_BasePanelFactoryEditor {
        SerializedProperty fileManager;

        protected override void OnEnable() {
            base.OnEnable();
            fileManager = serializedObject.FindProperty("fileManager");
        }

        protected override void BuildGenerateButton() {
            // Take out this if statement to set the value using setter when ever you change it in the inspector.
            // But then it gets called a couple of times when ever inspector updates
            // By having a button, you can control when the value goes through the setter and getter, your self.
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(MMVR_UnsavedPanelFactory)) {
                    MMVR_UnsavedPanelFactory factory = (MMVR_UnsavedPanelFactory)target;
                    factory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(fileManager);
        }
    }
}