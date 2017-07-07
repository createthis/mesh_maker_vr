using UnityEngine;
using UnityEditor;
using CreateThis.Factory.VR.UI.Button;

namespace MMVR.Factory.UI.Button {
    [CustomEditor(typeof(ResetButtonFactory))]
    [CanEditMultipleObjects]
    public class ResetButtonFactoryEditor : MomentaryButtonFactoryEditor {
        SerializedProperty mode;

        protected override void OnEnable() {
            base.OnEnable();
        }

        protected override void BuildGenerateButton() {
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(ResetButtonFactory)) {
                    ResetButtonFactory buttonFactory = (ResetButtonFactory)target;
                    buttonFactory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
        }
    }
}
