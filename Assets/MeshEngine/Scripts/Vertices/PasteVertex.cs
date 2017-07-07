using UnityEngine;

namespace MeshEngine {
    public class PasteVertex {
        public System.Guid ID;
        public CopyVertex copyVertex;
        public int indexInMesh;
        public GameObject instance;

        public PasteVertex() {
            ID = System.Guid.NewGuid();
        }
    }
}