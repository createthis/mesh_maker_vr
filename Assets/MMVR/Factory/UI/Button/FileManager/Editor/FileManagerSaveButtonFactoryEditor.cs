using UnityEngine;
using UnityEditor;
using CreateThis.Factory.VR.UI.Button;

namespace MMVR.Factory.UI.Button {
    [CustomEditor(typeof(FileManagerSaveButtonFactory))]
    [CanEditMultipleObjects]
    public class FileManagerSaveButtonFactoryEditor : MomentaryButtonFactoryEditor {
        SerializedProperty fileManager;

        protected override void OnEnable() {
            base.OnEnable();
            fileManager = serializedObject.FindProperty("fileManager");
        }

        protected override void BuildGenerateButton() {
            if (GUILayout.Button("Generate")) {
                if (target.GetType() == typeof(FileManagerSaveButtonFactory)) {
                    FileManagerSaveButtonFactory buttonFactory = (FileManagerSaveButtonFactory)target;
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