using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MMVR {
    public class SkyboxManager : MonoBehaviour {
        public Material space;
        public Material blueSky;
        public Material sunset;
        public Material milkyWay;

        public delegate void SkyboxChanged();
        public static event SkyboxChanged OnSkyboxChanged;

        public string skybox;

        public void SetSkybox(string name) {
            skybox = name;
#if UNITY_EDITOR
            if (EditorApplication.isPlaying) {
#endif
                MeshEngine.Settings.SetSkybox(skybox);
                UpdateSkybox();
                if (OnSkyboxChanged != null) OnSkyboxChanged();
#if UNITY_EDITOR
            }
#endif
        }

        public Material StringToMaterial(string name) {
            switch (name) {
                case "space":
                    return space;
                case "sunset":
                    return sunset;
                case "bluesky":
                    return blueSky;
                case "milkyway":
                    return milkyWay;
                default:
                    return null;
            }
        }

        public void UpdateSkybox() {
            RenderSettings.skybox = StringToMaterial(skybox);
        }

        // Use this for initialization
        void Start() {
            skybox = "bluesky";
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
