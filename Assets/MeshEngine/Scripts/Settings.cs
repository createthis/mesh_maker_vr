using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MeshEngine.Controller;

namespace MeshEngine {
    public static class Settings {
        public enum ToggleType {
            Snap,
            TracingMode,
            CustomPointerLocation,
            AlignmentTools,
        }

        public static string filePath = @"settings.json";
        public static string skybox;
        public delegate void SettingsChanged();
        public static event SettingsChanged OnSettingsChanged;
        public static float rotationSnapIncrements = 15f; // FIXME: not persisted yet
        public static float snapIncrements = 0.025f;
        public static Vector3 customPointerPosition;
        public static Vector3 customPointerRotation;
        public static Color fillColor;

        private static Dictionary<ToggleType, bool> toggle;
        private static bool loaded;
        private static bool hasInitialized;

        public static void SetSkybox(string name) {
            Initialize();
            skybox = name;
            Save();
            UpdateSkybox();
            if (OnSettingsChanged != null) OnSettingsChanged();
        }

        public static void SetToggle(ToggleType toggleType, bool value) {
            Initialize();
            if (toggle.ContainsKey(toggleType)) toggle[toggleType] = value;
            else toggle.Add(toggleType, value);
            if (OnSettingsChanged != null) OnSettingsChanged();
        }

        public static bool GetToggle(ToggleType toggleType) {
            Initialize();
            if (toggle.ContainsKey(toggleType)) return toggle[toggleType];
            else return false;
        }

        public static bool SnapEnabled() {
            Initialize();
            return GetToggle(ToggleType.Snap) && snapIncrements != 0;
        }

        public static void SetSnap(bool value) {
            Initialize();
            SetToggle(ToggleType.Snap, value);
            Save();
        }

        public static bool AlignmentToolsEnabled() {
            Initialize();
            return GetToggle(ToggleType.AlignmentTools);
        }

        public static void SetAlignmentTools(bool value) {
            Initialize();
            SetToggle(ToggleType.AlignmentTools, value);
            Save();
        }

        public static void SetSnapIncrements(float value) {
            Initialize();
            snapIncrements = value;
            if (OnSettingsChanged != null) OnSettingsChanged();
            Save();
        }

        public static void SetTracingMode(bool value) {
            Initialize();
            SetToggle(ToggleType.TracingMode, value);
            Save();
        }

        public static bool TracingMode() {
            Initialize();
            return GetToggle(ToggleType.TracingMode);
        }


        public static void SetCustomPointerLocation(bool value) {
            Initialize();
            SetToggle(ToggleType.CustomPointerLocation, value);
            Save();
        }

        public static bool CustomPointerLocation() {
            Initialize();
            return GetToggle(ToggleType.CustomPointerLocation);
        }

        public static void SetCustomPointerPosition(Vector3 position) {
            Initialize();
            customPointerPosition = position;
            if (OnSettingsChanged != null) OnSettingsChanged();
            Save();
        }

        public static void SetCustomPointerRotation(Vector3 rotation) {
            Initialize();
            customPointerRotation = rotation;
            if (OnSettingsChanged != null) OnSettingsChanged();
            Save();
        }

        public static void SetFillColor(Color color) {
            Initialize();
            fillColor = color;
            if (OnSettingsChanged != null) OnSettingsChanged();
            Save();
        }

        public static void UpdateSkybox() {
            Initialize();
        }

        private static JSONObject Vector3ToJSONObject(Vector3 position) {
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            j.AddField("x", position.x);
            j.AddField("y", position.y);
            j.AddField("z", position.z);
            return j;
        }

        private static Vector3 JSONObjectToVector3(JSONObject j) {
            Vector3 position = new Vector3(j["x"].n, j["y"].n, j["z"].n);
            return position;
        }

        public static void Save() {
            if (!loaded) return;
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            j.AddField("skybox", skybox);
            j.AddField("snap", GetToggle(ToggleType.Snap));
            j.AddField("snapIncrements", snapIncrements);
            j.AddField("alignmentTools", GetToggle(ToggleType.AlignmentTools));
            j.AddField("tracingMode", GetToggle(ToggleType.TracingMode));
            j.AddField("customPointerLocation", GetToggle(ToggleType.TracingMode));
            j.AddField("customPointerPosition", Vector3ToJSONObject(customPointerPosition));
            j.AddField("customPointerRotation", Vector3ToJSONObject(customPointerRotation));
            j.AddField("fillColor", "#" + ColorUtility.ToHtmlStringRGBA(fillColor));

            string encodedString = j.ToString();
            File.WriteAllText(filePath, encodedString);
        }

        public static void Load() {
            if (!File.Exists(filePath)) {
                skybox = "space";
                SetToggle(ToggleType.TracingMode, false);
                SetToggle(ToggleType.CustomPointerLocation, false);
                customPointerPosition = new Vector3(0, 0, 0);
                customPointerRotation = new Vector3(0, 0, 0);
                fillColor = Color.red;

                return;
            }
            string contents = File.ReadAllText(filePath);
            JSONObject obj = new JSONObject(contents);

            if (obj["skybox"]) SetSkybox(obj["skybox"].str);
            if (obj["snap"]) SetToggle(ToggleType.Snap, obj["snap"].b);
            if (obj["snapIncrements"]) SetSnapIncrements(obj["snapIncrements"].n);
            if (obj["alignmentTools"]) SetToggle(ToggleType.AlignmentTools, obj["alignmentTools"].b);
            if (obj["tracingMode"]) SetToggle(ToggleType.TracingMode, obj["tracingMode"].b);
            if (obj["customPointerLocation"]) SetToggle(ToggleType.CustomPointerLocation, obj["customPointerLocation"].b);
            if (obj["customPointerPosition"]) SetCustomPointerPosition(JSONObjectToVector3(obj["customPointerPosition"]));
            if (obj["customPointerRotation"]) SetCustomPointerRotation(JSONObjectToVector3(obj["customPointerRotation"]));
            if (obj["fillColor"]) {
                Color outColor;
                Debug.Log("fillColor from file=" + obj["fillColor"].str);
                ColorUtility.TryParseHtmlString(obj["fillColor"].str, out outColor);
                SetFillColor(outColor);
            }
        }

        // Use this for initialization
        private static void Initialize() {
            if (hasInitialized) return;
            hasInitialized = true;
            toggle = new Dictionary<ToggleType, bool>();
            loaded = false;
            skybox = "space";
            fillColor = Color.red;
            Load();
            loaded = true;
            SetToggle(ToggleType.Snap, false);
            SetToggle(ToggleType.AlignmentTools, true);
        }
    }
}