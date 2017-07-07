using UnityEngine;
using UnityEditor;

namespace MMVR.Factory.UI.Panel {
    [CustomEditor(typeof(MMVR_SettingsPanelFactory))]
    [CanEditMultipleObjects]

    public class MMVR_SettingsPanelFactoryEditor : MMVR_BasePanelFactoryEditor {
        SerializedProperty skyboxManager;

        protected override void OnEnable() {
            base.OnEnable();
            skyboxManager = serializedObject.FindProperty("skyboxManager");
        }

        protected override void BuildGenerateButton() {
            // Take out this if statement to set the value using setter when ever you change it in the inspector.
            // But then it gets called a couple of times when ever inspector updates
            // By having a button, you can control when the value goes through the setter and getter, your self.
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(MMVR_SettingsPanelFactory)) {
                    MMVR_SettingsPanelFactory factory = (MMVR_SettingsPanelFactory)target;
                    factory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(skyboxManager);
        }
    }
}