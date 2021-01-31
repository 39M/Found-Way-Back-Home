using UnityEditor;
using UnityEngine;

namespace Pinwheel.Griffin
{
    [CustomEditor(typeof(GTerrainData))]
    public class GTerrainDataInspector : Editor
    {
        private GTerrainData instance;

        private void OnEnable()
        {
            instance = (GTerrainData)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Select the corresponding terrain in the scene to edit terrain data.", GEditorCommon.WordWrapItalicLabel);
            EditorGUILayout.LabelField("Do NOT duplicate this asset, it won't work. Instead use Export/Import function to copy data.", GEditorCommon.WarningLabel);
        }

        [MenuItem("CONTEXT/GShading/Delete Textures")]
        public static void DeleteTextures()
        {
            Object o = Selection.activeObject;
            if (o is GShading)
            {
                GShading shading = o as GShading;
                GUtilities.DestroyObject(shading.AlbedoMap);
                GUtilities.DestroyObject(shading.MetallicMap);
                Texture2D[] controls = shading.SplatControls;
                for (int i = 0; i < controls.Length; ++i)
                {
                    if (controls[i] != null)
                    {
                        GUtilities.DestroyObject(controls[i]);
                    }
                }
            }
        }
    }
}
