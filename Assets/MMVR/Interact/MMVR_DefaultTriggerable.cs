using UnityEngine;
using MeshEngine;
using CreateThis.VR.UI.Interact;

namespace MMVR.Interact {
    public class MMVR_DefaultTriggerable : Triggerable {
        private DefaultTriggerable defaultTriggerable;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            if (defaultTriggerable == null) defaultTriggerable = GetDefaultTriggerable();
            if (defaultTriggerable != null) defaultTriggerable.OnTriggerDown(controller, controllerIndex);
        }

        public override void OnTriggerUp(Transform controller, int controllerIndex) {
            base.OnTriggerUp(controller, controllerIndex);
            if (defaultTriggerable != null) defaultTriggerable.OnTriggerUp(controller, controllerIndex);
        }

        private DefaultTriggerable Build<T>() where T : DefaultTriggerable {
            GameObject obj = new GameObject();
            obj.name = "DefaultTriggerable";
            return obj.AddComponent<T>();
        }

        private DefaultTriggerable GetDefaultTriggerable() {
            switch (Mode.mode) {
                case ModeType.Vertex:
                    return Build<VertexDefaultTriggerable>();
                case ModeType.PrimitivePlane:
                    return Build<PrimitivePlaneDefaultTriggerable>();
                case ModeType.PrimitiveBox:
                    return Build<PrimitiveBoxDefaultTriggerable>();
                case ModeType.PrimitiveCircle:
                    return Build<PrimitiveCircleDefaultTriggerable>();
                case ModeType.PrimitiveCylinder:
                    return Build<PrimitiveCylinderDefaultTriggerable>();
                case ModeType.PrimitiveSphere:
                    return Build<PrimitiveSphereDefaultTriggerable>();
                case ModeType.BoxSelect:
                    return Build<BoxSelectionDefaultTriggerable>();
                default:
                    return null;
            }
        }
    }
}