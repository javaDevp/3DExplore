using SharpGL;
using SharpGL.VertexBuffers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerformanceDemo
{
    public partial class Form2 : Form
    {
        /// <summary>
        /// 渲染上下文句柄
        /// </summary>
        private IntPtr hrc;

        /// <summary>
        /// 上下文句柄
        /// </summary>
        private IntPtr hdc;

        /// <summary>
        /// 三百万个点数据
        /// </summary>
        private float[] vertices = new float[3 * 3 * 100 * 100 * 100];

        private float[] colors = new float[3 * 3 * 100 * 100 * 100];

        private Stopwatch stopwatch = new Stopwatch();

        private object lockObj = new object();

        private bool isSizeChanged = false;

        private OpenGL gl = new OpenGL();

        public Form2()
        {
            InitializeComponent();
            GenerateRandomVertices();
            InitRenderContext(this.Handle);
            InitOpengl();
            InitVbo();
            //开始绘制
            var timer = new System.Timers.Timer();
            timer.Interval = 50;
            timer.Elapsed += (tsender, te) =>
            {
                Render();
            };
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void InitVbo()
        {
           
            
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            isSizeChanged = true;
            lock (lockObj)
            {
                Win32.wglMakeCurrent(hdc, hrc);
                MyOpenGL.glViewport(0, 0, Width, Height);
                MyOpenGL.glMatrixMode(MyOpenGL.GL_PROJECTION);
                MyOpenGL.glLoadIdentity();
                MyOpenGL.gluPerspective(45.0, Width / Height, 0.01, 1000);
                MyOpenGL.glMatrixMode(MyOpenGL.GL_MODELVIEW);
                MyOpenGL.glLoadIdentity();
                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            }
            isSizeChanged = false;
        }

        /// <summary>
        /// 随机生成顶点数据
        /// </summary>
        private void GenerateRandomVertices()
        {
            Random rand = new Random();
            for (var i = 0; i < vertices.Length / 3; i++)
            {
                vertices[3 * i] = (float)rand.NextDouble();
                vertices[3 * i + 1] = (float)rand.NextDouble();
                vertices[3 * i + 2] = -5;
            }
        }

        private void InitOpengl()
        {
            //Win32.wglMakeCurrent(hdc, hrc);

            //MyOpenGL.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            //Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);

        }

        private void InitRenderContext(IntPtr hwnd)
        {
            hdc = Win32.GetDC(hwnd);

            //	Setup a pixel format.
            Win32.PIXELFORMATDESCRIPTOR pfd = new Win32.PIXELFORMATDESCRIPTOR();
            pfd.Init();
            pfd.nVersion = 1;
            pfd.dwFlags = Win32.PFD_DRAW_TO_WINDOW | Win32.PFD_SUPPORT_OPENGL | Win32.PFD_DOUBLEBUFFER;
            pfd.iPixelType = Win32.PFD_TYPE_RGBA;
            pfd.cColorBits = (byte)16;
            pfd.cDepthBits = 16;
            pfd.cStencilBits = 8;
            pfd.iLayerType = Win32.PFD_MAIN_PLANE;

            //	Match an appropriate pixel format 
            int iPixelformat;
            if ((iPixelformat = Win32.ChoosePixelFormat(hdc, pfd)) == 0)
            {
                MessageBox.Show("ChoosePixelFormat");
                return;
            }

            //	Sets the pixel format
            if (Win32.SetPixelFormat(hdc, iPixelformat, pfd) == 0)
            {
                MessageBox.Show("SetPixelFormat");
                return;
            }

            hrc = Win32.wglCreateContext(hdc);
            if (hrc == IntPtr.Zero)
            {
                MessageBox.Show("创建渲染上下文失败");
                return;
            }
        }

        private void Render()
        {
            if (isSizeChanged)
                return;
            lock (lockObj)
            {
                stopwatch.Restart();
                Win32.wglMakeCurrent(hdc, hrc);
                draw();
                Win32.SwapBuffers(hdc);
                Win32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        private void draw()
        {
            MyOpenGL.glClear(MyOpenGL.GL_COLOR_BUFFER_BIT | MyOpenGL.GL_DEPTH_BUFFER_BIT);
            MyOpenGL.glLoadIdentity();
            MyOpenGL.glBegin(MyOpenGL.GL_TRIANGLES);
            MyOpenGL.glColor3f(1.0f, 0, 0);
            MyOpenGL.glVertex3f(-1.0f, -1, -5);
            MyOpenGL.glColor3f(0, 1.0f, 0);
            MyOpenGL.glVertex3f(1.0f, -1, -5);
            MyOpenGL.glColor3f(0, 0, 1.0f);
            MyOpenGL.glVertex3f(0, 1.0f, -5);
            MyOpenGL.glEnd();

            MyOpenGL.glBegin(MyOpenGL.GL_POINTS);
            for (var i = 0; i < vertices.Length / 3; i++)
            {
                MyOpenGL.glVertex3f(vertices[3 * i], vertices[3 * i + 1], vertices[3 * i + 2]);
            }
            MyOpenGL.glEnd();
            MyOpenGL.glFlush();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
            base.WndProc(ref m);
        }
    }
}
