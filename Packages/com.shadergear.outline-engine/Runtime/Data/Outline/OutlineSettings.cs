using UnityEngine;
using System;

namespace SHADERGEAR.OutlineEngine
{
    /// <summary>
    /// Contains all settings found in the outline tab.
    /// </summary>
    [Serializable]
    public class OutlineSettings
    {
        /// <summary>
        /// Whether to render the outline using a color, a texture or not at all.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether to render the outline using a color, a texture or not at all.")]
        public OutlineStyle Style { get; set; } = OutlineStyle.Color;

        /// <summary>
        /// The outline's width in pixels.
        /// </summary>
        [field: SerializeField]
        [field: Min(1)]
        [field: Tooltip("The outline's width in pixels.")]
        public int Width { get; set; } = 1;
        
        /// <summary>
        /// The outline's color.
        /// </summary>
        [field: SerializeField]
        [field: ColorUsage(showAlpha: true, hdr: true)]
        [field: Tooltip("The outline's color.")]
        public Color Color { get; set; } = Color.white;

        /// <summary>
        /// The outline's softness.
        /// </summary>
        [field: SerializeField]
        [field: Range(0f, 1f)]
        [field: Tooltip("The outline's softness.")]
        public float Softness { get; set; } = 0.05f;

        /// <summary>
        /// The outline's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The outline's texture.")]
        public Texture2D TextureSource { get; set; } = null;

        /// <summary>
        /// The scale of the outline's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The scale of the outline's texture.")]
        public Vector2 TextureScale { get; set; } = Vector2.one;

        /// <summary>
        /// The offset of the outline's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The offset of the outline's texture.")]
        public Vector2 TextureOffset { get; set; } = Vector2.zero;

        /// <summary>
        /// The rotation of the outline's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The rotation of the outline's texture.")]
        public float TextureRotation { get; set; } = 0f;

        /// <summary>
        /// The speed of the outline's texture animation.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The speed of the outline's texture animation.")]
        public float TextureAnimationSpeed { get; set; } = 0f;

        /// <summary>
        /// The direction of the outline's texture animation in degrees.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The direction of the outline's texture animation in degrees.")]
        public float TextureAnimationDirection { get; set; } = 0f;

        /// <summary>
        /// Checks whether outline settings are valid and changes them if necessary.
        /// </summary>
        public void Validate()
        {
            ValidateWidth();
            ValidateSoftness();
        }

        /// <summary>
        /// Makes width at least 1.
        /// </summary>
        void ValidateWidth()
        {
            Width = Mathf.Max(1, Width);
        }

        /// <summary>
        /// Clamps softness between 0 and 1.
        /// </summary>
        void ValidateSoftness()
        {
            Softness = Mathf.Clamp01(Softness);
        }
    }
}