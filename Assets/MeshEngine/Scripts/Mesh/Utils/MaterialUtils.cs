using UnityEngine;
using System.Collections.Generic;
using CreateThis.Unity;
using MeshEngine.Controller;

namespace MeshEngine {
    public static class MaterialUtils {
        public static UnityEngine.Material FillTriangle(Mesh mesh, Triangle triangle) {
            Material mmMaterial = mesh.materials.FillTriangle(triangle);
            UnityEngine.Material material = MaterialToInstance(mmMaterial, true, true, true);
            if (!mesh.materials.materialsChanged) return material;
            TriangleController triangleController = triangle.instance.GetComponent<TriangleController>();
            triangleController.material = material;
            triangleController.SyncMaterials();
            return material;
        }

        public static UnityEngine.Material MaterialToInstance(Material material, bool renderMesh, bool renderWireframe, bool renderNormals) {
            return MaterialCache.MaterialByColor(material.color, renderMesh, renderWireframe, renderNormals);

        }

        public static UnityEngine.Material MaterialToInstance(Mesh mesh, Material material, bool noWireframe = false) {
            return MaterialCache.MaterialByColor(material.color, mesh.GetRenderMesh(), mesh.GetRenderWireframe(), mesh.GetRenderNormals());
        }

        public static UnityEngine.Material[] AssignMaterials(Mesh mesh, List<UnityEngine.Material> materials) {
            UnityEngine.Material[] materialsArray = materials.ToArray();
            mesh.GetComponent<MeshRenderer>().materials = materialsArray;
            return materialsArray;
        }

        public static List<UnityEngine.Material> GetMaterials(Mesh mesh) {
            List<UnityEngine.Material> materials = new List<UnityEngine.Material>();
            List<Material> mmMaterials = mesh.materials.GetMaterials();
            foreach (Material mmMaterial in mmMaterials) {
                UnityEngine.Material material = MaterialCache.MaterialByColor(mmMaterial.color, mesh.GetRenderMesh(), mesh.GetRenderWireframe(), mesh.GetRenderNormals());
                materials.Add(material);
            }
            return materials;
        }
    }
}