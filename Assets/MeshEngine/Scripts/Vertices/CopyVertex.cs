using UnityEngine;

namespace MeshEngine {
    public class CopyVertex {
        public System.Guid ID;
        public Vector3 position;
        public int indexFromMesh;

        public CopyVertex() {
            ID = System.Guid.NewGuid();
        }
    }
}