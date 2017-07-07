using UnityEngine;
using System.Collections.Generic;

namespace MeshEngine {
    public class AlignmentTools {
        public List<GameObject> alignmentTools;

        public Mesh mesh;

        public AlignmentTools(Mesh mesh) {
            this.mesh = mesh;
            alignmentTools = new List<GameObject>();
        }

        public void UpdateAlignmentTools() {
            if (Settings.AlignmentToolsEnabled()) {
                ShowAlignmentTools();
            } else {
                HideAlignmentTools();
            }
        }

        public void SetActiveCollidersOnAlignmentTools(bool value) {
            GameObject[] alignmentTools = GameObject.FindGameObjectsWithTag("AlignmentTool");
            foreach (GameObject alignmentTool in alignmentTools) {
                BoxCollider[] boxColliders = alignmentTool.GetComponents<BoxCollider>();
                foreach (BoxCollider boxCollider in boxColliders) {
                    boxCollider.enabled = value;
                }
            }
        }

        public void ShowAlignmentTools() {
            foreach (GameObject alignmentTool in alignmentTools) {
                alignmentTool.SetActive(true);
            }
        }

        public void HideAlignmentTools() {
            foreach (GameObject alignmentTool in alignmentTools) {
                alignmentTool.SetActive(false);
            }
        }
    }
}