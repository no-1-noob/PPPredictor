using HMUI;
using System;
using UnityEngine;
using HarmonyLib;
using TMPro;

namespace PPPredictor.HarmonyPatches
{
    [HarmonyLib.HarmonyPatch]
    static class HarmonyPatch
    {

        /// <summary>
        /// Prefix for the GenerateTextMesh function.
        /// Fix that hoverhints on a floatingscreen with no/0 curve radius would not reset to flat text after a hoverhint with a radius was displayed.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyLib.HarmonyPatch(typeof(CurvedTextMeshPro), nameof(CurvedTextMeshPro.GenerateTextMesh))]
        static bool Prefix(CurvedTextMeshPro __instance)
        {
            CurvedCanvasSettings curvedCanvasSettings = __instance._curvedCanvasSettingsHelper.GetCurvedCanvasSettings(__instance.canvas);
            //Only use this logic for the PPPredictor floadingScreen so nothing else can be broken
            if (curvedCanvasSettings != null && curvedCanvasSettings.name == "BSMLFloatingScreen_PPPredictor")
            {
                OriginalGenerateTextMesh(__instance);

                if (string.IsNullOrEmpty(__instance.m_text))
                {
                    return false;
                }

                float num = ((curvedCanvasSettings == null) ? 0f : curvedCanvasSettings.radius);
                
                //Continue applying 0 radius
                //if (Mathf.Approximately(num, 0f) && !__instance._useScriptableObjectColors)
                //{
                //    return false;
                //}

                Vector2 curveUV = new Vector2(num, 0f);
                int materialCount = __instance.m_textInfo.materialCount;
                if (__instance._curvedMeshInfos == null || materialCount > __instance._curvedMeshInfos.Length)
                {
                    Array.Resize(ref __instance._curvedMeshInfos, materialCount);
                }

                __instance.UpdateMesh(__instance.m_mesh, 0, curveUV, __instance.color);
                __instance.canvasRenderer.SetMesh(__instance.m_mesh);

                for (int i = 1; i < materialCount; i++)
                {
                    if (!(__instance.m_subTextObjects[i] == null))
                    {
                        Mesh mesh = __instance.m_textInfo.meshInfo[i].mesh;
                        __instance.UpdateMesh(mesh, i, curveUV, __instance.color);
                        __instance.m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
                    }
                }
                return false; //Do NOT call the BeatSaber implementation of GenerateTextMesh for this

            }
            else
            {
                return true; //Call the BeatSaber implementation of GenerateTextMesh for all other instances
            }
        }

        [HarmonyReversePatch]
        [HarmonyLib.HarmonyPatch(typeof(TextMeshProUGUI), "GenerateTextMesh")]
        public static void OriginalGenerateTextMesh(object instance)
        {
            // Stub. The Harmony reverse patch will inject the original functionality here.
            throw new NotImplementedException("It's a stub");
        }
    }
}
