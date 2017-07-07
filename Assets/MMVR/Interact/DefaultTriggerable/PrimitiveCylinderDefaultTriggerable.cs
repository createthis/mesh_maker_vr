using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class PrimitiveCylinderDefaultTriggerable : PrimitiveBaseDefaultTriggerable {
        protected override GameObject GetPrimitivePrefab() {
            return Meshes.primitiveCylinderPrefab;
        }

        protected override void BuildPrimitiveController() {
            BuildPrimitiveController<PrimitiveCylinderController>();
        }
    }
}