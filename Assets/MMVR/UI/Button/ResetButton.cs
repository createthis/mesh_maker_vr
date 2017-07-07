using UnityEngine;
using CreateThis.VR.UI.Interact;
#if UNITY_EDITOR
using UnityEditor;
#endif
using CreateThis.VR.UI.Button;
using MeshEngine;

namespace MMVR.UI.Button {
    public class ResetButton : MomentaryButton {
        protected override void ClickHandler(Transform controller, int controllerIndex) {
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            mesh.ResetTransform();
        }
    }
}