using System.Collections.Generic;
using UnityEngine;
using MeshEngine.Controller;
using CreateThis.VR.UI.Interact;

namespace MeshEngine {
    public static class Meshes {
        public static GameObject vertexPrefab;
        public static GameObject trianglePrefab;
        public static UnityEngine.Material unselectedMaterial;
        public static UnityEngine.Material highlightMaterial;
        public static UnityEngine.Material outlineMaterial;
        public static GameObject primitivePlanePrefab;
        public static GameObject primitiveBoxPrefab;
        public static GameObject primitiveCirclePrefab;
        public static GameObject primitiveCylinderPrefab;
        public static GameObject primitiveSpherePrefab;
        public static GameObject boxSelectionPrefab;

        private static List<Mesh> meshes;
        private static Mesh selectedMesh;
        private static bool hasInitialized;

        public static void Add(Mesh mesh) {
            Initialize();

            meshes.Add(mesh);

            if (meshes.Count == 1) selectedMesh = mesh;
        }

        public static List<Mesh> GetMeshes() {
            Initialize();

            return meshes;
        }

        public static void SetSelectedMesh(Mesh mesh) {
            Initialize();

            if (meshes.Contains(mesh)) {
                selectedMesh = mesh;
            }
        }

        public static void NewMesh() {
            GameObject meshInstance = new GameObject();
            meshInstance.name = "New Mesh";
            Mesh newMesh = meshInstance.AddComponent<Mesh>();
            meshInstance.AddComponent<MeshFilter>();
            meshInstance.AddComponent<MeshRenderer>();
            meshInstance.AddComponent<BoxCollider>();
            MeshInputController meshInputController = meshInstance.AddComponent<MeshInputController>();
            meshInputController.mesh = newMesh;
            Selectable selectable = meshInstance.AddComponent<Selectable>();
            selectable.unselectedMaterials = new UnityEngine.Material[] { unselectedMaterial };
            selectable.highlightMaterial = highlightMaterial;
            selectable.outlineMaterial = outlineMaterial;
            newMesh.vertexPrefab = vertexPrefab;
            newMesh.trianglePrefab = trianglePrefab;
            newMesh.transform.Rotate(new Vector3(-90f, 0, 0)); // Compensate for Z being vertical rather than depth in Unity
            newMesh.transform.position = new Vector3(0, 2, 0);
            newMesh.Initialize();
            Add(newMesh);
        }

        public static Mesh GetSelectedMesh() {
            Initialize();

            if (selectedMesh == null) {
                if (meshes.Count == 0) {
                    NewMesh();
                }
            }
            return selectedMesh;
        }

        private static void Initialize() {
            if (hasInitialized) return;
            meshes = new List<Mesh>();
            hasInitialized = true;
        }
    }
}