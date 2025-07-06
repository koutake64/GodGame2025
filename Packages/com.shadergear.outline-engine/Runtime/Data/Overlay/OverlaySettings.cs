using UnityEngine;
using System;

namespace SHADERGEAR.OutlineEngine
{
    /// <summary>
    /// Contains all settings found in the overlay tab.
    /// </summary>
    [Serializable]
    public class OverlaySettings
    {
        /// <summary>
        /// Whether to render the overlay using a color, a texture or not at all.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Whether to render the overlay using a color, a texture or not at all.")]
        public OverlayStyle Style { get; set; } = OverlayStyle.None;

        /// <summary>
        /// The overlay's color.
        /// </summary>
        [field: SerializeField]
        [field: ColorUsage(showAlpha: true, hdr: true)]
        [field: Tooltip("The overlay's color.")]
        public Color Color { get; set; } = Color.white;

        /// <summary>
        /// The overlay's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The overlay's texture.")]
        public Texture2D TextureSource { get; set; } = null;

        /// <summary>
        /// The scale of the overlay's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The scale of the overlay's texture.")]
        public Vector2 TextureScale { get; set; } = Vector2.one;

        /// <summary>
        /// The offset of the overlay's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The offset of the overlay's texture.")]
        public Vector2 TextureOffset { get; set; } = Vector2.zero;

        /// <summary>
        /// The rotation of the overlay's texture.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The rotation of the overlay's texture.")]
        public float TextureRotation { get; set; } = 0f;

        /// <summary>
        /// The speed of the overlay's texture animation.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The speed of the overlay's texture animation.")]
        public float TextureAnimationSpeed { get; set; } = 0f;

        /// <summary>
        /// The direction of the overlay's texture animation in degrees.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The direction of the overlay's texture animation in degrees.")]
        public float TextureAnimationDirection { get; set; } = 0f;

        /// <summary>
        /// Checks whether overlay settings are valid and changes them if necessary.
        /// </summary>
        public void Validate() { }
    }
}