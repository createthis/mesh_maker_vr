using UnityEngine;
using CreateThis.VR.UI.Interact;

namespace MeshEngine.Controller {
    public class AlignmentToolController : Triggerable {
        public Mesh mesh;
        public string type;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            if (Mode.mode == ModeType.AlignmentDelete) Delete();
        }

        public void Delete() {
            mesh.alignmentTools.alignmentTools.Remove(gameObject);
            Destroy(gameObject);
        }

        // Use this for initialization
        void Start() {

        }
    }
}