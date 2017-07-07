using UnityEngine;
using CreateThis.Unity;
using MMVR.UI.Button;

namespace MMVR.Factory.UI.Button {
    public class FileManagerSaveButtonFactory : FileManagerBaseButtonFactory {
        protected override void AddButton(GameObject target, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            FileManagerSaveButton button = Undoable.AddComponent<FileManagerSaveButton>(target);
            PopulateButton(button, audioSourceDown, audioSourceUp);
        }
    }
}