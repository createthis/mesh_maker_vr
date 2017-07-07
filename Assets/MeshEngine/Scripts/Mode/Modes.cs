using System.Collections.Generic;

namespace MeshEngine {
    public static class Modes {
        public static List<ModeType> endingInSelection {
            get {
                return new List<ModeType>(new ModeType[] {
                    ModeType.SelectVertices,
                    ModeType.SelectVerticesByTriangles,
                    ModeType.PrimitiveCircle,
                    ModeType.PrimitiveCylinder,
                    ModeType.PrimitiveSphere,
                    ModeType.PrimitivePlane,
                    ModeType.PrimitiveBox,
                    ModeType.BoxSelect,
                });
            }
        }

        public static List<ModeType> endingInTriangleSelection {
            get {
                return new List<ModeType>(new ModeType[] {
                    ModeType.SelectTriangles,
                });
            }
        }

        public static List<ModeType> noDelete {
            get {
                return new List<ModeType>(new ModeType[] {
                    ModeType.Settings,
                    ModeType.Tools,
                    ModeType.Open,
                    ModeType.SaveAs,
                    ModeType.DeselectAll,
                    ModeType.Copy,
                    ModeType.Paste,
                    ModeType.DeleteSelection,
                    ModeType.FillSelection,
                    ModeType.SelectVertices,
                    ModeType.SelectVerticesByTriangles,
                    ModeType.MergeVertices,
                    ModeType.Extrude,
                    ModeType.SelectionFlipNormal,
                    ModeType.BoxSelect,
                    ModeType.RunTests,
                });
            }
        }

        public static List<ModeType> noSelectionClear {
            get {
                return new List<ModeType>(new ModeType[] {
                    ModeType.Copy,
                    ModeType.Paste,
                    ModeType.DeleteSelection,
                    ModeType.FillSelection,
                    ModeType.SelectVertices,
                    ModeType.SelectVerticesByTriangles,
                    ModeType.MergeVertices,
                    ModeType.Extrude,
                    ModeType.SelectionFlipNormal,
                    ModeType.BoxSelect,
                });
            }
        }

        public static List<ModeType> noTriangleSelectionClear {
            get {
                return new List<ModeType>(new ModeType[] {
                    ModeType.FillSelection,
                    ModeType.SelectionFlipNormal,
                    ModeType.SelectTriangles,
                });
            }
        }
    }
}