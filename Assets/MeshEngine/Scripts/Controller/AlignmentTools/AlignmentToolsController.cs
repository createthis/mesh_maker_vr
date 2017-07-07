using UnityEngine;
using System.IO;

namespace MeshEngine.Controller {
    public class AlignmentToolsController : MonoBehaviour {
        public Mesh mesh;
        public string filePath = @"alignment_tools.json";
        public GameObject alignmentXPrefab;
        public GameObject alignmentYPrefab;
        public GameObject alignmentZPrefab;
        public GameObject alignment3dPrefab;

        public JSONObject Vector3ToJSONObject(Vector3 position) {
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            j.AddField("x", position.x);
            j.AddField("y", position.y);
            j.AddField("z", position.z);
            return j;
        }

        public Vector3 JSONObjectToVector3(JSONObject j) {
            Vector3 position = new Vector3(j["x"].n, j["y"].n, j["z"].n);
            return position;
        }

        public JSONObject alignmentToolToJSONObject(GameObject alignmentTool) {
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
            j.AddField("type", alignmentTool.GetComponent<AlignmentToolController>().type);
            j.AddField("localPosition", Vector3ToJSONObject(alignmentTool.transform.localPosition));
            return j;
        }

        public GameObject prefabOfType(string type) {
            switch (type) {
                case "alignment_x":
                    return alignmentXPrefab;
                case "alignment_y":
                    return alignmentYPrefab;
                case "alignment_z":
                    return alignmentZPrefab;
                case "alignment_3d":
                    return alignment3dPrefab;
                default:
                    return null;
            }
        }

        public void AddAlignmentTool(string type, Vector3 localPosition) {
            GameObject alignmentInstance = Instantiate(prefabOfType(type), localPosition, Quaternion.identity);
            alignmentInstance.transform.parent = mesh.gameObject.transform;
            alignmentInstance.transform.localPosition = localPosition;
            alignmentInstance.transform.rotation = Quaternion.identity;
            mesh.alignmentTools.alignmentTools.Add(alignmentInstance);
            alignmentInstance.GetComponent<AlignmentToolController>().mesh = mesh;
        }

        public void Save() {
            JSONObject j = new JSONObject(JSONObject.Type.ARRAY);
            foreach (GameObject alignmentTool in mesh.alignmentTools.alignmentTools) {
                j.Add(alignmentToolToJSONObject(alignmentTool));
            }

            string encodedString = j.ToString();
            File.WriteAllText(filePath, encodedString);
        }

        public void Load() {
            if (!File.Exists(filePath)) {
                return;
            }
            string contents = File.ReadAllText(filePath);
            JSONObject obj = new JSONObject(contents);

            foreach (JSONObject alignmentToolObj in obj.list) {
                string type = alignmentToolObj["type"].str;
                Vector3 localPosition = JSONObjectToVector3(alignmentToolObj["localPosition"]);
                AddAlignmentTool(type, localPosition);
            }
        }

        // Use this for initialization
        void Awake() {
        }
    }
}