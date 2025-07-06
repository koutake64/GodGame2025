using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

namespace SHADERGEAR.OutlineEngine
{
    /// <summary>
    /// Contains all settings found in the rendering tab.
    /// </summary>
    [Serializable]
    public class RenderingSettings
    {
        /// <summary>
        /// Outlines and overlays are applied to objects on these rendering layers.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Outlines and overlays are applied to objects on these rendering layers.")]
        public RenderingLayerMask Targets { get; set; } = RenderingLayerMask.defaultRenderingLayerMask;

        /// <summary>
        /// Whether outlines and overlays are rendered before or after post processing.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether outlines and overlays are rendered before or after post processing.")]
        public RenderingEvent Event { get; set; } = RenderingEvent.RenderBeforePostProcessing;

        /// <summary>
        /// Whether outlines and overlays should be always visible or visible only if objects are in front or behind of others.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether outlines and overlays should be always visible or visible only if objects are in front or behind of others.")]
        public RenderingOcclusion Occlusion { get; set; } = RenderingOcclusion.RenderEverything;

        /// <summary>
        /// Objects on these rendering layers act as obstacles. Rendering layers already used in Targets are not allowed.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Objects on these rendering layers act as obstacles. Rendering layers already used in Targets are not allowed.")]
        public RenderingLayerMask Occluders { get; set; } = ~0;

        /// <summary>
        /// Whether to use a cutout texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether to use a cutout texture.")]
        public bool Cutout { get; set; } = false;

        /// <summary>
        /// The texture to cutout from.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The texture to cutout from.")]
        public Texture2D CutoutTexture { get; set; } = null;

        /// <summary>
        /// All values below this threshold are discarded.
        /// </summary>
        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("All values below this threshold are discarded.")]
        public float CutoutThreshold { get; set; } = 0.001f;

        /// <summary>
        /// The color channel to cutout from.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The color channel to cutout from.")]
        public CutoutChannel CutoutChannel { get; set; } = CutoutChannel.Alpha;

        /// <summary>
        /// Returns <see cref="RenderPassEvent"/> based on the current <see cref="Event"/>.
        /// </summary>
        public RenderPassEvent GetRenderPassEvent()
        {
            return Event switch
            {
                RenderingEvent.RenderBeforePostProcessing => RenderPassEvent.AfterRenderingTransparents,
                RenderingEvent.RenderAfterPostProcessing => RenderPassEvent.AfterRenderingPostProcessing,
                _ => throw new ArgumentOutOfRangeException(nameof(Event))
            };
        }

        /// <summary>
        /// Checks whether rendering settings are valid and changes them if necessary.
        /// </summary>
        public void Validate()
        {
            ValidateOccluders();
        }

        /// <summary>
        /// Makes the occluders mask not collide with the targets mask and contain only defined rendering layers.
        /// </summary>
        void ValidateOccluders()
        {
            Occluders &= ~Targets;
            Occluders &= RenderingLayerMask.GetDefinedRenderingLayersCombinedMaskValue();
        }
    }
}