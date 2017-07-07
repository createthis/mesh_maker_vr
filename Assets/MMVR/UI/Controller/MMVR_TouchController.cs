using UnityEngine;
using MeshEngine;
using CreateThis.VR.UI.Controller;

namespace MMVR.UI.Controller {
    public class MMVR_TouchController : TouchController {
        public Color lastFillColor;
        public Renderer pointerConeRenderer;

        private void UpdatePointerConeFromFillColor() {
            if (Settings.fillColor == lastFillColor) return;
            lastFillColor = Settings.fillColor;
            UnityEngine.Material[] materials = pointerConeRenderer.materials;
            Color pointerConeColor = Settings.fillColor;
            pointerConeColor.a = materials[0].color.a;
            materials[0].color = pointerConeColor;
            pointerConeRenderer.materials = materials;
        }

        // Use this for initialization
        protected override void Start() {
            base.Start();
            pointerConeRenderer = pointerConeInstance.GetComponent<Renderer>();
        }

        // Update is called once per frame
        protected override void Update() {
            base.Update();
            UpdatePointerConeFromFillColor();
        }
    }
}