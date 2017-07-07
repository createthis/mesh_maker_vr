using UnityEngine;
using MeshEngine;

namespace MMVR.Interact {
    public class VertexDefaultTriggerable : DefaultTriggerable {
        protected override void HandleTriggerDown(Transform controller, int controllerIndex) {
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            Vector3 position = Settings.SnapEnabled() ? Snap.WorldPosition(mesh.transform, controller.position) : controller.position;
            mesh.vertices.CreateAndAddVertexInstanceByWorldPosition(position);
        }

        protected override void HandleTriggerUp(Transform controller, int controllerIndex) {
            Destroy(gameObject);
        }
    }
}