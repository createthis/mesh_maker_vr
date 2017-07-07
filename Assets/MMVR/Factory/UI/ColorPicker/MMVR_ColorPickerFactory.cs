#if COLOR_PICKER
using UnityEngine;
using CreateThis.Unity;
using CreateThis.Factory.VR.UI.ColorPicker;
using MMVR.UI.ColorPicker;

namespace MMVR.Factory.UI.ColorPicker {
    public class MMVR_ColorPickerFactory : ColorPickerFactory {
        protected override void CreateColorPickerIO(GameObject parent) {
            MMVR_ColorPickerIO colorPickerIO = Undoable.AddComponent<MMVR_ColorPickerIO>(parent);
            colorPickerIO.color = Color.blue;
        }
    }
}
#endif