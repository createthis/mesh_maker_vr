using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if VRTK
using CreateThis.VRTK;
#endif
using CreateThis.Unity;
using CreateThis.VR.UI.Panel;
using MMVR.Factory.UI.Button;

namespace MMVR.Factory.UI.Panel {
    public class MMVR_UnsavedPanelFactory : MMVR_BasePanelFactory {
        public FileManager fileManager;

        private GameObject toolsPanelInstance;
        private StandardPanel toolsPanel;

        protected GameObject SaveButton(StandardPanel panel, GameObject parent, string buttonText) {
            FileManagerSaveButtonFactory factory = Undoable.AddComponent<FileManagerSaveButtonFactory>(disposable);
            SetMomentaryButtonValues(factory, panel, parent);
            factory.buttonText = buttonText;
            factory.fileManager = fileManager;
            return GenerateMomentaryButtonAndSetPosition(factory);
        }

        protected GameObject DiscardButton(StandardPanel panel, GameObject parent, string buttonText) {
            FileManagerDiscardButtonFactory factory = Undoable.AddComponent<FileManagerDiscardButtonFactory>(disposable);
            SetMomentaryButtonValues(factory, panel, parent);
            factory.buttonText = buttonText;
            factory.fileManager = fileManager;
            return GenerateMomentaryButtonAndSetPosition(factory);
        }

        private void CreateDisposable(GameObject parent) {
            if (disposable) return;
            disposable = EmptyChild(parent, "disposable");
        }

        private GameObject ButtonRow(GameObject parent) {
            GameObject row = Row(parent, "ButtonRow", TextAlignment.Center);
            DiscardButton(toolsPanel, row, "Discard");
            SaveButton(toolsPanel, row, "Save");
            return row;
        }
        
        private void CreateUnsavedPanel(GameObject parent) {
            toolsPanelInstance = Panel(parent, "UnsavedPanelInstance");
            toolsPanel = toolsPanelInstance.GetComponent<StandardPanel>();
            GameObject column = Column(toolsPanelInstance);

            LabelRow(column, "You have unsaved changes.");
            LabelRow(column, "Do you want to save the changes?");

            ButtonRow(column);
        }

        public override GameObject Generate() {
            base.Generate();

#if UNITY_EDITOR
            Undo.SetCurrentGroupName("MMVR_UnsavedPanelFactory Generate");
            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(this, "MMVR_UnsavedPanelFactory state");
#endif
            CreateDisposable(parent);
            CreateUnsavedPanel(parent);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(disposable);
            Undo.CollapseUndoOperations(group);
#else
            Destroy(disposable);
#endif
            return toolsPanelInstance;
        }
    }
}