using UnityEngine;
using MeshEngine;
using CreateThis.VR.UI.Interact;

namespace MMVR.Interact {
    public abstract class DefaultTriggerable : Triggerable {
        private ModeType mode;
        protected bool triggerDown;
        protected bool modeChanged;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            mode = Mode.mode;
            HandleTriggerDown(controller, controllerIndex);
        }

        public override void OnTriggerUp(Transform controller, int controllerIndex) {
            base.OnTriggerUp(controller, controllerIndex);
            if (mode == Mode.mode) HandleTriggerUp(controller, controllerIndex);
            else Destroy(gameObject);
        }

        protected virtual void HandleTriggerDown(Transform controller, int controllerIndex) {
            // override
        }

        protected virtual void HandleTriggerUpdate() {
            // override
        }

        protected virtual void HandleTriggerUp(Transform controller, int controllerIndex) {
            // override
        }

        protected virtual void ModeChanged() {
            modeChanged = true;
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        protected override void Update() {
            base.Update();
            if (triggerDown && mode == Mode.mode) {
                HandleTriggerUpdate();
            }
            if (mode != Mode.mode && !modeChanged) {
                ModeChanged();
            }
        }
    }
}