using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if VRTK
using CreateThis.VRTK;
#endif
using CreateThis.Unity;
using CreateThis.VR.UI.Panel;
using CreateThis.VR.UI.File;
using MMVR.Factory.UI.Button;
using MeshEngine;

namespace MMVR.Factory.UI.Panel {
    public class MMVR_ToolsPanelFactory : MMVR_BasePanelFactory {
        public FileOpen fileOpen;
        public FileSaveAs fileSaveAs;

        private GameObject toolsPanelInstance;
        private StandardPanel toolsPanel;

        protected GameObject ModeButton(StandardPanel panel, GameObject parent, string buttonText, ModeType mode) {
            ModeButtonFactory factory = Undoable.AddComponent<ModeButtonFactory>(disposable);
            SetToggleButtonValues(factory, panel, parent);
            factory.buttonText = buttonText;
            factory.mode = mode;
            return GenerateToggleButtonAndSetPosition(factory);
        }

        protected GameObject ResetButton(StandardPanel panel, GameObject parent, string buttonText) {
            ResetButtonFactory factory = Undoable.AddComponent<ResetButtonFactory>(disposable);
            SetMomentaryButtonValues(factory, panel, parent);
            factory.buttonText = buttonText;
            return GenerateMomentaryButtonAndSetPosition(factory);
        }

        private void CreateDisposable(GameObject parent) {
            if (disposable) return;
            disposable = EmptyChild(parent, "disposable");
        }

        private GameObject VertexRow(GameObject parent) {
            GameObject row = Row(parent, "VertexRow", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Add/Move", ModeType.Vertex);
            ModeButton(toolsPanel, row, "Delete", ModeType.Delete);
            return row;
        }

        private GameObject TriangleRow(GameObject parent) {
            GameObject row = Row(parent, "TriangleRow", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Create", ModeType.Face);
            ModeButton(toolsPanel, row, "Delete", ModeType.TriangleDelete);
            ModeButton(toolsPanel, row, "Flip Normal", ModeType.Normal);
            ModeButton(toolsPanel, row, "Fill", ModeType.Fill);
            ModeButton(toolsPanel, row, "Pick Color", ModeType.PickColor);
            return row;
        }

        private GameObject PrimitivesRow(GameObject parent) {
            GameObject row = Row(parent, "PrimitivesRow", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Plane", ModeType.PrimitivePlane);
            ModeButton(toolsPanel, row, "Box", ModeType.PrimitiveBox);
            ModeButton(toolsPanel, row, "Circle", ModeType.PrimitiveCircle);
            ModeButton(toolsPanel, row, "Cylinder", ModeType.PrimitiveCylinder);
            ModeButton(toolsPanel, row, "Sphere", ModeType.PrimitiveSphere);
            return row;
        }

        private GameObject VertexSelectionRow1(GameObject parent) {
            GameObject row = Row(parent, "VertexSelection1", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Select Vertices", ModeType.SelectVertices);
            ModeButton(toolsPanel, row, "Select Verts by Triangles", ModeType.SelectVerticesByTriangles);
            return row;
        }

        private GameObject VertexSelectionRow2(GameObject parent) {
            GameObject row = Row(parent, "VertexSelection2", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Deselect All", ModeType.DeselectAll);
            ModeButton(toolsPanel, row, "Copy", ModeType.Copy);
            ModeButton(toolsPanel, row, "Paste", ModeType.Paste);
            ModeButton(toolsPanel, row, "Delete", ModeType.DeleteSelection);
            return row;
        }

        private GameObject VertexSelectionRow3(GameObject parent) {
            GameObject row = Row(parent, "VertexSelection3", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Box Select", ModeType.BoxSelect);
            ModeButton(toolsPanel, row, "Extrude", ModeType.Extrude);
            ModeButton(toolsPanel, row, "Flip Normal", ModeType.SelectionFlipNormal);
            ModeButton(toolsPanel, row, "Fill", ModeType.FillSelection);
            return row;
        }

        private GameObject VertexSelectionRow4(GameObject parent) {
            GameObject row = Row(parent, "VertexSelection4", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Merge 2 Vertices", ModeType.MergeVertices);
            return row;
        }

        private GameObject TriangleSelectionRow(GameObject parent) {
            GameObject row = Row(parent, "TriangleSelectionRow", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Select Triangles", ModeType.SelectTriangles);
            ModeButton(toolsPanel, row, "Flip Normal", ModeType.SelectionFlipNormal);
            ModeButton(toolsPanel, row, "Fill", ModeType.FillSelection);
            return row;
        }

        private GameObject ObjectRow(GameObject parent) {
            GameObject row = Row(parent, "ObjectRow", TextAlignment.Left);
            ModeButton(toolsPanel, row, "Move Scale", ModeType.Object);
            ResetButton(toolsPanel, row, "Reset");
            return row;
        }

        private GameObject FileRow(GameObject parent) {
            GameObject row = Row(parent, "FileRow", TextAlignment.Left);
            PanelToggleVisibilityMomentaryButton(toolsPanel, row, "Open", fileOpen);
            PanelToggleVisibilityMomentaryButton(toolsPanel, row, "SaveAs", fileSaveAs);
            return row;
        }

        private GameObject AlignmentRow(GameObject parent) {
            GameObject row = Row(parent, "AlignmentRow", TextAlignment.Left);
            ModeButton(toolsPanel, row, "X Line", ModeType.AlignmentX);
            ModeButton(toolsPanel, row, "Y Line", ModeType.AlignmentY);
            ModeButton(toolsPanel, row, "Z Line", ModeType.AlignmentZ);
            ModeButton(toolsPanel, row, "3d", ModeType.Alignment3d);
            ModeButton(toolsPanel, row, "Delete", ModeType.AlignmentDelete);
            return row;
        }

        private void CreateToolsPanel(GameObject parent) {
            toolsPanelInstance = Panel(parent, "ToolsPanelInstance");
            toolsPanel = toolsPanelInstance.GetComponent<StandardPanel>();
            GameObject column = Column(toolsPanelInstance);

            LabelRow(column, "Tools");

            LabelRow(column, "Vertex");
            VertexRow(column);

            LabelRow(column, "Triangle");
            TriangleRow(column);

            LabelRow(column, "Primitives");
            PrimitivesRow(column);

            LabelRow(column, "Vertex Selection");
            VertexSelectionRow1(column);
            VertexSelectionRow2(column);
            VertexSelectionRow3(column);
            VertexSelectionRow4(column);

            LabelRow(column, "Triangle Selection");
            TriangleSelectionRow(column);

            LabelRow(column, "Object");
            ObjectRow(column);

            LabelRow(column, "File");
            FileRow(column);

            LabelRow(column, "Alignment");
            AlignmentRow(column);
        }

        public override GameObject Generate() {
            base.Generate();

#if UNITY_EDITOR
            Undo.SetCurrentGroupName("MMVR_ToolsPanelFactory Generate");
            int group = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(this, "MMVR_ToolsPanelFactory state");
#endif
            CreateDisposable(parent);
            CreateToolsPanel(parent);

#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(disposable);
            Undo.CollapseUndoOperations(group);
#else
            Destroy(disposable);
#endif
            return toolsPanelInstance;
        }
    }
}