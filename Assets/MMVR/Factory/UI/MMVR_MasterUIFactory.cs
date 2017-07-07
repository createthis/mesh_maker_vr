using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif
using CreateThis.Unity;
using CreateThis.Factory;
using CreateThis.Factory.VR.UI;
using CreateThis.Factory.VR.UI.File;
using CreateThis.VR.UI;
using CreateThis.VR.UI.File;
using CreateThis.VR.UI.Panel;
using CreateThis.VR.UI.Controller;
using MMVR.Factory.UI.Panel;

namespace MMVR.Factory.UI {
    public class MMVR_MasterUIFactory : BaseFactory {
        public GameObject parent;
        public SkyboxManager skyboxManager;
        public TouchPadMenuController touchPadMenuController;
        public FileManager fileManager;
        public Vector3 unsavedPanelLocalPosition = new Vector3(-1.06f, 0, 0);
        public Vector3 notificationPanelLocalPosition = new Vector3(-2.06f, 0, 0);
        public Vector3 fileOpenLocalPosition = new Vector3(-1.437f, 0, 0);
        public Vector3 fileSaveAsLocalPosition = new Vector3(-1.046f, 0, 0);
        public Vector3 settingsLocalPosition = new Vector3(-0.683f, -0.2091304f, 0);
        public Vector3 toolsLocalPosition = new Vector3(-0.3367146f, 0.3599027f, 0);
        public Vector3 keyboardLocalPosition = new Vector3(0.357f, 0, 0);

        private GameObject disposable;
        private GameObject keyboardInstance;
        private Keyboard keyboard;
        private GameObject unsavedPanelInstance;
        private GameObject notificationPanelInstance;
        private GameObject fileSaveAsInstance;
        private FileSaveAs fileSaveAs;
        private GameObject fileOpenInstance;
        private FileOpen fileOpen;
        private GameObject toolsInstance;
        private GameObject settingsInstance;

        private GameObject CreateKeyboard() {
            KeyboardFactory factory = Undoable.AddComponent<KeyboardFactory>(disposable);
            factory.parent = parent;
            GameObject panel = factory.Generate();
            keyboardInstance = panel;
            Vector3 localPosition = keyboardInstance.transform.localPosition;
            localPosition.x = keyboardLocalPosition.x;
            keyboardInstance.transform.localPosition = localPosition;
            keyboard = keyboardInstance.GetComponent<Keyboard>();
            return panel;
        }

        private GameObject CreateNotificationPanel() {
            MMVR_NotificationPanelFactory factory = Undoable.AddComponent<MMVR_NotificationPanelFactory>(disposable);
            factory.parent = parent;
            factory.fileManager = fileManager;
            GameObject panel = factory.Generate();
            notificationPanelInstance = panel;
            notificationPanelInstance.transform.localPosition = notificationPanelLocalPosition;
            fileManager.notificationPanel = notificationPanelInstance.GetComponent<NotificationPanel>();
            return panel;
        }

        private GameObject CreateUnsavedPanel() {
            MMVR_UnsavedPanelFactory factory = Undoable.AddComponent<MMVR_UnsavedPanelFactory>(disposable);
            factory.parent = parent;
            factory.fileManager = fileManager;
            GameObject panel = factory.Generate();
            unsavedPanelInstance = panel;
            unsavedPanelInstance.transform.localPosition = unsavedPanelLocalPosition;
            fileManager.unsavedPanel = unsavedPanelInstance.GetComponent<StandardPanel>();
            return panel;
        }

        private GameObject CreateFileSaveAs() {
            FileSaveAsFactory factory = Undoable.AddComponent<FileSaveAsFactory>(disposable);
            factory.parent = parent;
            factory.keyboard = keyboard;
            GameObject panel = factory.Generate();
            fileSaveAsInstance = panel;
            GameObject fileSaveAsContainer = panel.transform.parent.gameObject;
            fileSaveAsContainer.transform.localPosition = fileSaveAsLocalPosition;
            fileSaveAs = fileSaveAsInstance.transform.Find("DrivesPanel").GetComponent<FileSaveAs>();
            fileManager.saveAsPanel = fileSaveAs;


#if UNITY_EDITOR
            var onSaveAs = fileSaveAs.onSaveAs;
            for (int i = 0; i < onSaveAs.GetPersistentEventCount(); i++) {
                UnityEventTools.RemovePersistentListener(onSaveAs, 0);
            }
            UnityEventTools.AddPersistentListener(onSaveAs, fileManager.SaveAs);
            fileSaveAs.onSaveAs = onSaveAs;
#endif

            return panel;
        }

