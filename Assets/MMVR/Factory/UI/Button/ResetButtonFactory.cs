using UnityEngine;
using CreateThis.Unity;
using CreateThis.Factory.VR.UI.Button;
using MMVR.UI.Button;
using MeshEngine;

namespace MMVR.Factory.UI.Button {
    public class ResetButtonFactory : MomentaryButtonFactory {
        public void PopulateButton(ResetButton button, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            base.PopulateButton(button, audioSourceDown, audioSourceUp);
            button.clickOnTriggerExit = false;
        }

        protected override void AddButton(GameObject target, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            ResetButton button = Undoable.AddComponent<ResetButton>(target);
            PopulateButton(button, audioSourceDown, audioSourceUp);
        }
    }
}
