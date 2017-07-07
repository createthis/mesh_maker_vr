using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace MeshEngine {
    public class ExportObj {
        public ObjParser.Obj obj;
        public ObjParser.Mtl mtl;

        // input
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;
        public List<Material> materials;
        public List<string> materialNames;

        public ExportObj() {
        }

        private ObjParser.Types.Color ObjColorOfColor(Color color) {
            ObjParser.Types.Color objColor = new ObjParser.Types.Color();
            objColor.r = color.r;
            objColor.g = color.g;
            objColor.b = color.b;
            return objColor;
        }

        private string EscapeMaterialName(string name) {
            return name.Replace(" ", "_");
        }

        private List<ObjParser.Types.Material> ObjMaterialsOfListMaterials(List<Material> materials) {
            List<ObjParser.Types.Material> objMaterials = new List<ObjParser.Types.Material>();
            foreach (Material material in materials) {
                ObjParser.Types.Material objMaterial = new ObjParser.Types.Material();
                objMaterial.Name = EscapeMaterialName(material.name);
                objMaterial.DiffuseReflectivity = ObjColorOfColor(material.color);
                objMaterial.SpecularExponent = 96.078431f;
                objMaterial.SpecularReflectivity = ObjColorOfColor(new Color(0.5f, 0.5f, 0.5f));
                objMaterial.EmissiveCoefficient = ObjColorOfColor(new Color(0, 0, 0));
                objMaterial.IlluminationModel = 2;
                objMaterials.Add(objMaterial);
            }
            return objMaterials;
        }

        private List<ObjParser.Types.Vertex> ObjVerticesOfListVector3(Vector3[] vertices) {
            List<ObjParser.Types.Vertex> vertexList = new List<ObjParser.Types.Vertex>();
            int i = 0;
            foreach (Vector3 vertex in vertices) {
                ObjParser.Types.Vertex newVertex = new ObjParser.Types.Vertex();
                newVertex.X = vertex.x;
                newVertex.Y = vertex.y;
                newVertex.Z = vertex.z;
                newVertex.Index = i;
                vertexList.Add(newVertex);
                i++;
            }
            return vertexList;
        }

        private List<ObjParser.Types.TextureVertex> ObjTextureVerticesOfListVector2(Vector2[] uvs) {
            List<ObjParser.Types.TextureVertex> textureVertexList = new List<ObjParser.Types.TextureVertex>();
            int i = 0;
            foreach (Vector2 uv in uvs) {
                ObjParser.Types.TextureVertex newVertex = new ObjParser.Types.TextureVertex();
                newVertex.X = uv.x;
                newVertex.Y = uv.y;
                newVertex.Index = i;
                textureVertexList.Add(newVertex);
                i++;
            }
            return textureVertexList;
        }

        private List<ObjParser.Types.Face> ObjFacesOfListInt(int[] triangles) {
            List<ObjParser.Types.Face> faceList = new List<ObjParser.Types.Face>();

            for (int i = 0; i < triangles.Length; i += 3) {
                ObjParser.Types.Face newFace = new ObjParser.Types.Face();
                newFace.UseMtl = EscapeMaterialName(materialNames[i / 3]);
                newFace.VertexIndexList = new int[] {
                triangles[i] + 1,
                triangles[i + 1] + 1,
                triangles[i+2] + 1
            };
                if (uvs != null) {
                    newFace.TextureVertexIndexList = newFace.VertexIndexList;
                } else {
                    newFace.TextureVertexIndexList = new int[0];
                }
                faceList.Add(newFace);
            }
            return faceList;
        }

        public void Save(string path) {
            string objFileName = Path.ChangeExtension(path, ".obj");
            string mtlFileName = Path.ChangeExtension(path, ".mtl");

            File.Delete(objFileName);
            string[] headers = new string[] { "Mesh Maker VR" };
            obj = new ObjParser.Obj();
            obj.VertexList = ObjVerticesOfListVector3(vertices);
            if (uvs != null) obj.TextureList = ObjTextureVerticesOfListVector2(uvs);
            obj.FaceList = ObjFacesOfListInt(triangles);
            obj.WriteObjFile(objFileName, headers);

            File.Delete(mtlFileName);
            mtl = new ObjParser.Mtl();
            mtl.MaterialList = ObjMaterialsOfListMaterials(materials);
            mtl.WriteMtlFile(mtlFileName, headers);
        }
    }
}