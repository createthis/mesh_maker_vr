using UnityEngine;
using UnityEditor;
using CreateThis.Factory.VR.UI.Button;

namespace MMVR.Factory.UI.Button {
    [CustomEditor(typeof(ModeButtonFactory))]
    [CanEditMultipleObjects]
    public class ModeButtonFactoryEditor : ToggleButtonFactoryEditor {
        SerializedProperty mode;

        protected override void OnEnable() {
            base.OnEnable();
            mode = serializedObject.FindProperty("mode");
        }

        protected override void BuildGenerateButton() {
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(ModeButtonFactory)) {
                    ModeButtonFactory buttonFactory = (ModeButtonFactory)target;
                    buttonFactory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(mode);
        }
    }
}
