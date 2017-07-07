using System;
using System.Collections.Generic;

namespace MeshEngine {
    public class ImportObjFaceIndex {
        private struct Key : IEquatable<Key> {
            public int vertexIndex;
            public int textureVertexIndex;

            public bool Equals(Key other) {
                return this.vertexIndex == other.vertexIndex &&
                       this.textureVertexIndex == other.textureVertexIndex;
            }

            public override int GetHashCode() {
                return this.vertexIndex.GetHashCode() ^ this.textureVertexIndex.GetHashCode();
            }
        }
        private Dictionary<Key, int> index;

        public ImportObjFaceIndex() {
            index = new Dictionary<Key, int>();
        }

        private Key IndicesToKey(int faceVertexIndex, int faceTextureVertexIndex) {
            Key key = new Key();
            key.vertexIndex = faceVertexIndex;
            key.textureVertexIndex = faceTextureVertexIndex;
            return key;
        }

        public void Clear() {
            index.Clear();
        }

        public bool ContainsKey(int faceVertexIndex, int faceTextureVertexIndex) {
            Key key = IndicesToKey(faceVertexIndex, faceTextureVertexIndex);
            return index.ContainsKey(key);
        }

        public int VertexIndex(int faceVertexIndex, int faceTextureVertexIndex) {
            Key key = IndicesToKey(faceVertexIndex, faceTextureVertexIndex);
            return index[key];
        }

        public void Add(int faceVertexIndex, int faceTextureVertexIndex, int vertexIndex) {
            Key key = IndicesToKey(faceVertexIndex, faceTextureVertexIndex);
            index.Add(key, vertexIndex);
        }
    }
}