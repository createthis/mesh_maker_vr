using CreateThis.VR.UI.Interact;

namespace MeshEngine.Controller {
    public abstract class PrimitiveBaseController : Triggerable {
        public Mesh mesh;

        protected int triggerCount = 0;
    }
}