using UnityEngine;

namespace MMVR.UI.Button {
    public class FileManagerSaveButton : FileManagerBaseButton {
        protected override void ClickHandler(Transform controller, int controllerIndex) {
            fileManager.SaveAndOpen();
        }
    }
}