        private GameObject CreateFileOpen() {
            FileOpenFactory factory = Undoable.AddComponent<FileOpenFactory>(disposable);
            factory.parent = parent;
            GameObject panel = factory.Generate();
            fileOpenInstance = panel;
            GameObject fileOpenContainer = panel.transform.parent.gameObject;
            fileOpenContainer.transform.localPosition = fileOpenLocalPosition;
            fileOpen = fileOpenInstance.transform.Find("DrivesPanel").GetComponent<FileOpen>();

#if UNITY_EDITOR
            var onOpen = fileOpen.onOpen;
            for (int i = 0; i < onOpen.GetPersistentEventCount(); i++) {
                UnityEventTools.RemovePersistentListener(onOpen, 0);
            }
            UnityEventTools.AddPersistentListener(onOpen, fileManager.Open);
            fileOpen.onOpen = onOpen;
#endif

            return panel;
        }

        private void WireUpSave() {
#if UNITY_EDITOR
            var touchPadButtons = touchPadMenuController.touchPadButtons;
            for (int i = 0; i < touchPadButtons[1].onSelected.GetPersistentEventCount(); i++) {
                UnityEventTools.RemovePersistentListener(touchPadButtons[1].onSelected, 0);
            }
            UnityEventTools.AddPersistentListener(touchPadButtons[1].onSelected, fileManager.SaveFromUnityEvent);
            touchPadMenuController.touchPadButtons = touchPadButtons;
#endif
        }

        private GameObject CreateToolsPanel() {
            MMVR_ToolsPanelFactory factory = Undoable.AddComponent<MMVR_ToolsPanelFactory>(disposable);
            factory.parent = parent;
            factory.fileOpen = fileOpen;
            factory.fileSaveAs = fileSaveAs;
            GameObject panel = factory.Generate();
            toolsInstance = panel;
            Vector3 localPosition = toolsInstance.transform.localPosition;
            localPosition.x = toolsLocalPosition.x;
            toolsInstance.transform.localPosition = localPosition;

#if UNITY_EDITOR
            var touchPadButtons = touchPadMenuController.touchPadButtons;
            for (int i = 0; i < touchPadButtons[0].onSelected.GetPersistentEventCount(); i++) {
                UnityEventTools.RemovePersistentListener(touchPadButtons[0].onSelected, 0);
            }
            UnityEventTools.AddPersistentListener(touchPadButtons[0].onSelected, toolsInstance.GetComponent<StandardPanel>().ToggleVisible);
            touchPadMenuController.touchPadButtons = touchPadButtons;
#endif
            return panel;
        }

        private GameObject CreateSettingsPanel() {
            MMVR_SettingsPanelFactory factory = Undoable.AddComponent<MMVR_SettingsPanelFactory>(disposable);
            factory.parent = parent;
            factory.skyboxManager = skyboxManager;
            GameObject panel = factory.Generate();
            settingsInstance = panel;
            Vector3 localPosition = settingsInstance.transform.localPosition;
            localPosition.x = settingsLocalPosition.x;
            settingsInstance.transform.localPosition = localPosition;

#if UNITY_EDITOR
            var touchPadButtons = touchPadMenuController.touchPadButtons;
            for (int i = 0; i < touchPadButtons[0].onSelected.GetPersistentEventCount(); i++) {
                UnityEventTools.RemovePersistentListener(touchPadButtons[2].onSelected, 0);
            }
            UnityEventTools.AddPersistentListener(touchPadButtons[2].onSelected, settingsInstance.GetComponent<StandardPanel>().ToggleVisible);
            touchPadMenuController.touchPadButtons = touchPadButtons;
#endif
            return panel;
        }

        private void CreateDisposable() {
            if (disposable) return;
            disposable = EmptyChild(parent, "disposable");
        }

        private void CleanParent() {
            var children = new List<GameObject>();
            foreach (Transform child in parent.transform) children.Add(child.gameObject);
#if UNITY_EDITOR
            children.ForEach(child => Undo.DestroyObjectImmediate(child));
#else
            children.ForEach(child => Destroy(child));
#endif
        }

        public override GameObject Generate() {
            base.Generate();

#if UNITY_EDITOR
            Undo.SetCurrentGroupName("MasterFactory Generate");
            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(this, "MasterFactory state");
#endif
            CleanParent();

            CreateDisposable();
            CreateKeyboard();
            CreateUnsavedPanel();
            CreateNotificationPanel();
            CreateFileSaveAs();
            CreateFileOpen();
            CreateToolsPanel();
            CreateSettingsPanel();
            WireUpSave();

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(disposable);
            Undo.CollapseUndoOperations(group);
#else
            Destroy(disposable);
#endif
            return parent;
        }
    }
}