using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace TestOpenTK
{
    class ShaderProgram
    {
        #region Fields and constructor

        // Stores the different IDs
        public int ProgramID = -1;
        public int VShaderID = -1;
        public int FShaderID = -1;
        public int AttributeCount = 0;
        public int UniformCount = 0;

        public Dictionary<string, AttributeInfo> Attributes = new Dictionary<string, AttributeInfo>();
        public Dictionary<string, UniformInfo> Uniforms = new Dictionary<string, UniformInfo>();
        public Dictionary<string, uint> Buffers = new Dictionary<string, uint>();

        /// <summary>
        /// Generic constructor.
        /// </summary>
        public ShaderProgram()
        {
            ProgramID = GL.CreateProgram();
        }

        /// <summary>
        /// Constructor which loads shaders, links them and generates buffer objects.
        /// </summary>
        /// <param name="vshader">Vertex shader.</param>
        /// <param name="fshader">Fragment shader.</param>
        /// <param name="fromFile">True : load from a file. False : load directly from the shader's code.</param>
        public ShaderProgram(String vshader, String fshader, bool fromFile = false)
        {
            ProgramID = GL.CreateProgram();

            if (fromFile)
            {
                LoadShaderFromFile(vshader, ShaderType.VertexShader);
                LoadShaderFromFile(fshader, ShaderType.FragmentShader);
            }
            else
            {
                LoadShaderFromString(vshader, ShaderType.VertexShader);
                LoadShaderFromString(fshader, ShaderType.FragmentShader);
            }

            Link();
            GenBuffers();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads a shader directly from its code. It's this function that does it in the end.
        /// </summary>
        /// <param name="code">The shader's code.</param>
        /// <param name="type">The tye of the shader (vertex/fragment).</param>
        /// <param name="address">The address at which the shader will be stored.</param>
        private void loadShader(String code, ShaderType type, out int address)
        {
            address = GL.CreateShader(type);
            GL.ShaderSource(address, code);
            GL.CompileShader(address);
            GL.AttachShader(ProgramID, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        /// <summary>
        /// Loads a shader from its code.
        /// </summary>
        /// <param name="code">The shader's code.</param>
        /// <param name="type">The tye of the shader (vertex/fragment).</param>
        public void LoadShaderFromString(String code, ShaderType type)
        {
            if (type == ShaderType.VertexShader)
            {
                loadShader(code, type, out VShaderID);
            }
            else if (type == ShaderType.FragmentShader)
            {
                loadShader(code, type, out FShaderID);
            }
        }

        /// <summary>
        /// Loads a shader from its name (path).
        /// </summary>
        /// <param name="filename">The name (path) of the shader.</param>
        /// <param name="type">The tye of the shader (vertex/fragment).</param>
        public void LoadShaderFromFile(String filename, ShaderType type)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                if (type == ShaderType.VertexShader)
                {
                    loadShader(sr.ReadToEnd(), type, out VShaderID);
                }
                else if (type == ShaderType.FragmentShader)
                {
                    loadShader(sr.ReadToEnd(), type, out FShaderID);
                }
            }
        }

        /// <summary>
        /// Links the shader.
        /// </summary>
        public void Link()
        {
            GL.LinkProgram(ProgramID);

            Console.WriteLine(GL.GetProgramInfoLog(ProgramID));

            GL.GetProgram(ProgramID, GetProgramParameterName.ActiveAttributes, out AttributeCount);
            GL.GetProgram(ProgramID, GetProgramParameterName.ActiveUniforms, out UniformCount);

            for (int i = 0; i < AttributeCount; i++)
            {
                AttributeInfo info = new AttributeInfo();
                int length = 0;

                StringBuilder name = new StringBuilder();

                GL.GetActiveAttrib(ProgramID, i, 256, out length, out info.size, out info.type, name);

                info.name = name.ToString();
                info.address = GL.GetAttribLocation(ProgramID, info.name);
                Attributes.Add(name.ToString(), info);
            }

            for (int i = 0; i < UniformCount; i++)
            {
                UniformInfo info = new UniformInfo();
                int length = 0;

                StringBuilder name = new StringBuilder();

                GL.GetActiveUniform(ProgramID, i, 256, out length, out info.size, out info.type, name);

                info.name = name.ToString();
                Uniforms.Add(name.ToString(), info);
                info.address = GL.GetUniformLocation(ProgramID, info.name);
            }
        }

        /// <summary>
        /// Generates buffer objects.
        /// </summary>
        public void GenBuffers()
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                uint buffer = 0;
                GL.GenBuffers(1, out buffer);

                Buffers.Add(Attributes.Values.ElementAt(i).name, buffer);
            }

            for (int i = 0; i < Uniforms.Count; i++)
            {
                uint buffer = 0;
                GL.GenBuffers(1, out buffer);

                Buffers.Add(Uniforms.Values.ElementAt(i).name, buffer);
            }
        }

        /// <summary>
        /// Calls EnableVertexAttribArray for every attributes.
        /// </summary>
        public void EnableVertexAttribArrays()
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                GL.EnableVertexAttribArray(Attributes.Values.ElementAt(i).address);
            }
        }

        /// <summary>
        /// Calls DisableVertexAttribArray for every attributes.
        /// </summary>
        public void DisableVertexAttribArrays()
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                GL.DisableVertexAttribArray(Attributes.Values.ElementAt(i).address);
            }
        }

        /// <summary>
        /// Returns a particular attribute.
        /// </summary>
        /// <param name="name">The name of the desired attribute.</param>
        /// <returns>The desired attribute if it exists, else returns -1.</returns>
        public int GetAttribute(string name)
        {
            if (Attributes.ContainsKey(name))
            {
                return Attributes[name].address;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns a particular uniform.
        /// </summary>
        /// <param name="name">The name of the desired uniform.</param>
        /// <returns>The desired uniform if it exists, else returns -1.</returns>
        public int GetUniform(string name)
        {
            if (Uniforms.ContainsKey(name))
            {
                return Uniforms[name].address;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns a particular buffer.
        /// </summary>
        /// <param name="name">The name of the desired buffer.</param>
        /// <returns>the desired buffer if it exists, else returns -1.</returns>
        public uint GetBuffer(string name)
        {
            if (Buffers.ContainsKey(name))
            {
                return Buffers[name];
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// Stores information about shader input (attributes).
        /// </summary>
        public class AttributeInfo
        {
            public String name = "";
            public int address = -1;
            public int size = 0;
            public ActiveAttribType type;
        }

        /// <summary>
        /// Stores information about shader input (uniforms).
        /// </summary>
        public class UniformInfo
        {
            public String name = "";
            public int address = -1;
            public int size = 0;
            public ActiveUniformType type;
        }

        #endregion
    }
}
