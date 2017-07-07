using System.Collections.Generic;
using UnityEngine;
using CreateThis.VR.UI.Interact;

namespace MeshEngine.Controller {
    public static class RotateAroundPivotExtensions {
        //http://answers.unity3d.com/questions/47115/vector3-rotate-around.html
        //Returns the rotated Vector3 using a Quaterion
        public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Quaternion angle) {
            // NOTE: (point - pivot) Gets a vector that points from the pivot's position to the point's.
            return angle * (point - pivot) + pivot;
        }
        //Returns the rotated Vector3 using Euler
        public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Vector3 euler) {
            return RotateAroundPivot(point, pivot, Quaternion.Euler(euler));
        }
        //Rotates the Transform's position using a Quaterion
        public static void RotateAroundPivot(this Transform me, Vector3 pivot, Quaternion angle) {
            me.position = me.position.RotateAroundPivot(pivot, angle);
        }
        //Rotates the Transform's position using Euler
        public static void RotateAroundPivot(this Transform me, Vector3 pivot, Vector3 euler) {
            me.position = me.position.RotateAroundPivot(pivot, Quaternion.Euler(euler));
        }
    }

    public class VertexController : Grabbable {
        class DragData {
            public int controllerIndex;
            public Vector3 dragPoint;
            public Vector3 preDragPosition;
            public Quaternion startTransformRotation;
            public FakeTransform transform;
        }
        public class FakeTransform {
            public Vector3 position;
            public Quaternion rotation;
        }
        private List<DragData> drags = new List<DragData>();

        public Vector3 lastPosition; // last local position of the vertex inside the mesh
        public UnityEngine.Material material;
        public UnityEngine.Material stickySelectedMaterial;
        public Mesh mesh;
        private bool stickySelected;
        private List<ModeType> draggableModes = new List<ModeType>(new ModeType[] {
            ModeType.Vertex,
            ModeType.SelectVertices,
            ModeType.PrimitiveBox,
            ModeType.PrimitiveCylinder,
            ModeType.BoxSelect
        });
        private List<ModeType> selectionDraggableModes = new List<ModeType>(new ModeType[] {
            ModeType.Vertex,
            ModeType.SelectVertices,
            ModeType.PrimitiveBox,
            ModeType.PrimitiveCylinder,
            ModeType.BoxSelect
        });
        private bool selectable = true;

        public override void OnGrabStart(Transform controller, int controllerIndex) {
            base.OnGrabStart(controller, controllerIndex);
            DragStart(controller, controllerIndex);
        }

        public override void OnGrabUpdate(Transform controller, int controllerIndex) {
            DragUpdate(controller, controllerIndex);
        }

        public override void OnGrabStop(Transform controller, int controllerIndex) {
            base.OnGrabStop(controller, controllerIndex);
            DragEnd(controller, controllerIndex);
        }

        public int DragsIndexOfControllerIndex(int controllerIndex) {
            //Debug.Log("DragsIndexOfControllerIndex["+ GetInstanceID()+"] drags.Count=" + drags.Count + ",controllerIndex=" + controllerIndex);
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
                //Debug.Log("FindOrCreateDragsIndexOfControllerIndex[" + GetInstanceID() + "] created new index controllerIndex=" + controllerIndex + ",drags.Count=" + drags.Count);
                return drags.Count - 1;
            } else {
                return index;
            }
        }

        private void RotateAroundTransform(DragData drag) {
            // In order to add two quaternion rotations correctly you multiply them.
            // To subtract you need to multiply by the inverse.
            Quaternion rotationDelta = (
                Snap.LocalRotationOfWorldRotation(mesh.transform, drag.transform.rotation) *
                Quaternion.Inverse(Snap.LocalRotationOfWorldRotation(mesh.transform, drag.startTransformRotation))
                );
            Quaternion snappedRotationDelta = Snap.RotationDelta(rotationDelta);
            transform.position = Snap.RotateAroundPivot(mesh.transform, transform.position, drag.transform.position, snappedRotationDelta);
        }

        public void DragStart(Transform controller, int controllerIndex) {
            //Debug.Log("DragStart mode=" + Mode.mode + ",stickySelected=" + stickySelected);
            if (stickySelected && selectionDraggableModes.Contains(Mode.mode)) {
                mesh.selection.BroadcastDragStart(controller, controllerIndex, gameObject);
            }
            SelectionDragStart(controller, controllerIndex, gameObject);
        }

        public FakeTransform WhichTransform(Transform controller, GameObject instanceOfOrigin) {
            Transform whichTransform = instanceOfOrigin == gameObject ? controller : instanceOfOrigin.transform;
            FakeTransform fakeTransform = new FakeTransform();
            fakeTransform.position = whichTransform.position;
            fakeTransform.rotation = controller.rotation;
            return fakeTransform;
        }

        public void SelectionDragStart(Transform controller, int controllerIndex, GameObject instanceOfOrigin, bool allowSnap = true, bool forceDrag = false) {
            FakeTransform transformToUse = WhichTransform(controller, instanceOfOrigin);
            int index = FindOrCreateDragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("SelectionDragStart[" + GetInstanceID() + "] controllerIndex=" + controllerIndex + ",index=" + index);
            if (index > 0) {
                drags.RemoveAt(index);
                return; // only one controller may drag at a time.
            }
            if (!forceDrag && !draggableModes.Contains(Mode.mode)) {
                drags.RemoveAt(index);
                return; // only allow dragging vertices in vertex mode
            }

            drags[index].preDragPosition = transform.position;
            drags[index].dragPoint = transformToUse.position;
            drags[index].transform = transformToUse;
            drags[index].startTransformRotation = controller.rotation;
            lastPosition = transform.localPosition;
            Vector3 preSnapPosition = transformToUse.position - drags[index].dragPoint + drags[index].preDragPosition;
            Vector3 position = Settings.SnapEnabled() && allowSnap ? Snap.WorldPosition(mesh.transform, preSnapPosition) : preSnapPosition;
            UpdateLocalPositionFromWorldPosition(position);
        }

        public void DragUpdate(Transform controller, int controllerIndex) {
            SelectionDragUpdate(controller, controllerIndex, gameObject);
            if (stickySelected && selectionDraggableModes.Contains(Mode.mode)) {
                mesh.selection.BroadcastDragUpdate(controller, controllerIndex, gameObject);
            }
        }

        public void SelectionDragUpdate(Transform controller, int controllerIndex, GameObject instanceOfOrigin, bool allowSnap = true) {
            FakeTransform transformToUse = WhichTransform(controller, instanceOfOrigin);
            int index = DragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("SelectionDragUpdate[" + GetInstanceID() + "] controllerIndex=" + controllerIndex + ",index=" + index);
            if (index == -1 || index > 0) return; // only one controller may drag at a time.
            drags[index].transform = transformToUse;

            Vector3 preSnapPosition = transformToUse.position - drags[index].dragPoint + drags[index].preDragPosition;
            Vector3 position = Settings.SnapEnabled() && allowSnap ? Snap.WorldPosition(mesh.transform, preSnapPosition) : preSnapPosition;
            UpdateLocalPositionFromWorldPosition(position);
            if (instanceOfOrigin != gameObject) {
                RotateAroundTransform(drags[index]);
                UpdateLocalPositionFromWorldPosition(transform.position);
            } else {
                // no rotation
            }
        }

        public void DragEnd(Transform controller, int controllerIndex) {
            SelectionDragEnd(controller, controllerIndex, gameObject);
            if (stickySelected && selectionDraggableModes.Contains(Mode.mode)) {
                mesh.selection.BroadcastDragEnd(controller, controllerIndex, gameObject);
            }
        }

        public void SelectionDragEnd(Transform controller, int controllerIndex, GameObject instanceOfOrigin, bool allowSnap = true) {
            FakeTransform transformToUse = WhichTransform(controller, instanceOfOrigin);
            int index = DragsIndexOfControllerIndex(controllerIndex);
            //Debug.Log("SelectionDragEnd[" + GetInstanceID() + "] controllerIndex=" + controllerIndex + ",index=" + index);
            if (index == -1 || index > 0) return; // only one controller may drag at a time.
            drags[index].transform = transformToUse;
            Vector3 preSnapPosition = transformToUse.position - drags[index].dragPoint + drags[index].preDragPosition;
            Vector3 position = Settings.SnapEnabled() && allowSnap ? Snap.WorldPosition(mesh.transform, preSnapPosition) : preSnapPosition;
            UpdateLocalPositionFromWorldPosition(position);
            if (instanceOfOrigin != gameObject) {
                RotateAroundTransform(drags[index]);
                UpdateLocalPositionFromWorldPosition(transform.position);
            } else {
                // no rotation
            }

            lastPosition = Vector3.zero;
            GetComponent<Selectable>().SetSelected(false);
            drags.RemoveAt(index);
        }

        void TriggerDownModeFace() {
            mesh.triangles.AddTriangleVertex(gameObject);
        }

        void TriggerDownModeDelete() {
            RemoveVertex();
        }

        public void RemoveVertex() {
            //Debug.Log("VertexController#RemoveVertex");
            mesh.vertices.RemoveByVertexInstance(gameObject);
            mesh.vertices.DeactivateAndMoveToPool(gameObject);
        }

        void TriggerDownModeSelectVertices() {
            Vertex vertex = mesh.vertices.VertexOfInstance(gameObject);
            if (mesh.selection.VertexSelected(vertex)) {
                mesh.selection.DeselectVertex(gameObject);
                return;
            }
            mesh.selection.SelectVertex(gameObject);
        }

        public void TriggerDown(Transform controller, int controllerIndex) {
            switch (Mode.mode) {
                case ModeType.Delete:
                    TriggerDownModeDelete();
                    break;
                case ModeType.Face:
                    TriggerDownModeFace();
                    break;
                case ModeType.SelectVertices:
                    TriggerDownModeSelectVertices();
                    break;
            }
        }

        public void AddToMeshController() {
            mesh.vertices.AddVertexWithInstance(transform.localPosition, gameObject);
        }

        public UnityEngine.Material GetMaterial() {
            if (stickySelected) {
                return stickySelectedMaterial;
            } else {
                return material;
            }
        }

        public void SetStickySelected(bool value) {
            stickySelected = value;
            UnityEngine.Material[] materials = new UnityEngine.Material[1];
            materials[0] = GetMaterial();
            Selectable selectable = GetComponent<Selectable>();
            selectable.unselectedMaterials = materials;
            selectable.UpdateSelectedMaterials();
            selectable.SetSelected(selectable.selected);
        }

        public void CreateSphereCollider() {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider == null) {
                sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = 0.5f;
                sphereCollider.center = Vector3.zero;
            }
        }

        public void SetSelectable(bool value) {
            selectable = value;
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (selectable) {
                if (sphereCollider == null) CreateSphereCollider();
            } else {
                if (sphereCollider != null) Destroy(sphereCollider);
            }
        }

        public void Create(Vector3 position, Mesh mesh, bool addToMesh = true) {
            //Debug.Log("VertexController#Create[" + GetInstanceID() + "] position=" + position);
            transform.localPosition = GetLocalPositionFromWorldPosition(position);
            CreateFromLocalPosition(transform.localPosition, mesh, addToMesh);
        }

        public void CreateFromLocalPosition(Vector3 localPosition, Mesh mesh, bool addToMesh = true) {
            transform.localPosition = localPosition;
            lastPosition = transform.localPosition;
            //Debug.Log("VertexController#CreateFromLocalPosition[" + GetInstanceID() + "] localPosition=" + transform.localPosition);
            this.mesh = mesh;
            if (addToMesh) AddToMeshController();
        }

        public Vector3 GetLocalPositionFromWorldPosition(Vector3 newWorldPosition) {
            //Debug.Log("UpdateLocalPositionFromWorldPosition newWorldPosition=" + newWorldPosition);
            return transform.parent.transform.InverseTransformPoint(newWorldPosition);
        }

        public void UpdateLocalPositionFromWorldPosition(Vector3 newWorldPosition) {
            transform.localPosition = GetLocalPositionFromWorldPosition(newWorldPosition);
            //Debug.Log("UpdateLocalPositionFromWorldPosition transform.localPosition=" + transform.localPosition);
            UpdatePosition(transform.localPosition);
        }

        public void UpdatePosition(Vector3 newPosition) {
            //Debug.Log("VertexController#UpdatePosition[" + GetInstanceID() + "] lastPosition=" + lastPosition + ",newPosition=" + newPosition);
            mesh.vertices.ReplaceVertex(gameObject, newPosition);
            lastPosition = newPosition;
        }

        private void Start() {
            //Debug.Log("VertexController Start[" + GetInstanceID() + "]");
            //stickySelected = false;
        }
    }
}