using OpenTK;

namespace TestOpenTK
{
    /// <summary>
    /// Abstract class which represents a volume we want to display.
    /// </summary>
    public abstract class Volume
    {
        #region Fields

        public virtual int VertCount { get; set; }
        public virtual int IndiceCount { get; set; }
        public virtual int ColorDataCount { get; set; }

        public bool IsTextured = false;
        public int TextureID;
        public int TextureCoordsCount;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        #endregion

        #region Methods

        /// <summary>
        /// Getter for this volume's vertices.
        /// </summary>
        /// <returns>Returns this volume's vertices as a Vertex3[].</returns>
        public abstract Vector3[] GetVerts();

        /// <summary>
        /// Getter for this volume's indices.
        /// </summary>
        /// <param name="offset">Number of vertices buffered before this volume.</param>
        /// <returns>Returns an array of indices with offset applied.</returns>
        public abstract int[] GetIndices(int offset = 0);

        /// <summary>
        /// Getter for this volume's color data.
        /// </summary>
        /// <returns>Returns this volume's colors as a Vector3[].</returns>
        public abstract Vector3[] GetColorData();

        /// <summary>
        /// Calculates the model matrix from transforms.
        /// </summary>
        public abstract void CalculateModelMatrix();

        /// <summary>
        /// Getter for this volume's textures coordinates.
        /// </summary>
        /// <returns>Returns this volume's textures coordinates as a Vector2[].</returns>
        public abstract Vector2[] GetTextureCoords();

        #endregion
    }
}
