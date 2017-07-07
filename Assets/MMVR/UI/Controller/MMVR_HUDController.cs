using UnityEngine;
using MeshEngine;
using CreateThis.VR.UI.Controller;
using CreateThis.VR.GimbalCamera;

namespace MMVR.UI.Controller {
    public class MMVR_HUDController : MonoBehaviour {
        public Color lastFillColor;
        public GimbalCameraHUDFactory hudFactory;

        private void UpdateHUDOutlineFromFillColor() {
            if (Settings.fillColor == lastFillColor) return;
            lastFillColor = Settings.fillColor;

            GameObject hudOutline = hudFactory.GetHUDOutline();
            MeshRenderer meshRenderer = hudOutline.GetComponent<MeshRenderer>();

            meshRenderer.material.SetColor("_OutlineColor", Settings.fillColor);
        }

        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            UpdateHUDOutlineFromFillColor();
        }
    }
}