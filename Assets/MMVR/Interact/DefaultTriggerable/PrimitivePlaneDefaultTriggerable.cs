using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class PrimitivePlaneDefaultTriggerable : PrimitiveBaseDefaultTriggerable {
        protected override GameObject GetPrimitivePrefab() {
            return Meshes.primitivePlanePrefab;
        }

        protected override void BuildPrimitiveController() {
            BuildPrimitiveController<PrimitivePlaneController>();
        }
    }
}