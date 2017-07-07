using UnityEngine;
using CreateThis.Factory.VR.UI.Button;
using MMVR.UI.Button;

namespace MMVR.Factory.UI.Button {
    public abstract class FileManagerBaseButtonFactory : MomentaryButtonFactory {
        public FileManager fileManager;

        public void PopulateButton(FileManagerBaseButton button, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            base.PopulateButton(button, audioSourceDown, audioSourceUp);
            button.clickOnTriggerExit = true;
            button.fileManager = fileManager;
        }
    }
}