using UnityEngine;
using MeshEngine;
using MeshEngine.Controller;
using CreateThis.VR.UI.Interact;

namespace MMVR.Interact {
    public abstract class PrimitiveBaseDefaultTriggerable : DefaultTriggerable {
        protected GameObject primitiveControllerInstance;

        private bool hasInitialized = false;

        protected virtual GameObject GetPrimitivePrefab() {
            // override me
            return new GameObject();
        }

        protected virtual void BuildPrimitiveController() {
            // Override and call BuildPrimitiveController<T>() here
        }

        protected void BuildPrimitiveController<T>() where T : PrimitiveBaseController {
            if (primitiveControllerInstance == null) {
                primitiveControllerInstance = Instantiate(GetPrimitivePrefab(), controller.position, controller.rotation);
                MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
                primitiveControllerInstance.transform.parent = mesh.gameObject.transform;
                primitiveControllerInstance.GetComponent<T>().mesh = mesh;
                hasInitialized = true;
            }
        }

        private void PrimitiveControllerOnTriggerDown(Transform controller, int controllerIndex) {
            Triggerable triggerable = primitiveControllerInstance.GetComponent<Triggerable>();
            triggerable.OnTriggerDown(controller, controllerIndex);
        }

        protected override void HandleTriggerDown(Transform controller, int controllerIndex) {
            BuildPrimitiveController();
            PrimitiveControllerOnTriggerDown(controller, controllerIndex);
        }

        protected override void HandleTriggerUp(Transform controller, int controllerIndex) {
        }

        protected override void Update() {
            base.Update();
            if (hasInitialized && primitiveControllerInstance == null) Destroy(gameObject); // self destruct if spawned controller has self destructed
        }
    }
}