using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace MeshEngine {
    public class ImportObj {
        public ObjParser.Obj obj;
        public ObjParser.Mtl mtl;

        // output
        public List<Vector3> vertices;
        public List<Vector2> uvs;
        public List<int> triangles;
        public List<string> materialNames;
        public List<Material> materials;

        private ImportObjFaceIndex faceIndex;
        private Vector3 loadObjectOffset = new Vector3(0, 0, 0);

        public ImportObj() {
            vertices = new List<Vector3>();
            uvs = new List<Vector2>();
            triangles = new List<int>();
            materialNames = new List<string>();
            materials = new List<Material>();
            faceIndex = new ImportObjFaceIndex();
        }

        public void Clear() {
            vertices.Clear();
            uvs.Clear();
            triangles.Clear();
            materialNames.Clear();
            materials.Clear();
            faceIndex.Clear();
        }

        private Color ColorOfObjColor(ObjParser.Types.Color objColor) {
            Color color = new Color();
            color.r = objColor.r;
            color.g = objColor.g;
            color.b = objColor.b;
            return color;
        }

        private List<Material> ListMaterialsOfObjMaterials(List<ObjParser.Types.Material> objMaterialList) {
            List<Material> materials = new List<Material>();
            foreach (ObjParser.Types.Material objMaterial in objMaterialList) {
                Material material = Materials.NewMaterialFromColor(ColorOfObjColor(objMaterial.DiffuseReflectivity));
                material.name = objMaterial.Name;
                materials.Add(material);
            }
            return materials;
        }

        private int VertexIndexFromFaceElement(ObjParser.Types.Face face, int elementIndex) {
            int faceVertexIndex = face.VertexIndexList[elementIndex];
            int faceTextureVertexIndex = face.TextureVertexIndexList[elementIndex];
            int vertexIndex;

            ObjParser.Types.Vertex vertex = obj.VertexList[faceVertexIndex - 1];

            if (faceIndex.ContainsKey(faceVertexIndex, faceTextureVertexIndex)) {
                vertexIndex = faceIndex.VertexIndex(faceVertexIndex, faceTextureVertexIndex);
            } else {
                vertices.Add(new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z) + loadObjectOffset);

                if (faceTextureVertexIndex != 0) {
                    ObjParser.Types.TextureVertex textureVertex = obj.TextureList[faceTextureVertexIndex - 1];
                    uvs.Add(new Vector2((float)textureVertex.X, (float)textureVertex.Y));
                }
                vertexIndex = vertices.Count - 1;
                faceIndex.Add(faceVertexIndex, faceTextureVertexIndex, vertexIndex);
            }
            return vertexIndex;
        }

        private void BuildFromFaces() {
            for (int i = 0; i < obj.FaceList.Count; i++) {
                ObjParser.Types.Face face = obj.FaceList[i];

                if (face.VertexIndexList.Length == 3) { // triangle face
                    triangles.Add(VertexIndexFromFaceElement(face, 0));
                    triangles.Add(VertexIndexFromFaceElement(face, 1));
                    triangles.Add(VertexIndexFromFaceElement(face, 2));
                    materialNames.Add(face.UseMtl);
                }
                if (face.VertexIndexList.Length == 4) { // quad face
                    triangles.Add(VertexIndexFromFaceElement(face, 0));
                    triangles.Add(VertexIndexFromFaceElement(face, 1));
                    triangles.Add(VertexIndexFromFaceElement(face, 2));
                    materialNames.Add(face.UseMtl);

                    triangles.Add(VertexIndexFromFaceElement(face, 0));
                    triangles.Add(VertexIndexFromFaceElement(face, 2));
                    triangles.Add(VertexIndexFromFaceElement(face, 3));
                    materialNames.Add(face.UseMtl);
                }
            }
        }

        public void Load(string path) {
            string objFileName = Path.ChangeExtension(path, ".obj");
            string mtlFileName = Path.ChangeExtension(path, ".mtl");

            if (File.Exists(mtlFileName)) {
                mtl = new ObjParser.Mtl();
                mtl.LoadMtl(mtlFileName);
                materials = ListMaterialsOfObjMaterials(mtl.MaterialList);
            }

            if (!File.Exists(objFileName)) return;
            obj = new ObjParser.Obj();
            obj.LoadObj(objFileName);
            BuildFromFaces();
        }
    }
}