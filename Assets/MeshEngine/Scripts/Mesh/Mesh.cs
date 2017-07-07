using UnityEngine;
using System.Collections.Generic;

namespace MeshEngine {
    public class Mesh : MonoBehaviour {
        public UnityEngine.Mesh uMesh;
        public GameObject vertexPrefab;
        public GameObject trianglePrefab;
        public Material highlightMaterial;
        public Material outlineMaterial;
        public float normalLength;

        public Vertices vertices;
        public Triangles triangles;
        public Edges edges;
        public Selection selection;
        public AlignmentTools alignmentTools;
        public Persistence persistence;
        public Copy copy;
        public Extrusion extrusion;
        public Materials materials;

        public bool load;
        private bool renderMesh;
        private bool renderWireframe;
        private bool renderNormals;

        private bool hasInitialized;


        private void Awake() {
        }

        public Vector3 Center(Vector3 a, Vector3 b, Vector3 c) {
            return (a + b + c) / 3f;
        }

        public void Initialize() {
            Debug.Log("MeshController#Start[" + gameObject.GetInstanceID() + "]");

            renderMesh = true;
            renderWireframe = true;
            renderNormals = true;

            uMesh = new UnityEngine.Mesh();
            GetComponent<MeshFilter>().mesh = uMesh;

            vertices = new Vertices(this);
            triangles = new Triangles(this);
            edges = new Edges(this);
            alignmentTools = new AlignmentTools(this);
            selection = new Selection(this);
            persistence = new Persistence(this);
            copy = new Copy(this);
            extrusion = new Extrusion(this);
            materials = new Materials(this);

            vertices.verticesChanged = true;
            triangles.trianglesChanged = true;

            if (load) {
                persistence.Load();

                gameObject.transform.Rotate(new Vector3(-90f, 0, 0)); // Compensate for Z being vertical rather than depth in Unity

                // Prime the vertex instance pool
                vertices.CreateVertexInstances();

                // Prime the triangle instance pool
                triangles.autoCreateTriangleObjects = true;
                triangles.SetTriangleInstancesSelectable(false);
                triangles.CreateTriangleInstances();
                triangles.DeleteTriangleInstances();
            }

            BoxCollider collider = GetComponent<BoxCollider>();
            Bounds bounds = GetComponent<MeshRenderer>().bounds;
            collider.transform.parent = this.transform;
            collider.center = this.transform.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
            //Debug.Log("vertices=" + VerticesToString());
            //Debug.Log("triangles=" + TrianglesToString());
            //instance.transform.localScale = new Vector3(width / 9.0f, width / 9.0f, width / 9.0f);
            hasInitialized = true;
        }

        public void LateUpdate() {
            vertices.BuildVertices();
            triangles.BuildTriangles();
        }

        public void Start() {
            if (!hasInitialized) Initialize();
        }

        public void UpdateAlignmentTools() { // This method only exists so I can select it for UnityEvents via the Editor
            alignmentTools.UpdateAlignmentTools();
        }

        public void Load() { // This method only exists so I can select it for UnityEvents via the Editor
            persistence.Load();
        }

        public void Save() { // This method only exists so I can select it for UnityEvents via the Editor
            persistence.Save();
        }

        public void SetRenderOptions(bool renderMeshValue, bool renderWireframeValue, bool renderNormalsValue) {
            bool changed = false;
            if (renderMesh != renderMeshValue || renderWireframe != renderWireframeValue || renderNormals != renderNormalsValue) changed = true;
            renderMesh = renderMeshValue;
            renderWireframe = renderWireframeValue;
            renderNormals = renderNormalsValue;

            if (changed) {
                List<UnityEngine.Material> materials = MaterialUtils.GetMaterials(this);
                MaterialUtils.AssignMaterials(this, materials);
            }
        }

        public bool GetRenderMesh() {
            return renderMesh;
        }

        public bool GetRenderWireframe() {
            return renderWireframe;
        }

        public bool GetRenderNormals() {
            return renderNormals;
        }

        public void StartScale() {
            vertices.DeleteVertexInstances();
            triangles.DeleteTriangleInstances();
            SetRenderOptions(true, true, true);
        }

        public void EndScale() {
            Mode.SetMode(Mode.mode);
        }

        public void ResetTransform() {
            vertices.DeleteVertexInstances();
            triangles.DeleteTriangleInstances();
            transform.localScale = Vector3.one;
            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.Euler(new Vector3(-90f, 0, 0));
            Mode.SetMode(Mode.mode);
        }

        public void EnableBoxCollider() {
            renderWireframe = false;
            renderNormals = false;
            uMesh.RecalculateBounds();
            GetComponent<BoxCollider>().enabled = true;

            Quaternion oldRotation = transform.rotation;
            Vector3 oldPosition = transform.position;
            Vector3 oldScale = transform.localScale;

            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            BoxCollider collider = GetComponent<BoxCollider>();
            Bounds bounds = GetComponent<MeshRenderer>().bounds;
            collider.transform.parent = transform;
            collider.center = transform.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;

            transform.position = oldPosition;
            transform.rotation = oldRotation;
            transform.localScale = oldScale;
        }

        public void DisableBoxCollider() {
            renderWireframe = true;
            renderNormals = true;
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}