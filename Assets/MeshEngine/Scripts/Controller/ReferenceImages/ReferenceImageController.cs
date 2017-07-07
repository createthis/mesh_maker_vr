using System.Collections.Generic;
using UnityEngine;
using MeshEngine.UnityEvents;

namespace MeshEngine.Controller {
    public class ReferenceImageController : MonoBehaviour {
        class DragData {
            public int controllerIndex;
            public Vector3 localCurrentPosition;
            public Vector3 scalePoint;
            public Vector3 dragPoint;
            public Vector3 preDragPosition;
            public Vector3 currentPosition;
        }

        public ReferenceImageEvent onDragEnd;
        public GameObject dragAnchorPrefab;

        private Vector3 dragAnchorOffset = new Vector3(0.04f, 0.5f, 0);
        private List<DragData> drags = new List<DragData>();
        private float scalePercentage;
        private Vector3 dragOriginalScale;
        private GameObject dragAnchorInstance;
        private int dragAnchorControllerIndex = 4949;

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

        public Vector3 IgnoreAxis(Vector3 newPosition, Vector3 oldPosition, string axis) {
            switch (axis) {
                case "y":
                    return new Vector3(newPosition.x, 0, newPosition.z);
                default:
                    return Vector3.zero;
            }
        }

        public void PlaceDragAnchor(Transform controller, int controllerIndex) {
            //if (Settings.referenceImagesLocked) return;
            //Debug.Log("PlaceDragAnchor controller=" + controller + ",controllerIndex=" + controllerIndex);
            if (dragAnchorInstance == null) {
                dragAnchorInstance = Instantiate(dragAnchorPrefab, controller.position, Quaternion.identity);
                dragAnchorInstance.GetComponent<DragAnchorController>().referenceImageController = this;
                dragAnchorInstance.transform.parent = transform;
                Vector3 newPosition = IgnoreAxis(transform.InverseTransformPoint(controller.position), transform.localPosition, "y");
                dragAnchorInstance.transform.localPosition = newPosition + dragAnchorOffset;
                dragAnchorInstance.transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0));
                dragAnchorInstance.GetComponent<DragAnchorController>().SetControllerTransform(controller);
                DragStart(controller, dragAnchorControllerIndex);
            } else {
                Vector3 newPosition = IgnoreAxis(transform.InverseTransformPoint(controller.position), transform.localPosition, "y");
                dragAnchorInstance.transform.localPosition = newPosition + dragAnchorOffset;
                DragEnd(controller, dragAnchorControllerIndex);
                dragAnchorInstance.GetComponent<DragAnchorController>().SetControllerTransform(controller);
                DragStart(controller, dragAnchorControllerIndex);
            }
        }

        public void RemoveDragAnchor() {
            DragAnchorController.StoreTransform controllerTransform = dragAnchorInstance.GetComponent<DragAnchorController>().controllerTransform;
            dragAnchorInstance.transform.position = controllerTransform.position;
            dragAnchorInstance.transform.rotation = controllerTransform.rotation;
            dragAnchorInstance.transform.localScale = controllerTransform.localScale;

            DragEnd(dragAnchorInstance.transform, dragAnchorControllerIndex);
            Destroy(dragAnchorInstance);
        }

        public void CalculateScale() {
            DragData firstDrag = drags[0];
            DragData secondDrag = drags[1];

            float distanceStart = Vector3.Distance(firstDrag.scalePoint, secondDrag.scalePoint);
            float distanceNow = Vector3.Distance(firstDrag.localCurrentPosition, secondDrag.localCurrentPosition);

            scalePercentage = distanceNow / distanceStart;
            transform.localScale = dragOriginalScale * scalePercentage;
        }

        public void DragStart(Transform controller, int controllerIndex) {
            //if (Settings.referenceImagesLocked) return;
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("DragStart controllerIndex=" + controllerIndex + ",index=" + index+",controller.position="+controller.position);

            if (index > 1) return; // only two drag points allowed at a time.

            drags[index].currentPosition = controller.position;
            drags[index].preDragPosition = transform.localPosition;
            drags[index].dragPoint = GetLocalPositionFromWorldPosition(controller.position);
            drags[index].localCurrentPosition = GetLocalPositionFromWorldPosition(controller.position);
            dragOriginalScale = transform.localScale;

            if (drags.Count == 1) { // drag
                transform.localPosition = GetLocalPositionFromWorldPosition(controller.position) - drags[index].dragPoint + drags[index].preDragPosition;
            } else if (drags.Count == 2) { // scale and drag
                drags[0].preDragPosition = transform.localPosition;
                drags[0].dragPoint = GetLocalPositionFromWorldPosition(drags[0].currentPosition);
                drags[0].scalePoint = GetLocalPositionFromWorldPosition(drags[0].currentPosition);
                drags[1].scalePoint = GetLocalPositionFromWorldPosition(drags[1].currentPosition);
                CalculateScale();
                transform.localPosition = GetLocalPositionFromWorldPosition(drags[0].currentPosition) - drags[0].dragPoint * scalePercentage + drags[0].preDragPosition * scalePercentage;
            }
        }

        public void DragUpdate(Transform controller, int controllerIndex) {
            //if (Settings.referenceImagesLocked) return;
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("DragUpdate controllerIndex=" + controllerIndex + ",index=" + index);

            if (index > 1) return; // only two drag points allowed at a time.
            drags[index].currentPosition = controller.position;
            drags[index].localCurrentPosition = GetLocalPositionFromWorldPosition(controller.position);

            if (drags.Count == 1) { // drag
                if (scalePercentage != 0) {
                    transform.localPosition = GetLocalPositionFromWorldPosition(controller.position) - drags[index].dragPoint * scalePercentage + drags[index].preDragPosition * scalePercentage;
                } else {
                    transform.localPosition = GetLocalPositionFromWorldPosition(controller.position) - drags[index].dragPoint + drags[index].preDragPosition;
                }
            } else if (drags.Count == 2) { // scale and drag
                CalculateScale();
                transform.localPosition = GetLocalPositionFromWorldPosition(drags[0].currentPosition) - drags[0].dragPoint * scalePercentage + drags[0].preDragPosition * scalePercentage;
            }
        }

        public void DragEnd(Transform controller, int controllerIndex) {
            //if (Settings.referenceImagesLocked) return;
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("DragEnd controllerIndex="+controllerIndex+",index=" + index + ",controller.position=" + controller.position);

            if (index > 1) {
                Debug.Log("DragEnd index > 1 THIS SHOULD NEVER HAPPEN index=" + index);
                return; // only two drag points allowed at a time.
            }
            drags[index].currentPosition = controller.position;
            drags[index].localCurrentPosition = GetLocalPositionFromWorldPosition(controller.position);

            if (drags.Count == 1) { // drag
                if (scalePercentage != 0) {
                    transform.localPosition = GetLocalPositionFromWorldPosition(controller.position) - drags[index].dragPoint * scalePercentage + drags[index].preDragPosition * scalePercentage;
                } else {
                    transform.localPosition = GetLocalPositionFromWorldPosition(controller.position) - drags[index].dragPoint + drags[index].preDragPosition;
                }
                onDragEnd.Invoke(transform.localPosition, transform.localScale);
                scalePercentage = 0;
            } else if (drags.Count == 2) { // scale and drag
                CalculateScale();
                transform.localPosition = GetLocalPositionFromWorldPosition(drags[0].currentPosition) - drags[0].dragPoint * scalePercentage + drags[0].preDragPosition * scalePercentage;
            }

            //Debug.Log("DragEnd removing index=" + index);
            drags.RemoveAt(index);
        }

        private Vector3 GetLocalPositionFromWorldPosition(Vector3 newWorldPosition) {
            //Debug.Log("UpdateLocalPositionFromWorldPosition newWorldPosition=" + newWorldPosition);
            return transform.parent.InverseTransformPoint(newWorldPosition);
        }

        private void UpdateLocalPositionFromWorldPosition(Vector3 newWorldPosition) {
            transform.localPosition = GetLocalPositionFromWorldPosition(newWorldPosition);
            //Debug.Log("UpdateLocalPositionFromWorldPosition transform.localPosition=" + transform.localPosition);
        }

        // Use this for initialization
        void Start() {
            dragAnchorInstance = null;
            scalePercentage = 0;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}