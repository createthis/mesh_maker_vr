using UnityEngine;
using CreateThis.VR.UI.Interact;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CreateThis.VR.UI.Button;
using MeshEngine;

namespace MMVR.UI.Button {
    public class ModeButton : ToggleButton {
        public ModeType mode;

        protected override void ClickHandler(Transform controller, int controllerIndex) {
            Mode.SetMode(mode);
        }

        private void ModeChanged() {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying) {
#endif
                if (Mode.mode == mode) {
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
            Mode.OnModeChanged += ModeChanged;
            ModeChanged();
        }
    }
}
