using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;

namespace MMVR.Interact {
    public class BoxSelectionDefaultTriggerable : PrimitiveBaseDefaultTriggerable {
        protected override GameObject GetPrimitivePrefab() {
            return Meshes.boxSelectionPrefab;
        }

        protected override void BuildPrimitiveController() {
            if (primitiveControllerInstance == null) {
                BuildPrimitiveController<BoxSelectionController>();
                MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
                BoxSelectionController boxSelectionController = primitiveControllerInstance.GetComponent<BoxSelectionController>();
                boxSelectionController.realMesh = mesh;
                boxSelectionController.CreateMesh();
            }
        }
    }
}