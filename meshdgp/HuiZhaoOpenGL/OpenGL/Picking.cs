﻿//using System;
//using System.IO;
//using System.Drawing;
//using System.Diagnostics;

//using OpenTK; 
//using OpenTK.Graphics.OpenGL;




//namespace GraphicsResearch
//{
//    /// <summary>
//    /// This demo shows over which triangle the cursor is, it does so by assigning all 3 vertices of a triangle the same Ids.
//    /// Each Id is a uint, split into 4 bytes and used as triangle color. In an extra pass, the screen is cleared to uint.MaxValue,
//    /// and then the mesh is drawn using color. Using GL.ReadPixels() the value under the mouse cursor is read and can be converted.
//    /// </summary>
 
//    class Picking : IRender 
//    {
//        MeshView3D meshView;

//        /// <summary>Creates a 800x600 window with the specified title.</summary>
//        public Picking()
//        {
             
//        }

//        struct Byte4
//        {
//            public byte R, G, B, A;

//            public Byte4(byte[] input)
//            {
//                R = input[0];
//                G = input[1];
//                B = input[2];
//                A = input[3];
//            }

//            public uint ToUInt32()
//            {
//                byte[] temp = new byte[] { this.R, this.G, this.B, this.A };
//                return BitConverter.ToUInt32(temp, 0);
//            }

//            public override string ToString()
//            {
//                return this.R + ", " + this.G + ", " + this.B + ", " + this.A;
//            }
//        }

//        struct Vertex
//        {
//            public Byte4 Color; // 4 bytes
//            public Vector3 Position; // 12 bytes

//            public const byte SizeInBytes = 16;
//        }

//        const TextureTarget Target = TextureTarget.TextureRectangleArb;
//        float angle;
//        BeginMode VBO_PrimMode;
//        Vertex[] VBO_Array;
//        uint VBO_Handle;

//        uint SelectedTriangle;

//        // int VertexShaderObject, FragmentShaderObject, this.handle;

//        /// <summary>Load resources here.</summary>
//        /// <param name="e">Not used.</param>
//        private void OnLoad()
//        {
//            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);
//            GL.Enable(EnableCap.DepthTest);
//            GL.Enable(EnableCap.CullFace);
           
//            #region prepare data for VBO from procedural object
//            DrawableShape temp_obj = new SierpinskiTetrahedron(3f, SierpinskiTetrahedron.eSubdivisions.Five, false);
//            VertexT2fN3fV3f[] temp_VBO;
//            uint[] temp_IBO;
//            temp_obj.GetArraysforVBO(out VBO_PrimMode, out temp_VBO, out temp_IBO);
//            temp_obj.Dispose();
//            if (temp_IBO != null)
//                throw new Exception("Expected data for GL.DrawArrays, but Element Array is not null.");

//            // Convert from temp mesh to final object, copy position and add triangle Ids for the color attribute.
//            VBO_Array = new Vertex[temp_VBO.Length];
//            int TriangleCounter = -1;
//            for (int i = 0; i < temp_VBO.Length; i++)
//            {
//                // Position
//                VBO_Array[i].Position = temp_VBO[i].Position;

//                // Index
//                if (i % 3 == 0)
//                    TriangleCounter++;
//                VBO_Array[i].Color = new Byte4(BitConverter.GetBytes(TriangleCounter));
//            }
//            #endregion prepare data for VBO from procedural object

//            #region Setup VBO for drawing
//            GL.GenBuffers(1, out VBO_Handle);
//            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_Handle);
//            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(VBO_Array.Length * Vertex.SizeInBytes), VBO_Array, BufferUsageHint.StaticDraw);
//            GL.InterleavedArrays(InterleavedArrayFormat.C4ubV3f, 0, IntPtr.Zero);

//            ErrorCode err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Trace.WriteLine("VBO Setup failed (Error: " + err + "). Attempting to continue.");
//            #endregion Setup VBO for drawing

//            #region Shader
//            /*
//            // Load&Compile Vertex Shader

//            using (StreamReader sr = new StreamReader("Data/Shaders/Picking_VS.glsl"))
//            {
//                VertexShaderObject = GL.CreateShader(ShaderType.VertexShader);
//                GL.ShaderSource(VertexShaderObject, sr.ReadToEnd());
//                GL.CompileShader(VertexShaderObject);
//            }

//            err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Trace.WriteLine("Vertex Shader: " + err);

//            string LogInfo;
//            GL.GetShaderInfoLog(VertexShaderObject, out LogInfo);
//            if (LogInfo.Length > 0 && !LogInfo.Contains("hardware"))
//                Trace.WriteLine("Vertex Shader failed!\nLog:\n" + LogInfo);
//            else
//                Trace.WriteLine("Vertex Shader compiled without complaint.");

//            // Load&Compile Fragment Shader

//            using (StreamReader sr = new StreamReader("Data/Shaders/Picking_FS.glsl"))
//            {
//                FragmentShaderObject = GL.CreateShader(ShaderType.FragmentShader);
//                GL.ShaderSource(FragmentShaderObject, sr.ReadToEnd());
//                GL.CompileShader(FragmentShaderObject);
//            }
//            GL.GetShaderInfoLog(FragmentShaderObject, out LogInfo);

//            err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Trace.WriteLine("Fragment Shader: " + err);

