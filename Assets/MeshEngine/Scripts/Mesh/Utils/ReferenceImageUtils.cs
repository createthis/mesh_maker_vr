using UnityEngine;
using System.IO;
using CreateThis.Math;
using CreateThis.Unity;
using CreateThis.VR.UI.Interact;

namespace MeshEngine {
    public static class ReferenceImageUtils {
        private static float defaultScale = 0.2f;
        private static bool lastReferenceImagesLocked;

        static Texture2D FlipTextureVertical(Texture2D original) {
            Texture2D flipped = new Texture2D(original.width, original.height);
            Color[] pix = original.GetPixels(0, 0, original.width, original.height);
            Debug.Log("pix.Length=" + pix.Length);
            System.Array.Reverse(pix, 0, pix.Length);
            //for (int row = 0; row < original.height; ++row)
            //    System.Array.Reverse(pix, row * original.width, original.height);
            flipped.SetPixels(0, 0, original.width, original.height, pix);
            flipped.Apply();

            return flipped;
        }

        static Texture2D FlipTextureHorizontal(Texture2D original) {
            Texture2D flipped = new Texture2D(original.width, original.height);

            int xN = original.width;
            int yN = original.height;


            for (int i = 0; i < xN; i++) {
                for (int j = 0; j < yN; j++) {
                    flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                }
            }
            flipped.Apply();

            return flipped;
        }

        public static void LoadTexture(string filePath, GameObject referenceImage, bool reverseTexture = false) {
            if (!File.Exists(filePath)) {
                Debug.Log("LoadTexture does not exist filePath=" + filePath);
                referenceImage.SetActive(false);
                return;
            }
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(File.ReadAllBytes(filePath));
            if (reverseTexture) {
                tex = FlipTextureHorizontal(FlipTextureVertical(tex));
            }
            Debug.Log("LoadTexture filePath=" + filePath + ", width=" + tex.width + ", height=" + tex.height);
            referenceImage.transform.localScale = new Vector3(Ratio.SolveForD(tex.height, tex.width, defaultScale), defaultScale, defaultScale);
            referenceImage.GetComponent<MeshRenderer>().materials[0].mainTexture = tex;

            Selectable selectable = referenceImage.GetComponent<Selectable>();
            selectable.Initialize();
            UnityEngine.Material[] unselectedMaterials = new UnityEngine.Material[1] { referenceImage.GetComponent<MeshRenderer>().materials[0] };
            Debug.Log("unselectedMaterials.Length=" + unselectedMaterials.Length);
            for (int i = 0; i < unselectedMaterials.Length; i++) {
                unselectedMaterials[i] = GameObject.Instantiate(unselectedMaterials[i]);
                unselectedMaterials[i].mainTexture = tex;
            }
            int textureCacheId = TextureCache.Add(tex);
            selectable.renderMesh = false;
            selectable.textureCacheId = textureCacheId;
            selectable.unselectedMaterial = null;
            selectable.unselectedMaterials = unselectedMaterials;
            selectable.UpdateSelectedMaterials();
        }
    }
}