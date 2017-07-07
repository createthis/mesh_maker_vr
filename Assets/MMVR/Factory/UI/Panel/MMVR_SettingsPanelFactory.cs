using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if VRTK
using CreateThis.VRTK;
#endif
using CreateThis.Unity;
using CreateThis.VR.UI;
using CreateThis.VR.UI.Panel;
using CreateThis.VR.UI.ColorPicker;
using MMVR.Factory.UI.Button;
using MMVR.Factory.UI.ColorPicker;
using ToggleType = MeshEngine.Settings.ToggleType;

namespace MMVR.Factory.UI.Panel {
    public class MMVR_SettingsPanelFactory : MMVR_BasePanelFactory {
        public ColorPickerProfile colorPickerProfile;
        public SkyboxManager skyboxManager;

        private GameObject settingsPanelInstance;
        private StandardPanel settingsPanel;

        protected GameObject SkyboxButton(StandardPanel panel, GameObject parent, string buttonText, string skybox) {
            SkyboxButtonFactory factory = Undoable.AddComponent<SkyboxButtonFactory>(disposable);
            SetToggleButtonValues(factory, panel, parent);
            factory.buttonText = buttonText;
            factory.skybox = skybox;
            factory.skyboxManager = skyboxManager;
            return GenerateToggleButtonAndSetPosition(factory);
        }

        protected GameObject ColorPicker(StandardPanel panel, GameObject parent) {
            ButtonProfile profile = Defaults.GetMomentaryButtonProfile(momentaryButtonProfile);
            MMVR_ColorPickerFactory factory = Undoable.AddComponent<MMVR_ColorPickerFactory>(disposable);
            factory.parent = parent;
            factory.colorPickerProfile = Defaults.GetProfile(colorPickerProfile);
            GameObject colorPicker = factory.Generate();
            colorPicker.transform.localPosition = new Vector3(0, 0, profile.labelZ);
            return colorPicker;
        }

        protected GameObject SettingsButton(StandardPanel panel, GameObject parent, string buttonText, ToggleType toggleType) {
            SettingsToggleButtonFactory factory = Undoable.AddComponent<SettingsToggleButtonFactory>(disposable);
            SetToggleButtonValues(factory, panel, parent);
            factory.buttonText = buttonText;
            factory.toggleType = toggleType;
            return GenerateToggleButtonAndSetPosition(factory);
        }

        private void CreateDisposable(GameObject parent) {
            if (disposable) return;
            disposable = EmptyChild(parent, "disposable");
        }

        private GameObject FileLabelRow(GameObject parent) {
            GameObject row = Row(parent, "FileLabelRow", TextAlignment.Left);
            Label(row, "FileLabel", "File");
            return row;
        }

        private GameObject ChangeSkyboxLabelRow(GameObject parent) {
            GameObject row = Row(parent, "ChangeSkyboxLabelRow", TextAlignment.Left);
            Label(row, "ChangeSkyboxLabel", "Change Skybox");
            return row;
        }

        private GameObject SnapsRow(GameObject parent) {
            GameObject row = Row(parent, "SnapRow", TextAlignment.Left);
            SettingsButton(settingsPanel, row, "Snaps", ToggleType.Snap);
            return row;
        }

        private GameObject AlignmentToolsRow(GameObject parent) {
            GameObject row = Row(parent, "AlignmentToolsRow", TextAlignment.Left);
            SettingsButton(settingsPanel, row, "Alignment Tools", ToggleType.AlignmentTools);
            return row;
        }

        private GameObject TracingRow(GameObject parent) {
            GameObject row = Row(parent, "TracingRow", TextAlignment.Left);
            SettingsButton(settingsPanel, row, "3d Tracing Mode", ToggleType.TracingMode);
            SettingsButton(settingsPanel, row, "Custom Pointer Location", ToggleType.CustomPointerLocation);
            return row;
        }

        private GameObject ColorPickerRow(GameObject parent) {
            GameObject row = Row(parent, "ColorPickerRow", TextAlignment.Left);
            ColorPicker(settingsPanel, row);
            return row;
        }

        private GameObject SkyboxRow(GameObject parent) {
            GameObject row = Row(parent, "SkyboxRow", TextAlignment.Left);
            SkyboxButton(settingsPanel, row, "Space", "space");
            SkyboxButton(settingsPanel, row, "Sunset", "sunset");
            SkyboxButton(settingsPanel, row, "Blue Sky", "bluesky");
            SkyboxButton(settingsPanel, row, "Milky Way", "milkyway");
            return row;
        }

        private void CreateSettingsPanel(GameObject parent) {
            settingsPanelInstance = Panel(parent, "SettingsPanelInstance");
            settingsPanel = settingsPanelInstance.GetComponent<StandardPanel>();
            GameObject column = Column(settingsPanelInstance);

            LabelRow(column, "Settings");
            SnapsRow(column);

            AlignmentToolsRow(column);
            TracingRow(column);

            LabelRow(column, "Fill Color");
            ColorPickerRow(column);

            LabelRow(column, "Skybox");
            SkyboxRow(column);
        }

        public override GameObject Generate() {
            base.Generate();

#if UNITY_EDITOR
            Undo.SetCurrentGroupName("SettingsPanelFactory Generate");
            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(this, "SettingsPanelFactory state");
#endif
            CreateDisposable(parent);
            CreateSettingsPanel(parent);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(disposable);
            Undo.CollapseUndoOperations(group);
#else
            Destroy(disposable);
#endif
            return settingsPanelInstance;
        }
    }
}