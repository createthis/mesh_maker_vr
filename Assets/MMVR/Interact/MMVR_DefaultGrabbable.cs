using UnityEngine;
using CreateThis.VR.UI.Interact;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class MMVR_DefaultGrabbable : Grabbable {
        public override void OnGrabStart(Transform controller, int controllerIndex) {
            base.OnGrabStart(controller, controllerIndex);
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            mesh.GetComponent<MeshInputController>().OnGrabStart(controller, controllerIndex);
        }

        public override void OnGrabUpdate(Transform controller, int controllerIndex) {
            base.OnGrabUpdate(controller, controllerIndex);
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            mesh.GetComponent<MeshInputController>().OnGrabUpdate(controller, controllerIndex);
        }

        public override void OnGrabStop(Transform controller, int controllerIndex) {
            base.OnGrabStop(controller, controllerIndex);
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            mesh.GetComponent<MeshInputController>().OnGrabStop(controller, controllerIndex);
        }

        // Use this for initialization
        void Start() {

        }
    }
}