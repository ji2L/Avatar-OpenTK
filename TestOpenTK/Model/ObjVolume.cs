using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using OpenTK;

namespace TestOpenTK.Model
{
    class ObjVolume : Volume
    {
        #region Fields

        protected Vector3[] vertices;
        protected Vector3[] colors;
        protected Vector2[] texturecoords;

        protected List<Tuple<int, int, int>> faces = new List<Tuple<int, int, int>>();

        public override int VertCount { get { return vertices.Length; } }
        public override int IndiceCount { get { return faces.Count * 3; } }
        public override int ColorDataCount { get { return colors.Length; } }

        #endregion

        #region Methods

        /// <summary>
        /// Get vertices for this object.
        /// </summary>
        /// <returns>Returns an array of vertices.</returns>
        public override Vector3[] GetVerts()
        {
            return vertices;
        }

        /// <summary>
        /// Get indices to draw this object.
        /// </summary>
        /// <param name="offset">Number of vertices buffered before this object.</param>
        /// <returns>Returns an array of indices with offset applied.</returns>
        public override int[] GetIndices(int offset = 0)
        {
            List<int> temp = new List<int>();

            foreach (var face in faces)
            {
                temp.Add(face.Item1 + offset);
                temp.Add(face.Item2 + offset);
                temp.Add(face.Item3 + offset);
            }

            return temp.ToArray();
        }

        /// <summary>
        /// Get color data.
        /// </summary>
        /// <returns>Returns an array of colors.</returns>
        public override Vector3[] GetColorData()
        {
            return colors;
        }

        /// <summary>
        /// Get texture coordinates.
        /// </summary>
        /// <returns>Returns an array of texture coordinates.</returns>
        public override Vector2[] GetTextureCoords()
        {
            return texturecoords;
        }

        /// <summary>
        /// Calculates the model matrix from transforms.
        /// </summary>
        public override void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(Position);
        }

        /// <summary>
        /// Loads a .obj file from its filename (path).
        /// </summary>
        /// <param name="filename">The name (path) of the file.</param>
        /// <returns>Returns parsed data as an usable ObjVolume object.</returns>
        public static ObjVolume LoadFromFile(string filename)
        {
            ObjVolume obj = new ObjVolume();

            try
            {
                // Extract the content of the file and send it to the parser.
                using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
                {
                    obj = LoadFromString(reader.ReadToEnd());
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found: {0}", filename);
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading file: {0}", filename);
                Console.WriteLine(e.ToString());
            }

            return obj;
        }

        /// <summary>
        /// Loads a .obj file from its content. Called by LoadFromFile.
        /// </summary>
        /// <param name="obj">The content of the .obj file.</param>
        /// <returns>Returns parsed data as an usable ObjVoume object.</returns>
        protected static ObjVolume LoadFromString(string obj)
        {
            // Constants used for parsing
            NumberStyles style = NumberStyles.Float;
            CultureInfo culture = CultureInfo.InvariantCulture;

            // Seperate lines from the file
            List<String> lines = new List<string>(obj.Split('\n'));

            // Lists to hold model data
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> colors = new List<Vector3>();
            List<Vector2> texs = new List<Vector2>();
            List<Tuple<int, int, int>> faces = new List<Tuple<int, int, int>>();

            // Read file line by line
            foreach (String line in lines)
            {
                if (line.StartsWith("v ")) // Vertex definition
                {
                    // Cut off beginning of line
                    String temp = line.Substring(2);

                    Vector3 vec = new Vector3();

                    if (temp.Count((char c) => c == ' ') == 2) // Check if there's enough elements for a vertex
                    {
                        String[] vertparts = temp.Split(' ');

                        // Attempt to parse each part of the vertice
                        bool success = float.TryParse(vertparts[0], style, culture, out vec.X);
                        success |= float.TryParse(vertparts[1], style, culture, out vec.Y);
                        success |= float.TryParse(vertparts[2], style, culture, out vec.Z);

                        // Dummy color/texture coordinates for now
                        colors.Add(new Vector3((float)Math.Sin(vec.Z), (float)Math.Sin(vec.Z), (float)Math.Sin(vec.Z)));
                        texs.Add(new Vector2((float)Math.Sin(vec.Z), (float)Math.Sin(vec.Z)));

                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Console.WriteLine("Error parsing vertex: {0}", line);
                        }
                    }

                    verts.Add(vec);
                }
                else if (line.StartsWith("f ")) // Face definition
                {
                    // Cut off beginning of line
                    String temp = line.Substring(2);

                    Tuple<int, int, int> face = new Tuple<int, int, int>(0, 0, 0);

                    if (temp.Count((char c) => c == ' ') == 2) // Check if there's enough elements for a face
                    {
                        String[] faceparts = temp.Split(' ');

                        int i1, i2, i3;

                        // Attempt to parse each part of the face
                        bool success = int.TryParse(faceparts[0], style, culture, out i1);
                        success |= int.TryParse(faceparts[1], style, culture, out i2);
                        success |= int.TryParse(faceparts[2], style, culture, out i3);

                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Console.WriteLine("Error parsing face: {0}", line);
                        }
                        else
                        {
                            // Decrement to get zero-based vertex numbers
                            face = new Tuple<int, int, int>(i1 - 1, i2 - 1, i3 - 1);
                            faces.Add(face);
                        }
                    }
                }
            }

            // Create the ObjVolume object
            ObjVolume vol = new ObjVolume();
            vol.vertices = verts.ToArray();
            vol.faces = new List<Tuple<int, int, int>>(faces);
            vol.colors = colors.ToArray();
            vol.texturecoords = texs.ToArray();

            return vol;
        }

        #endregion
    }
}
