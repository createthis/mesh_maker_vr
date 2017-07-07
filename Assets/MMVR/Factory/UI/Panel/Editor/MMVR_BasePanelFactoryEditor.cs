using UnityEngine;
using UnityEditor;
using CreateThis.Factory;

namespace MMVR.Factory.UI.Panel {
    [CustomEditor(typeof(MMVR_BasePanelFactory))]
    [CanEditMultipleObjects]

    public class MMVR_BasePanelFactoryEditor : BaseFactoryEditor {
        SerializedProperty parent;
        SerializedProperty panelProfile;
        SerializedProperty panelContainerProfile;
        SerializedProperty momentaryButtonProfile;
        SerializedProperty toggleButtonProfile;

        protected override void OnEnable() {
            base.OnEnable();
            parent = serializedObject.FindProperty("parent");
            panelProfile = serializedObject.FindProperty("panelProfile");
            panelContainerProfile = serializedObject.FindProperty("panelContainerProfile");
            momentaryButtonProfile = serializedObject.FindProperty("momentaryButtonProfile");
            toggleButtonProfile = serializedObject.FindProperty("toggleButtonProfile");
            panelProfile = serializedObject.FindProperty("panelProfile");
        }

        protected override void BuildGenerateButton() {
            // Take out this if statement to set the value using setter when ever you change it in the inspector.
            // But then it gets called a couple of times when ever inspector updates
            // By having a button, you can control when the value goes through the setter and getter, your self.
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(MMVR_BasePanelFactory)) {
                    MMVR_BasePanelFactory factory = (MMVR_BasePanelFactory)target;
                    factory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(parent);
            EditorGUILayout.PropertyField(panelProfile);
            EditorGUILayout.PropertyField(panelContainerProfile);
            EditorGUILayout.PropertyField(momentaryButtonProfile);
            EditorGUILayout.PropertyField(toggleButtonProfile);
        }
    }
}