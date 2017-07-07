using System.IO;
using UnityEngine;
using MeshEngine;
using CreateThis.VR.UI;
using CreateThis.VR.UI.Panel;
using CreateThis.VR.UI.File;


namespace MMVR {
    public class FileManager : MonoBehaviour {
        public FileSaveAs saveAsPanel;
        public StandardPanel unsavedPanel;
		public NotificationPanel notificationPanel;
		private string openPath;

        public void SaveAndOpen(string path = null) {
            if (path == null || path == "") path = openPath;
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            unsavedPanel.SetVisible(false);
            mesh.Save();
            OpenWithoutSave(path);
        }

        public void Open(string path, Transform controller, int controllerIndex) {
            Debug.Log("FileManager.Open");
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            if (mesh.persistence.changedSinceLastSave) {
                openPath = path;
                unsavedPanel.SetVisible(true, controller, controllerIndex);
            } else {
                OpenWithoutSave(path);
            }
        }

        public void OpenWithoutSave(string path = null) {
            if (path == null || path == "") path = openPath;
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            PanelManager.HideAllPanels(null);
            mesh.selection.ClearSelectedVertices();
            mesh.vertices.DeleteVertexInstances();
            mesh.triangles.DeleteTriangleInstances();
            mesh.persistence.Load(path);
            Mode.SetMode(Mode.mode);
        }

        public void SaveAs(string path, Transform controller, int controllerIndex) {
            Debug.Log("FileManager.SaveAs");
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            mesh.selection.ClearSelectedVertices();
            mesh.vertices.DeleteVertexInstances();
            mesh.triangles.DeleteTriangleInstances();
            mesh.persistence.Save(path);
            PanelManager.HideAllPanels(null);
            notificationPanel.DisplayMessage("Saved", controller, controllerIndex);
            Mode.SetMode(Mode.mode);
        }

        public void SaveFromUnityEvent(Transform controller, int controllerIndex) {
            Save(null, controller, controllerIndex);
        }

        public void Save(string path, Transform controller, int controllerIndex) {
            MeshEngine.Mesh mesh = Meshes.GetSelectedMesh();
            if (mesh.persistence.HasSavedOrLoaded()) {
                mesh.persistence.Save();
                notificationPanel.DisplayMessage("Saved", controller, controllerIndex);
            } else {
                saveAsPanel.SetVisible(true, controller, controllerIndex);
            }
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}