using UnityEngine;
using CreateThis.Unity;
using MMVR.UI.Button;

namespace MMVR.Factory.UI.Button {
    public class FileManagerDiscardButtonFactory : FileManagerBaseButtonFactory {
        protected override void AddButton(GameObject target, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            FileManagerDiscardButton button = Undoable.AddComponent<FileManagerDiscardButton>(target);
            PopulateButton(button, audioSourceDown, audioSourceUp);
        }
    }
}