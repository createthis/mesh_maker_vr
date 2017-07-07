using UnityEngine;

namespace MMVR.UI.Button {
    public class FileManagerDiscardButton : FileManagerBaseButton {
        protected override void ClickHandler(Transform controller, int controllerIndex) {
            fileManager.OpenWithoutSave();
        }
    }
}
