using System;
using OpenTK;

namespace TestOpenTK
{
    class Camera
    {
        #region Fields

        public Vector3 Position = Vector3.Zero;
        public Vector3 Orientation = new Vector3((float)Math.PI, 0f, 0f);

        #endregion

        #region Methods

        /// <summary>
        /// Computes the view matrix based on the orientation of the camera.
        /// </summary>
        /// <returns>Returns the view computed view matrix.</returns>
        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3();

            lookat.X = (float)(Math.Sin((float)Orientation.X) * Math.Cos((float)Orientation.Y));
            lookat.Y = (float)Math.Sin((float)Orientation.Y);
            lookat.Z = (float)(Math.Cos((float)Orientation.X) * Math.Cos((float)Orientation.Y));

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }

        #endregion
    }
}
