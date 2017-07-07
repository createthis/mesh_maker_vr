using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if VRTK
using CreateThis.VRTK;
#endif
using CreateThis.Unity;
using CreateThis.VR.UI.Panel;
using CreateThis.Factory.VR.UI.Container;
using MMVR.Factory.UI.Button;

namespace MMVR.Factory.UI.Panel {
    public class MMVR_NotificationPanelFactory : MMVR_BasePanelFactory {
        public FileManager fileManager;

        private GameObject notificationPanelInstance;
        private NotificationPanel notificationPanel;
        private TextMesh notificationLabel;


        protected GameObject NotificationPanel(GameObject parent, string name) {
            PanelContainerFactory factory = Undoable.AddComponent<PanelContainerFactory>(disposable);
            factory.parent = parent;
            factory.containerName = name;
            factory.panelContainerProfile = panelContainerProfile;
            GameObject panel = factory.Generate();

            NotificationPanel standardPanel = Undoable.AddComponent<NotificationPanel>(panel);
            standardPanel.grabTarget = panel.transform;
            standardPanel.panelProfile = panelProfile;

#if VRTK
            CreateThis_VRTK_Interactable interactable = Undoable.AddComponent<CreateThis_VRTK_Interactable>(panel);
            CreateThis_VRTK_GrabAttach grabAttach = Undoable.AddComponent<CreateThis_VRTK_GrabAttach>(panel);
            interactable.isGrabbable = true;
            interactable.grabAttachMechanicScript = grabAttach;
#endif

            Rigidbody rigidbody = Undoable.AddComponent<Rigidbody>(panel);
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            return panel;
        }

        private void CreateDisposable(GameObject parent) {
            if (disposable) return;
            disposable = EmptyChild(parent, "disposable");
        }

        private void CreateNotificationPanel(GameObject parent) {
            notificationPanelInstance = NotificationPanel(parent, "NotificationPanelInstance");
            notificationPanel = notificationPanelInstance.GetComponent<NotificationPanel>();
            GameObject column = Column(notificationPanelInstance);

            GameObject row = Row(column, "LabelRow", TextAlignment.Center);
            GameObject label = Label(row, "Label", "Notifications Go Here");
            notificationLabel = label.GetComponent<TextMesh>();
            notificationPanel.notificationLabel = notificationLabel;
        }

        public override GameObject Generate() {
            base.Generate();

#if UNITY_EDITOR
            Undo.SetCurrentGroupName("MMVR_NotificationPanelFactory Generate");
            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(this, "MMVR_NotificationPanelFactory state");
#endif
            CreateDisposable(parent);
            CreateNotificationPanel(parent);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(disposable);
            Undo.CollapseUndoOperations(group);
#else
            Destroy(disposable);
#endif
            return notificationPanelInstance;
        }
    }
}