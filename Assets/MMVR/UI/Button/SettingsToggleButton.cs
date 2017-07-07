using UnityEngine;
using CreateThis.VR.UI.Interact;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CreateThis.VR.UI.Button;
using MeshEngine;

namespace MMVR.UI.Button {
    public class SettingsToggleButton : ToggleButton {
        public Settings.ToggleType toggleType;

        protected override void ClickHandler(Transform controller, int controllerIndex) {
            Settings.SetToggle(toggleType, On);
        }

        private void SettingsChanged() {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying) {
#endif
                if (Settings.GetToggle(toggleType)) {
                    GetComponent<Selectable>().SetStickySelected(true);
                    if (On != true) On = true;
                } else {
                    GetComponent<Selectable>().SetStickySelected(false);
                    if (On != false) On = false;
                }
                GetComponent<Selectable>().SetSelected(GetComponent<Selectable>().selected);
#if UNITY_EDITOR
            }
#endif
        }

        // Use this for initialization
        void Start() {
            Initialize();
            Settings.OnSettingsChanged += SettingsChanged;
            SettingsChanged();
        }
    }
}
