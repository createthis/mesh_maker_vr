using UnityEngine;

namespace MeshEngine {
    public class MeshEngine_Defaults : MonoBehaviour {
        public GameObject vertexPrefab;
        public GameObject trianglePrefab;
        public UnityEngine.Material unselectedMaterial;
        public UnityEngine.Material highlightMaterial;
        public UnityEngine.Material outlineMaterial;
        public GameObject primitivePlanePrefab;
        public GameObject primitiveBoxPrefab;
        public GameObject primitiveCirclePrefab;
        public GameObject primitiveCylinderPrefab;
        public GameObject primitiveSpherePrefab;
        public GameObject boxSelectionPrefab;


        private void Initialize() {
            if (vertexPrefab != null) Meshes.vertexPrefab = vertexPrefab;
            if (trianglePrefab != null) Meshes.trianglePrefab = trianglePrefab;
            if (unselectedMaterial != null) Meshes.unselectedMaterial = unselectedMaterial;
            if (highlightMaterial != null) Meshes.highlightMaterial = highlightMaterial;
            if (outlineMaterial != null) Meshes.outlineMaterial = outlineMaterial;
            if (primitivePlanePrefab != null) Meshes.primitivePlanePrefab = primitivePlanePrefab;
            if (primitiveBoxPrefab != null) Meshes.primitiveBoxPrefab = primitiveBoxPrefab;
            if (primitiveCirclePrefab != null) Meshes.primitiveCirclePrefab = primitiveCirclePrefab;
            if (primitiveCylinderPrefab != null) Meshes.primitiveCylinderPrefab = primitiveCylinderPrefab;
            if (primitiveSpherePrefab != null) Meshes.primitiveSpherePrefab = primitiveSpherePrefab;
            if (boxSelectionPrefab != null) Meshes.boxSelectionPrefab = boxSelectionPrefab;
            Settings.Load();
            Meshes.GetSelectedMesh(); // Load from files.
        }

        void Awake() {
            Initialize();
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}