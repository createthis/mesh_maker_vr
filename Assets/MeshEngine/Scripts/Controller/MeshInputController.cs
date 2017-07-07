using System.Collections.Generic;
using UnityEngine;
using CreateThis.VR.UI.Interact;

namespace MeshEngine.Controller {
    public class MeshInputController : Grabbable {
        class DragData {
            public int controllerIndex;
            public Vector3 scalePoint;
            public Vector3 dragPoint;
            public Vector3 preDragPosition;
            public Vector3 currentPosition;
            public Quaternion lastControllerRotation;
            public Transform transform;
        }

        public Mesh mesh;

        private List<DragData> drags = new List<DragData>();
        private float scalePercentage;
        private Quaternion lastPinchScaleRotation;
        private Vector3 dragOriginalScale;

        // functions that can be called via the messaging system
        public override void OnGrabStart(Transform controller, int controllerIndex) {
            base.OnGrabStart(controller, controllerIndex);
            DragStart(controller, controllerIndex);
        }

        public override void OnGrabUpdate(Transform controller, int controllerIndex) {
            foreach (DragData drag in drags) {
                DragUpdate(drag.transform, drag.controllerIndex);
            }
        }

        public override void OnGrabStop(Transform controller, int controllerIndex) {
            base.OnGrabStop(controller, controllerIndex);
            DragEnd(controller, controllerIndex);
        }

        public int DragsIndexOfControllerIndex(int controllerIndex) {
            for (int i = 0; i < drags.Count; i++) {
                DragData drag = drags[i];
                if (drag.controllerIndex == controllerIndex) {
                    return i;
                }
            }
            return -1;
        }

        public int FindOrCreateDragsIndexOfControllerIndex(int controllerIndex) {
            int index = DragsIndexOfControllerIndex(controllerIndex);
            if (index == -1) {
                DragData drag = new DragData();
                drag.controllerIndex = controllerIndex;
                drags.Add(drag);
                return drags.Count - 1;
            } else {
                return index;
            }
        }

        public void CalculateScale() {
            DragData firstDrag = drags[0];
            DragData secondDrag = drags[1];

            float distanceStart = Vector3.Distance(firstDrag.scalePoint, secondDrag.scalePoint);
            float distanceNow = Vector3.Distance(firstDrag.currentPosition, secondDrag.currentPosition);

            scalePercentage = distanceNow / distanceStart;
            transform.localScale = dragOriginalScale * scalePercentage;
        }

        private void RotateAroundTransform(DragData drag) {
            // In order to add two quaternion rotations correctly you multiply them.
            // To subtract you need to multiply by the inverse.
            Vector3 rotationDelta = (drag.transform.rotation * Quaternion.Inverse(drag.lastControllerRotation)).eulerAngles;
            Vector3 oldPosition = transform.position;
            transform.RotateAround(drag.transform.position, Vector3.right, rotationDelta.x);
            transform.RotateAround(drag.transform.position, Vector3.up, rotationDelta.y);
            transform.RotateAround(drag.transform.position, Vector3.forward, rotationDelta.z);

            if (scalePercentage != 0) {
                drag.preDragPosition = (transform.position + drag.preDragPosition * scalePercentage - oldPosition) / scalePercentage;
            } else {
                drag.preDragPosition = transform.position + drag.preDragPosition - oldPosition;
            }
            drag.lastControllerRotation = drag.transform.rotation;
        }

        public void DragStart(Transform controller, int controllerIndex) {
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("DragStart controllerIndex=" + controllerIndex + ",index=" + index+",controller.position="+controller.position);

            if (index > 1) return; // only two drag points allowed at a time.

            drags[index].currentPosition = controller.position;
            drags[index].preDragPosition = transform.position;
            drags[index].dragPoint = controller.position;
            drags[index].transform = controller;
            drags[index].lastControllerRotation = controller.rotation;
            dragOriginalScale = transform.localScale;

            if (drags.Count == 1) { // drag
                transform.position = controller.position - drags[index].dragPoint + drags[index].preDragPosition;
            } else if (drags.Count == 2) { // scale and drag
                mesh.StartScale();

                drags[0].preDragPosition = transform.position;
                drags[0].dragPoint = drags[0].currentPosition;
                drags[0].scalePoint = drags[0].currentPosition;
                drags[1].scalePoint = drags[1].currentPosition;
                CalculateScale();
                transform.position = drags[0].currentPosition - drags[0].dragPoint * scalePercentage + drags[0].preDragPosition * scalePercentage;
            }
        }

        public void DragUpdate(Transform controller, int controllerIndex) {
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("DragUpdate controllerIndex=" + controllerIndex + ",index=" + index+",drags.Count="+drags.Count+",scalePercentage="+scalePercentage);

            if (index > 1) return; // only two drag points allowed at a time.
            drags[index].currentPosition = controller.position;

            if (drags.Count == 1) { // drag
                if (scalePercentage != 0) {
                    transform.position = controller.position - drags[index].dragPoint * scalePercentage + drags[index].preDragPosition * scalePercentage;
                } else {
                    transform.position = controller.position - drags[index].dragPoint + drags[index].preDragPosition;
                }
                RotateAroundTransform(drags[index]);
            } else if (drags.Count == 2) { // scale and drag
                CalculateScale();
                RotateAroundTransform(drags[0]);
                transform.position = drags[0].currentPosition - drags[0].dragPoint * scalePercentage + drags[0].preDragPosition * scalePercentage;
            }
        }

        public void DragEnd(Transform controller, int controllerIndex) {
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("DragEnd controllerIndex="+controllerIndex+",index=" + index + ",controller.position=" + controller.position);

            if (index > 1) {
                Debug.Log("DragEnd index > 1 THIS SHOULD NEVER HAPPEN index=" + index);
                return; // only two drag points allowed at a time.
            }
            drags[index].currentPosition = controller.position;
            int dragCount = drags.Count;

            if (drags.Count == 1) { // drag
                if (scalePercentage != 0) {
                    transform.position = controller.position - drags[index].dragPoint * scalePercentage + drags[index].preDragPosition * scalePercentage;
                } else {
                    transform.position = controller.position - drags[index].dragPoint + drags[index].preDragPosition;
                }
                RotateAroundTransform(drags[index]);
                scalePercentage = 0;
            } else if (drags.Count == 2) { // scale and drag
                CalculateScale();
                RotateAroundTransform(drags[0]);
                transform.position = drags[0].currentPosition - drags[0].dragPoint * scalePercentage + drags[0].preDragPosition * scalePercentage;

                mesh.EndScale();
            }

            //Debug.Log("DragEnd removing index=" + index);
            if (dragCount == 2 && index == 0) {
                // This is a bit of cheating to ensure the object doesn't jump if the user lets go with their first drag first.
                // FIXME: This shouldn't be necessary, but I clearly have some screwy code somewhere.
                Transform otherController = drags[1].transform;
                int otherControllerIndex = drags[1].controllerIndex;
                drags.RemoveAt(1);
                drags.RemoveAt(0);
                scalePercentage = 0;
                DragStart(otherController, otherControllerIndex);
            } else {
                drags.RemoveAt(index);
            }
        }

        // Use this for initialization
        void Start() {
            scalePercentage = 0;
        }

        protected override void Update() {
            if (drags.Count > 0) {
                OnGrabUpdate(controller, controllerIndex);
            }
        }
    }
}