//            if (LogInfo.Length > 0 && !LogInfo.Contains("hardware"))
//                Trace.WriteLine("Fragment Shader failed!\nLog:\n" + LogInfo);
//            else
//                Trace.WriteLine("Fragment Shader compiled without complaint.");

//            // Link the Shaders to a usable Program
//            this.handle = GL.CreateProgram();
//            GL.AttachShader(this.handle, VertexShaderObject);
//            GL.AttachShader(this.handle, FragmentShaderObject);

//            // link it all together
//            GL.LinkProgram(this.handle);

//            err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Trace.WriteLine("LinkProgram: " + err);

//            GL.UseProgram(this.handle);

//            err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Trace.WriteLine("UseProgram: " + err);

//            // flag ShaderObjects for delete when not used anymore
//            GL.DeleteShader(VertexShaderObject);
//            GL.DeleteShader(FragmentShaderObject);

//            int temp;
//            GL.GetProgram(this.handle, ProgramParameter.LinkStatus, out temp);
//            Trace.WriteLine("Linking Program (" + this.handle + ") " + ((temp == 1) ? "succeeded." : "FAILED!"));
//            if (temp != 1)
//            {
//                GL.GetProgramInfoLog(this.handle, out LogInfo);
//                Trace.WriteLine("Program Log:\n" + LogInfo);
//            }

//            Trace.WriteLine("End of Shader build. GL Error: " + GL.GetError());

//            GL.UseProgram(0);
//*/
//            #endregion Shader

//        }

//        private void OnUnload( )
//        {
//            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
//            GL.DeleteBuffers(1, ref VBO_Handle);

         
//        }

//        /// <summary>
//        /// Called when your window is resized. Set your viewport here. It is also
//        /// a good place to set up your projection matrix (which probably changes
//        /// along when the aspect ratio of your window).
//        /// </summary>
//        /// <param name="e">Contains information on the new Width and Size of the GameWindow.</param>
//        private void OnResize( )
//        {
//            GL.Viewport(0, 0, meshView.Width, meshView.Height);

//            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, meshView.Width / (float)meshView.Height, 0.1f, 10.0f);
//            GL.MatrixMode(MatrixMode.Projection);
//            GL.LoadMatrix(ref projection);
//        }

        
//        /// <summary>
//        /// Called when it is time to render the next frame. Add your rendering code here.
//        /// </summary>
//        /// <param name="e">Contains timing information.</param>
//        private void OnRenderFrame(double time)
//        {
//            GL.Color3(Color.White);
//            GL.Enable(EnableCap.ColorArray);

//            #region Pass 1: Draw Object and pick Triangle
//            GL.ClearColor(1f, 1f, 1f, 1f); // clears to uint.MaxValue
//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

//            Matrix4 modelview = Matrix4.LookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
//            GL.MatrixMode(MatrixMode.Modelview);
//            GL.LoadMatrix(ref modelview);
//            GL.Translate(0f, 0f, -3f);
//            GL.Rotate(angle, Vector3.UnitX);
//            GL.Rotate(angle, Vector3.UnitY);
//            angle += (float)time * 3.0f;

//            // You may re-enable the shader, but it works perfectly without and will run on intel HW too
//            // GL.UseProgram(this.handle);
//            GL.DrawArrays(VBO_PrimMode, 0, VBO_Array.Length);
//            // GL.UseProgram(0);

//            // Read Pixel under mouse cursor
//            Byte4 Pixel = new Byte4();
//            GL.ReadPixels((int)meshView.Tool.MouseCurrPos.x, (int)meshView.Tool.MouseCurrPos.y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref Pixel);
//            SelectedTriangle = Pixel.ToUInt32();
//            #endregion Pass 1: Draw Object and pick Triangle

//            GL.Color3(Color.White);
//            GL.Disable(EnableCap.ColorArray);

//            #region Pass 2: Draw Shape
//            if (SelectedTriangle == uint.MaxValue)
//                GL.ClearColor(.2f, .1f, .3f, 1f); // purple
//            else
//                GL.ClearColor(0f, .2f, .3f, 1f); // cyan
//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

//            GL.Color3(1f, 1f, 1f);
//            GL.DrawArrays(VBO_PrimMode, 0, VBO_Array.Length);

//            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
//            GL.Color3(Color.Red);
//            GL.DrawArrays(VBO_PrimMode, 0, VBO_Array.Length);
//            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

//            if (SelectedTriangle != uint.MaxValue)
//            {
//                GL.Disable(EnableCap.DepthTest);
//                GL.Color3(Color.Green);
//                GL.DrawArrays(VBO_PrimMode, (int)SelectedTriangle * 3, 3);
//                GL.Enable(EnableCap.DepthTest);
//            }
//            #endregion Pass 2: Draw Shape

//            meshView.SwapBuffers();

//            ErrorCode err = GL.GetError();
//            if (err != ErrorCode.NoError)
//                Trace.WriteLine("Error at Swapbuffers: " + err);
//        }



//        #region IRender Members

//        public void Resize(int width, int height)
//        {
//            this.OnResize();
//        }

//        public void Render()
//        {
//            this.OnRenderFrame(0);
//        }

//        public void Init(MeshView3D meshView)
//        {
//            meshView.MakeCurrent();
//            this.OnLoad();
//            this.meshView = meshView;
//        }

//        #endregion

//        #region IDisposable Members

//        public void Dispose()
//        {
//            this.OnUnload();
//        }

//        #endregion
//    }
//}