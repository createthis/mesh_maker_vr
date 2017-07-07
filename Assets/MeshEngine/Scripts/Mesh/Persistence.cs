using MeshEngine.Controller;

namespace MeshEngine {
    public class Persistence {
        public ObjParser.Obj obj;
        public ObjParser.Mtl mtl;
        public string defaultFilename = @"load";
        public bool changedSinceLastSave;
        public string lastFilePath;

        public Mesh mesh;

        public Persistence(Mesh mesh) {
            this.mesh = mesh;
            changedSinceLastSave = false;
        }

        public void Load(string filePath = null) {
            if (filePath == null) filePath = defaultFilename;

            ImportObj importObj = new ImportObj();
            importObj.Load(filePath);
            lastFilePath = filePath;

            if (importObj.materials != null) {
                mesh.materials.SetMaterials(importObj.materials);
            }
            mesh.vertices.SetVertices(mesh.vertices.ListVertexOfVector3Array(importObj.vertices, importObj.uvs));
            mesh.triangles.triangles = importObj.triangles;
            mesh.materials.PopulateTriangleMaterialsByMaterialNames(importObj.materialNames);
            mesh.vertices.CreateVertexInstances();
            mesh.vertices.verticesChanged = true;
            mesh.triangles.trianglesChanged = true;
            //mesh.GetComponent<AlignmentToolsController>().Load();
            //mesh.alignmentTools.UpdateAlignmentTools();
            //mesh.alignmentTools.SetActiveCollidersOnAlignmentTools(false);
            changedSinceLastSave = false;
        }

        public bool HasSavedOrLoaded() {
            if (lastFilePath == null) return false;
            return true;
        }

        public void Save(string filePath = null) {
            if (filePath == null) {
                if (lastFilePath != null) {
                    filePath = lastFilePath;
                } else {
                    filePath = defaultFilename;
                }
            } else {
                lastFilePath = filePath;
            }

            mesh.vertices.BuildVertices();
            mesh.triangles.BuildTriangles();

            ExportObj exportObj = new ExportObj();
            exportObj.vertices = mesh.uMesh.vertices;
            exportObj.uvs = mesh.uMesh.uv;
            exportObj.triangles = mesh.triangles.triangles.ToArray();
            exportObj.materials = mesh.materials.GetMaterials();
            exportObj.materialNames = mesh.materials.MaterialNames();
            exportObj.Save(filePath);

            //mesh.GetComponent<AlignmentToolsController>().Save();
            changedSinceLastSave = false;
        }
    }
}