using UnityEngine;
using CreateThis.VR.UI.Interact;
using CreateThis.Unity;

namespace MeshEngine.Controller {
    public class TriangleController : Triggerable {
        public UnityEngine.Mesh uMesh;
        public Mesh mesh;
        public Vertex a;
        public Vertex b;
        public Vertex c;
        public Vector3 center;
        public UnityEngine.Material material;
        public UnityEngine.Material stickySelectedMaterial;

        private UnityEngine.Material blendedStickySelectedMaterial;
        private bool stickySelected;
        private bool isSelectable = true;
        private Selectable selectable;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private UnityEngine.Mesh collisionMesh;
        private bool hasInitialized;
        private UnityEngine.Material[] materials;
        private Vector3[] meshVertices;
        private int[] meshTriangles;
        private Vector3[] collisionMeshVertices;
        private int[] collisionMeshTriangles;

        public override void OnTriggerDown(Transform controller, int controllerIndex) {
            base.OnTriggerDown(controller, controllerIndex);
            switch (Mode.mode) {
                case ModeType.PickColor:
                    TriggerDownModePickColor();
                    break;
                case ModeType.SelectTriangles:
                    TriggerDownModeSelectTriangles();
                    break;
                case ModeType.SelectVerticesByTriangles:
                    TriggerDownModeSelectVertices();
                    break;
                case ModeType.TriangleDelete:
                    TriggerDownModeTriangleDelete();
                    break;
                case ModeType.Normal:
                    TriggerDownModeNormal();
                    break;
                case ModeType.Fill:
                    TriggerDownModeFill();
                    break;
            }
        }

        private void TriggerDownModeSelectTriangles() {
            if (mesh.selection.TriangleSelectedByVertices(GetTriangle())) {
                mesh.selection.DeselectTriangleByVertices(GetTriangle());
                return;
            }
            mesh.selection.SelectTriangleByVertices(GetTriangle());
        }

        private void TriggerDownModeSelectVertices() {
            if (mesh.selection.VerticesSelected(GetTriangle())) {
                mesh.selection.DeselectVertices(GetTriangle());
                return;
            }
            mesh.selection.SelectVertices(GetTriangle());
        }

        private void TriggerDownModeNormal() {
            mesh.triangles.FlipNormalByVertices(GetTriangle());
        }

        private void TriggerDownModeTriangleDelete() {
            RemoveTriangle();
        }

        private void TriggerDownModePickColor() {
            Settings.SetFillColor(material.color);
        }

        private void TriggerDownModeFill() {
            Triangle triangle = mesh.triangles.FindTriangleByVertices(GetTriangle());
            material = MaterialUtils.FillTriangle(mesh, triangle);
        }

        public void RemoveTriangle() {
            mesh.triangles.RemoveTriangleByVertices(GetTriangle());
        }

        public UnityEngine.Material GetMaterial() {
            if (stickySelected) {
                return blendedStickySelectedMaterial;
            } else {
                return material;
            }
        }

        public void UpdateStickySelectedMaterial() {
            Color color = material.color * stickySelectedMaterial.color;
            color.a = 1.0f;
            blendedStickySelectedMaterial = MaterialCache.MaterialByColor(color, true, true, true);
        }

        public void SyncMaterials() {
            UpdateStickySelectedMaterial();
            materials[0] = GetMaterial();
            meshRenderer.materials = materials;
            selectable.unselectedMaterials = materials;
            selectable.outlineMesh = uMesh;
            selectable.UpdateSelectedMaterials();
        }

        public void SetStickySelected(bool value) {
            stickySelected = value;
            SyncMaterials();
            selectable.SetSelected(selectable.selected);
        }

        public void SetSelectable(bool value) {
            bool oldValue = isSelectable;
            isSelectable = value;
            if (isSelectable) {
                meshCollider.enabled = true;
                if (oldValue != true) UpdateMeshCollider();
            } else {
                meshCollider.enabled = false;
            }
        }

        public void SetA(Vector3 newPosition) {
            a.position = newPosition;
            UpdateMeshes();
        }

        public void SetB(Vector3 newPosition) {
            b.position = newPosition;
            UpdateMeshes();
        }

        public void SetC(Vector3 newPosition) {
            c.position = newPosition;
            UpdateMeshes();
        }

        public void UpdateMeshCollider() {
            collisionMeshVertices[0] = a.position - center;
            collisionMeshVertices[1] = b.position - center;
            collisionMeshVertices[2] = c.position - center;
            collisionMeshVertices[3] = -Vector3.Cross(b.position - a.position, c.position - a.position).normalized * 0.025f;
            collisionMesh.Clear();
            collisionMesh.vertices = collisionMeshVertices;
            collisionMesh.triangles = collisionMeshTriangles;

            if (!(a.position == b.position || b.position == c.position)) meshCollider.sharedMesh = collisionMesh;
        }

        public void UpdateMeshes() {
            center = mesh.Center(a.position, b.position, c.position);
            transform.localPosition = center;

            UpdateMesh();
            if (selectable) UpdateMeshCollider();
        }

        private void UpdateMesh() {
            meshVertices[0] = a.position - center;
            meshVertices[1] = b.position - center;
            meshVertices[2] = c.position - center;
            uMesh.Clear();
            uMesh.vertices = meshVertices;
            uMesh.triangles = meshTriangles;
        }

        public void UnsubscribeEvents() {
            if (a != null) a.OnUpdateVertex -= UpdateMeshes;
            if (b != null) b.OnUpdateVertex -= UpdateMeshes;
            if (c != null) c.OnUpdateVertex -= UpdateMeshes;
        }

        public void SubscribeEvents() {
            if (a != null) a.OnUpdateVertex += UpdateMeshes;
            if (b != null) b.OnUpdateVertex += UpdateMeshes;
            if (c != null) c.OnUpdateVertex += UpdateMeshes;
        }

        public void Populate(Vertex a, Vertex b, Vertex c, Vector3 center) {
            UnsubscribeEvents();

            this.a = a;
            this.b = b;
            this.c = c;

            SubscribeEvents();

            this.center = center;
            transform.localScale = Vector3.one;

            UpdateMesh();
            if (selectable) UpdateMeshCollider();
        }

        public Vertex[] GetTriangle() {
            Vertex[] triangle = new Vertex[] { a, b, c };
            return triangle;
        }

        private void OnDisable() {
            UnsubscribeEvents();
        }

        private void OnEnable() {
            SubscribeEvents();
        }

        public void Initialize() {
            if (!hasInitialized) { // avoid GC alloc on subsequent calls
                selectable = GetComponent<Selectable>();
                meshRenderer = GetComponent<MeshRenderer>();
                meshCollider = GetComponent<MeshCollider>();
                if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();
                collisionMesh = new UnityEngine.Mesh();
                collisionMeshVertices = new Vector3[4];
                collisionMesh.vertices = new Vector3[4];
                collisionMeshTriangles = new int[] { 0, 1, 2, 0, 3, 1, 3, 0, 2, 1, 3, 2 };
                collisionMesh.triangles = collisionMeshTriangles;
                uMesh = gameObject.GetComponent<MeshFilter>().mesh;
                meshVertices = new Vector3[3];
                uMesh.vertices = new Vector3[3];
                meshTriangles = new int[3] { 0, 1, 2 };
                uMesh.triangles = meshTriangles;
                materials = new UnityEngine.Material[1];
                hasInitialized = true;
            }
        }

        // Use this for initialization
        void Start() {
            if (!hasInitialized) Initialize();
        }
    }
}