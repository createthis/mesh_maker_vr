using UnityEngine;
using UnityEditor;
using CreateThis.Factory.VR.UI.Button;

namespace MMVR.Factory.UI.Button {
    [CustomEditor(typeof(FileManagerDiscardButtonFactory))]
    [CanEditMultipleObjects]
    public class FileManagerDiscardButtonFactoryEditor : MomentaryButtonFactoryEditor {
        SerializedProperty fileManager;

        protected override void OnEnable() {
            base.OnEnable();
            fileManager = serializedObject.FindProperty("fileManager");
        }

        protected override void BuildGenerateButton() {
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(FileManagerDiscardButtonFactory)) {
                    FileManagerDiscardButtonFactory buttonFactory = (FileManagerDiscardButtonFactory)target;
                    buttonFactory.Generate();
                }
            }
        }

        protected override void AdditionalProperties() {
            base.AdditionalProperties();
            EditorGUILayout.PropertyField(fileManager);
        }
    }
}