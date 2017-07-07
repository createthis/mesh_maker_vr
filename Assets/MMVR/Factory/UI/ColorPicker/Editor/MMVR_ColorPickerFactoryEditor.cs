#if COLOR_PICKER
using UnityEditor;
using CreateThis.Factory.VR.UI.ColorPicker;

namespace MMVR.Factory.UI.ColorPicker {
    [CustomEditor(typeof(MMVR_ColorPickerFactory))]
    [CanEditMultipleObjects]

    public class MMVR_ColorPickerFactoryEditor : ColorPickerFactoryEditor {
    }
}
#endif