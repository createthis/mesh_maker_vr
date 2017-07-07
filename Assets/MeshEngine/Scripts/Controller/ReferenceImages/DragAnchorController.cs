using UnityEngine;

namespace MeshEngine.Controller {
    public class DragAnchorController : MonoBehaviour {
        public class StoreTransform {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 localScale;
        }

        public ReferenceImageController referenceImageController;
        public StoreTransform controllerTransform;

        public void SetControllerTransform(Transform controller) {
            controllerTransform = new StoreTransform();
            controllerTransform.position = controller.position;
            controllerTransform.localScale = controller.localScale;
            controllerTransform.rotation = controller.rotation;
        }

        public void Delete() {
            referenceImageController.RemoveDragAnchor();
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}