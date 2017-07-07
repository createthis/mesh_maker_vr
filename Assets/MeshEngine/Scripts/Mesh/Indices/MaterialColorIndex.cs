using UnityEngine;
using System.Collections.Generic;

namespace MeshEngine {
    public class MaterialColorIndex {
        private Dictionary<Color, List<int>> index; // key is a Color, value is an array of indices in MaterialManager.materials that contain this Color.

        public MaterialColorIndex() {
            if (index == null) {
                index = new Dictionary<Color, List<int>>();
            }
        }

        public List<int> MaterialIndicesByColor(Color color) {
            if (index.ContainsKey(color)) return index[color];
            return null;
        }

        public int LastMaterialIndexByColor(Color color) {
            if (index.ContainsKey(color)) {
                List<int> indices = MaterialIndicesByColor(color);
                return indices[indices.Count - 1];
            }
            return -1;
        }

        public bool ContainsColor(Color color) {
            return index.ContainsKey(color);
        }

        public void AddIndexByColor(Color color, int index) {
            if (this.index.ContainsKey(color)) {
                List<int> indices = this.index[color];
                indices.Add(index);
            } else {
                List<int> indices = new List<int>();
                indices.Add(index);
                this.index.Add(color, indices);
            }
        }

        public void Clear() {
            index.Clear();
        }
    }
}