using SharpGL;
using SharpGL.VertexBuffers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerformanceDemo
{
    public partial class Form1 : Form
    {
        #region Fields
        private OpenGLControl openGLControl;

        /// <summary>
        /// 三百万个点数据
        /// </summary>
        private float[] vertices = new float[3 * 3 * 100 * 100 * 100];

        private float[] colors = new float[3 * 3 * 100 * 100 * 100];

        /// <summary>
        /// VBA
        /// </summary>
        private VertexBufferArray vertexBufferArray;

        /// <summary>
        /// 渲染线程
        /// </summary>
        private Thread renderThread;
        #endregion

        #region Constructors
        public Form1()
        {
            InitializeComponent();
            InitSharpGLControl();
            GenerateRandomVertices();
            InitVbo();
            this.Load += Form1_Load;
        }

        #endregion

        #region Methods
        /// <summary>
        /// 初始化线程（渲染等）
        /// </summary>
        private void InitThreads()
        {
            renderThread = new Thread(Render);
            renderThread.Start();
        }

        /// <summary>
        /// 渲染
        /// </summary>
        private void Render()
        {
            while (true)
            {

                var gl = openGLControl.OpenGL;
                gl.MakeCurrent();

                gl.Begin(OpenGL.GL_TRIANGLES);
                gl.Color(1.0f, 0, 0);
                gl.Vertex(-1.0f, -1, z);
                gl.Color(0, 1.0f, 0);
                gl.Vertex(1.0f, -1, z);
                gl.Color(0, 0, 1.0f);
                gl.Vertex(0, 1.0f, z);
                gl.End();
                openGLControl.Invoke(new MethodInvoker(() =>
                {
                    openGLControl.DoRender();
                }));
                
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 初始化Vbo
        /// </summary>
        private void InitVbo()
        {
            var gl = openGLControl.OpenGL;
            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            //  Create a vertex buffer for the vertex data.
            var vertexDataBuffer = new VertexBuffer();
            vertexDataBuffer.Create(gl);
            vertexDataBuffer.Bind(gl);
            vertexDataBuffer.SetData(gl, 0, vertices, false, 3);

            //  Now do the same for the colour data.
            var colourDataBuffer = new VertexBuffer();
            colourDataBuffer.Create(gl);
            colourDataBuffer.Bind(gl);
            colourDataBuffer.SetData(gl, 1, colors, false, 3);

            //  Unbind the vertex array, we've finished specifying data for it.
            vertexBufferArray.Unbind(gl);
        }

        /// <summary>
        /// 随机生成顶点数据
        /// </summary>
        private void GenerateRandomVertices()
        {
            Random rand = new Random();
            for(var i = 0; i < vertices.Length / 3; i++)
            {
                vertices[3 * i] = (float)rand.NextDouble();
                vertices[3 * i + 1] = (float)rand.NextDouble();
                vertices[3 * i + 2] = z;
            }

            for (var i = 0; i < colors.Length / 3; i++)
            {
                colors[3 * i] = (float)rand.NextDouble();
                colors[3 * i + 1] = (float)rand.NextDouble();
                colors[3 * i + 2] = (float)rand.NextDouble();
            }
        }

        /// <summary>
        /// 初始化sharpgl控件
        /// </summary>
        private void InitSharpGLControl()
        {
            openGLControl = new OpenGLControl();
            ((ISupportInitialize)openGLControl).BeginInit();
            openGLControl.Dock = DockStyle.Fill;
            openGLControl.RenderContextType = RenderContextType.FBO;
            openGLControl.OpenGLInitialized += OpenGLControl_OpenGLInitialized;
            openGLControl.Resize += OpenGLControl_Resize;
            openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
            ((ISupportInitialize)openGLControl).EndInit();
            splitContainer1.Panel1.Controls.Add(openGLControl);
        }

        /// <summary>
        /// sharpgl尺寸变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGLControl_Resize(object sender, EventArgs e)
        {
            var gl = openGLControl.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45.0, openGLControl.Width / openGLControl.Height, 0.01, 1000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            //gl.Viewport(0, 0, openGLControl.Width, openGLControl.Height);
        }

        private float z = -5;

        /// <summary>
        /// sharpgl绘制事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            //this.BeginInvoke(new MethodInvoker(() => 
            //{
            var gl = openGLControl.OpenGL;
            //IntPtr hrc = Win32.wglCreateContext(openGLControl.Handle);
            //Win32.wglMakeCurrent(openGLControl.Handle, hrc);
            //Task.Factory.StartNew(() => 
            //{
                
                
                //gl.MakeCurrent();
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

                gl.LoadIdentity();
            //gl.PushMatrix();
            gl.Begin(OpenGL.GL_TRIANGLES);
            gl.Color(1.0f, 0, 0);
            gl.Vertex(-1.0f, -1, z);
            gl.Color(0, 1.0f, 0);
            gl.Vertex(1.0f, -1, z);
            gl.Color(0, 0, 1.0f);
            gl.Vertex(0, 1.0f, z);
            gl.End();

            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Begin(OpenGL.GL_POINTS);
            for (var i = 0; i < vertices.Length / 3; i++)
            {
                //gl.Color(colors[3 * i], colors[3 * i + 1], colors[3 * i + 2]);
                gl.Vertex(vertices[3 * i], vertices[3 * i + 1], vertices[3 * i + 2]);
            }
            gl.End();
            //});

            //if (z > -10)
            //    z -= 0.2f;
            //}));


        }

        /// <summary>
        /// sharpgl初始化完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = openGLControl.OpenGL;
            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            var gl = openGLControl.OpenGL;
            var iParam = new int[1];
            gl.GetInteger(OpenGL.GL_RED_BITS, iParam);
            txtRedBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_GREEN_BITS, iParam);
            txtGreenBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_BLUE_BITS, iParam);
            txtBlueBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_ALPHA_BITS, iParam);
            txtAlphaBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_INDEX_BITS, iParam);
            txtIndexBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_DEPTH_BITS, iParam);
            txtDepthBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_STENCIL_BITS, iParam);
            txtStencilBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_ACCUM_RED_BITS, iParam);
            txtAccumRedBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_ACCUM_GREEN_BITS, iParam);
            txtAccumGreenBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_ACCUM_BLUE_BITS, iParam);
            txtAccumBlueBits.Text = iParam[0].ToString();
            gl.GetInteger(OpenGL.GL_ACCUM_ALPHA_BITS, iParam);
            txtAccumAlphaBits.Text = iParam[0].ToString();
            //var bParam = new bool[1];
            //gl.GetBoolean(OpenGL.GL_STEREO, 0, bParam);
            //chkStereo.Checked = bParam[0];
            //gl.GetBoolean(OpenGL.GL_DOUBLEBUFFER, 0, bParam);
            //chkDoubleBuffer.Checked = bParam[0];
            //InitThreads();
        }
        #endregion
    }
}
