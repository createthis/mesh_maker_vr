using UnityEngine;
using CreateThis.Unity;
using CreateThis.Factory.VR.UI.Button;
using MMVR.UI.Button;
using MeshEngine;

namespace MMVR.Factory.UI.Button {
    public class ModeButtonFactory : ToggleButtonFactory {
        public ModeType mode;

        public void PopulateButton(ModeButton button, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            base.PopulateButton(button, audioSourceDown, audioSourceUp);
            button.clickOnTriggerExit = true;
            button.mode = mode;
        }

        protected override void AddButton(GameObject target, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            ModeButton button = Undoable.AddComponent<ModeButton>(target);
            PopulateButton(button, audioSourceDown, audioSourceUp);
        }
    }
}
