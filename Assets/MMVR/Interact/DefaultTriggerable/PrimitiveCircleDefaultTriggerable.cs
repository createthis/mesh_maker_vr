using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class PrimitiveCircleDefaultTriggerable : PrimitiveBaseDefaultTriggerable {
        protected override GameObject GetPrimitivePrefab() {
            return Meshes.primitiveCirclePrefab;
        }

        protected override void BuildPrimitiveController() {
            BuildPrimitiveController<PrimitiveCircleController>();
        }
    }
}