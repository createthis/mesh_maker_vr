#if COLOR_PICKER
using CreateThis.VR.UI.ColorPicker;
using MeshEngine;

namespace MMVR.UI.ColorPicker {
    public class MMVR_ColorPickerIO : ColorPickerIOBase {
        protected override void OnColorChange(HSBColor color) {
            base.OnColorChange(color);
            Settings.SetFillColor(this.color);
        }

        protected override void Start() {
            this.color = Settings.fillColor;
            base.Start();
        }
    }
}
#endif