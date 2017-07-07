using UnityEngine;
using CreateThis.Unity;
using CreateThis.Factory.VR.UI.Button;
using MMVR.UI.Button;
using MeshEngine;

namespace MMVR.Factory.UI.Button {
    public class SettingsToggleButtonFactory : ToggleButtonFactory {
        public Settings.ToggleType toggleType;

        public void PopulateButton(SettingsToggleButton button, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            base.PopulateButton(button, audioSourceDown, audioSourceUp);
            button.toggleType = toggleType;
        }

        protected override void AddButton(GameObject target, AudioSource audioSourceDown, AudioSource audioSourceUp) {
            SettingsToggleButton button = Undoable.AddComponent<SettingsToggleButton>(target);
            PopulateButton(button, audioSourceDown, audioSourceUp);
        }
    }
}
