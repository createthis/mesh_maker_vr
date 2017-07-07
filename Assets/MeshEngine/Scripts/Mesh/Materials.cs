using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MeshEngine {
    public class Materials {
        public class MaterialTriangles {
            public List<int> triangles;
        }
        public Mesh mesh;
        public bool materialsChanged;

        private List<Material> materials;
        private Dictionary<string, int> nameIndex;
        private MaterialColorIndex colorIndex;
        private List<int> triangleMaterials; // index points to triangle in TrianglesManager.triangles * 3, value is index in MaterialManager.materials. length = TrianglesManager.triangles.Count / 3.

        public Materials(Mesh mesh) {
            this.mesh = mesh;
            materials = new List<Material>();
            nameIndex = new Dictionary<string, int>();
            colorIndex = new MaterialColorIndex();
            triangleMaterials = new List<int>();
            materialsChanged = false;
        }

        public void Clear() {
            materials.Clear();
            nameIndex.Clear();
            colorIndex.Clear();
            triangleMaterials.Clear();
        }

        public int TriangleMaterialsCount() {
            return triangleMaterials.Count;
        }

        public void TruncateToCount(int count) {
            for (int i = triangleMaterials.Count - 1; i >= count; i--) {
                triangleMaterials.RemoveAt(i);
            }
            materialsChanged = true;
        }

        public int FindOrCreateMaterialIndexByFillColor() {
            Color fillColor = Settings.fillColor;
            int materialIndex = FindOrCreateMaterialIndexByColor(fillColor);
            return materialIndex;
        }

        public void SetMaterials(List<Material> myMaterials) {
            Clear();
            materials = myMaterials;
            BuildIndices();
        }

        public List<Material> GetMaterials() {
            return materials;
        }

        public List<int> GetTriangleIndexMaterialIndex() {
            return triangleMaterials;
        }

        public Material MaterialByIndex(int materialIndex) {
            return materials[materialIndex];
        }

        private string ColorArrayString(Color[] colors) {
            List<string> strings = new List<string>();
            foreach (Color color in colors) {
                strings.Add(ColorUtility.ToHtmlStringRGBA(color));
            }
            return string.Join(",", strings.ToArray());
        }

        public int FindOrCreateMaterialIndexByColor(Color color) {
            int index = colorIndex.LastMaterialIndexByColor(color);
            if (index != -1) return index;
            return NewMaterialIndexFromColor(color);
        }

        private string FindNextName(string name) {
            // FIXME: This is O(N) for the number of same names in the system.
            int i = 2;
            while (true) {
                string nextName = name + '_' + i;
                if (!nameIndex.ContainsKey(nextName)) return nextName;
                i++;
            }
        }

        public int NewMaterialIndexFromColor(Color color) {
            Material material = NewMaterialFromColor(color);
            if (nameIndex.ContainsKey(material.name)) material.name = FindNextName(material.name);
            return AddMaterial(material);
        }

        public static Material NewMaterialFromColor(Color color) {
            Material material = new Material();
            material.color = color;
            material.name = "Material_#" + ColorUtility.ToHtmlStringRGBA(color);
            return material;
        }

        public int GetLastMaterialIndex() {
            if (materials.Count > 0) return materials.Count - 1;
            Material material = GetLastMaterial();
            AddMaterial(material);
            return materials.Count - 1;
        }

        public Material GetLastMaterial() {
            if (materials.Count > 0) return materials.Last();
            Color fillColor = Settings.fillColor;
            return NewMaterialFromColor(fillColor);
        }

        public int MaterialIndexByTriangleIndex(int triangleIndex) {
            int index = triangleIndex / 3;
            return triangleMaterials[index];
        }

        public Material GetMaterialByTriangleIndex(int triangleIndex) {
            int index = triangleIndex / 3;
            return materials[triangleMaterials[index]];
        }

        public List<string> MaterialNames() {
            List<string> materialNames = new List<string>();
            for (int i = 0; i < triangleMaterials.Count; i++) {
                materialNames.Add(materials[triangleMaterials[i]].name);
            }
            return materialNames;
        }

        public void SetTriangleIndexMaterialIndex(int triangleIndex, int materialIndex) {
            int index = triangleIndex / 3;
            triangleMaterials[index] = materialIndex;
            materialsChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void AppendTriangleUsingFillColorMaterial() {
            int fillColorMaterialIndex = FindOrCreateMaterialIndexByFillColor();
            triangleMaterials.Add(fillColorMaterialIndex);
            //Debug.Log("AppendTriangleUsingFillColorMaterial fillColor=" + ColorUtility.ToHtmlStringRGBA(meshController.settingsPanelController.GetComponent<SettingsController>().fillColor) + ",mmMaterial.color=" + ColorUtility.ToHtmlStringRGBA(materials[fillColorMaterialIndex].color) + ",triangleMaterials.Count="+ triangleMaterials.Count + ",fillColorMaterialIndex="+ fillColorMaterialIndex);
            materialsChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void AppendTriangleUsingMaterialIndex(int materialIndex) {
            triangleMaterials.Add(materialIndex);
            materialsChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void AppendTriangleUsingLastMaterial() {
            int lastMaterialIndex = GetLastMaterialIndex();
            triangleMaterials.Add(lastMaterialIndex);
            materialsChanged = true;
            mesh.persistence.changedSinceLastSave = true;
        }

        public void RemoveTriangleByIndex(int triangleIndex) {
            int index = triangleIndex / 3;
            triangleMaterials.RemoveAt(index);
            materialsChanged = true;
            mesh.persistence.changedSinceLastSave = true;
            // TODO: remove material if all references to it have been removed?
        }

        private string ColorToString(Color color) {
            return "r=" + color.r.ToString() + ",g=" + color.g.ToString() + ",b=" + color.b.ToString() + ",a=" + color.a.ToString();
        }

        public int AddMaterial(Material material) {
            materials.Add(material);
            int index = materials.Count - 1;
            nameIndex.Add(material.name, index);
            colorIndex.AddIndexByColor(material.color, index);
            return index;
        }

        public Material MaterialByTriangleIndex(int triangleIndex) {
            int index = triangleIndex / 3;
            //Debug.Log("MaterialByTriangleIndex index=" + index + ",triangleMaterials[index]="+ triangleMaterials[index]);
            return materials[triangleMaterials[index]];
        }

        public void BuildIndices() {
            BuildNameIndex();
            BuildColorIndex();
        }

        public void BuildColorIndex() {
            colorIndex.Clear();
            for (int i = 0; i < materials.Count; i++) {
                Material material = materials[i];
                colorIndex.AddIndexByColor(material.color, i);
            }
        }

        public void BuildNameIndex() {
            nameIndex.Clear();
            for (int i = 0; i < materials.Count; i++) {
                Material material = materials[i];
                nameIndex.Add(material.name, i);
            }
        }

        public Material FillTriangle(Triangle triangle) {
            Material triangleMaterial = mesh.materials.GetMaterialByTriangleIndex(triangle.index);
            Color fillColor = Settings.fillColor;
            if (triangleMaterial.color == fillColor) return triangleMaterial; // triangle is already the desired color - nothing to do.

            int materialIndex = mesh.materials.FindOrCreateMaterialIndexByColor(fillColor);
            mesh.materials.SetTriangleIndexMaterialIndex(triangle.index, materialIndex);
            Material material = mesh.materials.MaterialByIndex(materialIndex);
            materialsChanged = true;
            mesh.persistence.changedSinceLastSave = true;
            return material;
        }

        public void PopulateTriangleMaterialsByMaterialNames(List<string> materialNames) {
            triangleMaterials.Clear();
            foreach (string name in materialNames) {
                if (name == null) {
                    int materialIndex = GetLastMaterialIndex();
                    triangleMaterials.Add(materialIndex);
                } else {
                    triangleMaterials.Add(nameIndex[name]);
                }
            }
        }

        public List<MaterialTriangles> GetTriangles() {
            List<MaterialTriangles> triangles = new List<MaterialTriangles>();

            for (int i = 0; i < materials.Count; i++) {
                MaterialTriangles materialTriangles = new MaterialTriangles();
                materialTriangles.triangles = new List<int>();
                triangles.Add(materialTriangles);
            }

            for (int i = 0; i < mesh.triangles.triangles.Count; i++) {
                int materialIndex = triangleMaterials[i / 3];
                triangles[materialIndex].triangles.Add(mesh.triangles.triangles[i]);
            }
            return triangles;
        }
    }
}