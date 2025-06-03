using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Dungeon_Crawler.Scripts
{
    internal class RendererTransparencyData
    {
        public readonly Material Opaque;
        public readonly Material Transparent;
        public readonly MaterialPropertyBlock Block = new();

        public Coroutine Fade;
        public float Alpha = 1f;
        public float TargetAlpha = 1f;   // new

        public RendererTransparencyData(Material original)
        {
            Opaque = new Material(original);
            Transparent = new Material(original);
            ConvertToTransparent(Transparent);
        }

        /// <summary>
        /// Converts a material to a transparent version for URP with alpha blending.
        /// Keeps depth writing on to prevent back-face blending artifacts.
        /// </summary>
        private static void ConvertToTransparent(Material mat)
        {
            // Set surface type to Transparent (URP-specific convention)
            mat.SetFloat("_Surface", 1f);  // 0 = Opaque, 1 = Transparent

            // Enable standard alpha blending
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            // Enable depth writing to avoid inner-face blending issues
            mat.SetInt("_ZWrite", 1);

            // Shader feature keywords for URP transparency
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");

            // Render queue just after default transparent range
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 1;
        }


    }

}

