namespace MeshEngine {
    public static class Mode {
        public static ModeType mode;
        public delegate void ModeChanged();
        public static event ModeChanged OnModeChanged;

        private static void Initialize() {
            mode = ModeType.Vertex;
        }

        public static void SetMode(ModeType value) {
            ModeType oldMode = mode;
            mode = value;

            Mesh mesh = Meshes.GetSelectedMesh();
            if (!mesh) return;

            //Debug.Log("oldMode=" + oldMode + ",mode=" + mode);
            if (!Modes.noDelete.Contains(mode)) {
                mesh.selection.ClearSelectedVertices();
                mesh.vertices.DeleteVertexInstances();
                mesh.triangles.DeleteTriangleInstances();
                mesh.alignmentTools.SetActiveCollidersOnAlignmentTools(false);
                mesh.SetRenderOptions(true, true, true);
                mesh.triangles.autoCreateTriangleObjects = false;
                mesh.triangles.SetTriangleInstancesSelectable(true);
            }

            if (mode != ModeType.SelectVerticesByTriangles) {
                mesh.vertices.SetVertexInstancesSelectable(true);
            }

            switch (mode) {
                case ModeType.AlignmentDelete:
                    mesh.alignmentTools.SetActiveCollidersOnAlignmentTools(true);
                    if (OnModeChanged != null) OnModeChanged();
                    return;
                case ModeType.Settings:
                    mode = oldMode;
                    if (OnModeChanged != null) OnModeChanged();
                    return;
                case ModeType.Tools:
                    mode = oldMode;
                    if (OnModeChanged != null) OnModeChanged();
                    return;
                case ModeType.Open:
                    mode = oldMode;
                    if (OnModeChanged != null) OnModeChanged();
                    return;
                case ModeType.SaveAs:
                    //meshController.keyboardPanelController.ToggleVisible();

                    mode = oldMode;
                    if (OnModeChanged != null) OnModeChanged();
                    return;
                case ModeType.DeselectAll:
                    mesh.selection.Clear();
                    mesh.copy.Clear();
                    mode = oldMode;
                    if (OnModeChanged != null) OnModeChanged();
                    return;
                case ModeType.Vertex:
                case ModeType.Face:
                case ModeType.Delete:
                    mesh.vertices.CreateVertexInstances();
                    break;
                case ModeType.PrimitiveSphere:
                case ModeType.PrimitiveCircle:
                case ModeType.PrimitiveCylinder:
                case ModeType.PrimitivePlane:
                case ModeType.PrimitiveBox:
                    mesh.vertices.CreateVertexInstances();
                    mesh.triangles.autoCreateTriangleObjects = true;
                    mesh.SetRenderOptions(false, false, false);
                    mesh.triangles.SetTriangleInstancesSelectable(false);
                    mesh.triangles.CreateTriangleInstances();
                    break;
                case ModeType.SelectVertices:
                    mesh.vertices.CreateVertexInstances(false);
                    mesh.triangles.SetTriangleInstancesSelectable(false);
                    mesh.triangles.CreateTriangleInstances();
                    mesh.SetRenderOptions(false, false, false);
                    break;
                case ModeType.SelectVerticesByTriangles:
                    mesh.vertices.SetVertexInstancesSelectable(false);
                    mesh.vertices.CreateVertexInstances(false);
                    mesh.triangles.SetTriangleInstancesSelectable(true);
                    mesh.triangles.CreateTriangleInstances();
                    mesh.SetRenderOptions(false, false, false);
                    break;
                case ModeType.BoxSelect:
                    mesh.vertices.CreateVertexInstances(false);
                    mesh.triangles.autoCreateTriangleObjects = true;
                    mesh.triangles.SetTriangleInstancesSelectable(false);
                    mesh.triangles.CreateTriangleInstances();
                    mesh.SetRenderOptions(false, false, false);
                    break;
                case ModeType.Normal:
                case ModeType.Fill:
                case ModeType.TriangleDelete:
                case ModeType.SelectTriangles:
                case ModeType.PickColor:
                    mesh.SetRenderOptions(false, false, false);
                    mesh.triangles.CreateTriangleInstances();
                    break;
            }

            if (!Modes.noSelectionClear.Contains(mode)) {
                mesh.selection.Clear();
                mesh.copy.Clear();
            }


            if (Modes.endingInTriangleSelection.Contains(oldMode) && !Modes.noTriangleSelectionClear.Contains(mode)) {
                mesh.selection.Clear();
                mesh.copy.Clear();
            }

            if (Modes.endingInSelection.Contains(oldMode) && mode == ModeType.DeleteSelection) {
                mesh.selection.DeleteSelected();
                mesh.triangles.DeleteTriangleInstances(); // This shouldn't be necessary, but there is a bug somewhere
                mesh.triangles.SetTriangleInstancesSelectable(false);
                mesh.triangles.CreateTriangleInstances(); // ditto
                mesh.SetRenderOptions(false, false, false);
                mode = ModeType.SelectVertices;
            }

            if ((Modes.endingInSelection.Contains(oldMode) || Modes.endingInTriangleSelection.Contains(oldMode)) && mode == ModeType.FillSelection) {
                mesh.selection.FillSelected();
                mesh.SetRenderOptions(false, false, false);
                mode = (oldMode == ModeType.SelectTriangles) ? ModeType.SelectTriangles : ModeType.SelectVertices;
            }

            if (Modes.endingInSelection.Contains(oldMode) && mode == ModeType.Extrude) {
                mesh.extrusion.ExtrudeSelected(true);
                mesh.SetRenderOptions(false, false, false);
                mesh.triangles.SetTriangleInstancesSelectable(false);
                mode = ModeType.SelectVertices;
            }

            if (oldMode == ModeType.SelectVertices && mode == ModeType.MergeVertices) {
                mesh.vertices.MergeSelected();
                mesh.SetRenderOptions(false, false, false);
                mode = ModeType.SelectVertices;
            }

            if ((Modes.endingInSelection.Contains(oldMode) || Modes.endingInTriangleSelection.Contains(oldMode)) && mode == ModeType.SelectionFlipNormal) {
                mesh.triangles.FlipNormalsOfSelection();
                mesh.SetRenderOptions(false, false, false);
                mode = (oldMode == ModeType.SelectTriangles) ? ModeType.SelectTriangles : ModeType.SelectVertices;
            }

            if (Modes.endingInSelection.Contains(oldMode) && mode == ModeType.Copy) {
                mesh.copy.CopySelection();
                mesh.SetRenderOptions(false, false, false);
                mode = ModeType.SelectVertices;
            }

            if (Modes.endingInSelection.Contains(oldMode) && mode == ModeType.Paste) {
                mesh.selection.Clear();
                mesh.triangles.autoCreateTriangleObjects = true;
                mesh.copy.Paste();
                mesh.SetRenderOptions(false, false, false);
                mode = ModeType.SelectVertices;
            }

            if (oldMode != ModeType.Object && mode == ModeType.Object) {
                mesh.triangles.trianglesChanged = true;
                mesh.EnableBoxCollider();
            }

            if (oldMode == ModeType.Object && mode != ModeType.Object) {
                mesh.DisableBoxCollider();
            }
            if (OnModeChanged != null) OnModeChanged();
        }

        public static void UpdateMode() {
            Mesh mesh = Meshes.GetSelectedMesh();
            if (!mesh) return;

            mesh.alignmentTools.SetActiveCollidersOnAlignmentTools(false);
            if (mode == ModeType.AlignmentDelete) {
                mesh.alignmentTools.SetActiveCollidersOnAlignmentTools(true);
            }
            switch (mode) {
                case ModeType.AlignmentX:
                case ModeType.AlignmentY:
                case ModeType.AlignmentZ:
                case ModeType.Alignment3d:
                case ModeType.AlignmentDelete:
                    /*
                    if (!Settings.AlignmentToolsEnabled()) {
                        mode = ModeType."vertex";
                    }
                    */
                    break;
            }
        }
    }
}