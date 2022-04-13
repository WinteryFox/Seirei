using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Portals {
    [Serializable]
    [PostProcess (typeof(PortalRenderer), PostProcessEvent.AfterStack, "Custom/Portal")]
    public sealed class PortalShader : PostProcessEffectSettings {
        [Range (-0.002f, 1f), Tooltip ("Intensity of the bump map")]
        public FloatParameter refraction = new() { value = 0.015f };
    }

    public sealed class PortalRenderer : PostProcessEffectRenderer<PortalShader> {
        public override void Render (PostProcessRenderContext context) {
            var sheet = context.propertySheets.Get (Shader.Find ("Hidden/Custom/Portal"));
            sheet.properties.SetFloat ("_Refraction", settings.refraction);
            context.command.BlitFullscreenTriangle (context.source, context.destination, sheet, 0);
        }
    }
}
