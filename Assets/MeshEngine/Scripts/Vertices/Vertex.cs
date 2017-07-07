using UnityEngine;

namespace MeshEngine {
    public class Vertex {
        public System.Guid ID;
        public Vector3 position;
        public GameObject instance;
        public delegate void UpdateVertex();
        public event UpdateVertex OnUpdateVertex;
        public Vector2 uv;

        public Vertex() {
            ID = System.Guid.NewGuid();
        }

        public void CallOnUpdateVertex() {
            if (OnUpdateVertex != null) {
                OnUpdateVertex();
            }
        }
    }
}