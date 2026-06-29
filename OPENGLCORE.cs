using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyDraw
{
    public class OPENGLCORE
    {
        // Windows API constants
        private const int PFD_DRAW_TO_WINDOW = 0x00000004;
        private const int PFD_SUPPORT_OPENGL = 0x00000020;
        private const int PFD_DOUBLEBUFFER = 0x00000001;

        private const int PFD_TYPE_RGBA = 0;
        private const int PFD_MAIN_PLANE = 0;

        // Windows API structures
        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public ushort nSize;
            public ushort nVersion;
            public uint dwFlags;
            public byte iPixelType;
            public byte cColorBits;
            public byte cRedBits;
            public byte cRedShift;
            public byte cGreenBits;
            public byte cGreenShift;
            public byte cBlueBits;
            public byte cBlueShift;
            public byte cAlphaBits;
            public byte cAlphaShift;
            public byte cAccumBits;
            public byte cAccumRedBits;
            public byte cAccumGreenBits;
            public byte cAccumBlueBits;
            public byte cAccumAlphaBits;
            public byte cDepthBits;
            public byte cStencilBits;
            public byte cAuxBuffers;
            public byte iLayerType;
            public byte bReserved;
            public uint dwLayerMask;
            public uint dwVisibleMask;
            public uint dwDamageMask;
        }

        private const string GLU_DLL = "glu32.dll";

        // Windows API functions
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll")]
        public static extern bool SetPixelFormat(IntPtr hdc, int format, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll")]
        public static extern bool SwapBuffers(IntPtr hdc);

        [DllImport("opengl32.dll")]
        public static extern IntPtr wglCreateContext(IntPtr hdc);

        [DllImport("opengl32.dll")]
        public static extern bool wglMakeCurrent(IntPtr hdc, IntPtr hglrc);

        [DllImport("opengl32.dll")]
        public static extern void glClear(uint mask);

        [DllImport("opengl32.dll")]
        public static extern void glClearColor(float red, float green, float blue, float alpha);

        [DllImport("opengl32.dll")]
        public static extern void glBegin(uint mode);

        [DllImport("opengl32.dll")]
        public static extern void glEnd();

        [DllImport("opengl32.dll")]
        public static extern void glVertex2f(float x, float y);

        [DllImport("opengl32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void glVertex2d(double x, double y);
        [DllImport("opengl32.dll")]
        public static extern void glColor3f(float red, float green, float blue);

        [DllImport("opengl32.dll")]
        public static extern void glPointSize(float size);

        [DllImport("opengl32.dll")]
        public static extern void glMatrixMode(uint mode);

        [DllImport("opengl32.dll")]
        public static extern void glLoadIdentity();

        [DllImport("opengl32.dll")]
        public static extern void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);

        [DllImport("opengl32.dll")]
        public static extern void glViewport(int x, int y, int width, int height);
        [DllImport("opengl32.dll")]
        public static extern void glPushMatrix();

        [DllImport("opengl32.dll")]
        public static extern void glPopMatrix();

        [DllImport("opengl32.dll")]
        public static extern void glTranslatef(float x, float y, float z);

        [DllImport("opengl32.dll")]
        public static extern void glScalef(float x, float y, float z);

        [DllImport("opengl32.dll")]
        public static extern void glGenTextures(int n, out int textures);
        [DllImport("opengl32.dll")]
        public static extern void glBindTexture(uint target, int texture);
        [DllImport("opengl32.dll")]
        public static extern void glTexImage2D(
    uint target,
    int level,
    int internalformat,
    int width,
    int height,
    int border,
    uint format,
    uint type,
    IntPtr pixels);
        [DllImport("opengl32.dll")]
        public static extern void glTexParameteri(
    uint target,
    uint pname,
    int param);

        [DllImport("opengl32.dll")]
        public static extern void glTexCoord2f(float s, float t);
        [DllImport("opengl32.dll")]
        public static extern void glBlendFunc(uint sfactor, uint dfactor);

        // GLU callbacks
        public delegate void BeginCallback(uint type);
        public delegate void VertexCallback(IntPtr vertexData);
        public delegate void EndCallback();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ErrorCallback(int errorCode);
        public delegate void CombineCallback(
            [In] double[] coords,                // double[3] — new vertex coordinates
            [In] IntPtr[] vertexData,           // void*[4] — original vertices involved
            [In] float[] weights,               // float[4] — weights for interpolation
            out IntPtr outData                  // void** — pointer to your new vertex
        );
        [DllImport("opengl32.dll")]
        public static extern void glEnable(uint cap);
        [DllImport("opengl32.dll")]
        public static extern void glDisable(uint cap);
        // Import GLU functions
        [DllImport(GLU_DLL)] public static extern IntPtr gluNewTess();
        [DllImport(GLU_DLL)] public static extern void gluDeleteTess(IntPtr tess);
        [DllImport(GLU_DLL)] public static extern void gluTessBeginPolygon(IntPtr tess, IntPtr polygonData);
        [DllImport(GLU_DLL)] public static extern void gluTessEndPolygon(IntPtr tess);
        [DllImport(GLU_DLL)] public static extern void gluTessBeginContour(IntPtr tess);
        [DllImport(GLU_DLL)] public static extern void gluTessEndContour(IntPtr tess);
        [DllImport(GLU_DLL)] public static extern void gluTessVertex(IntPtr tess, double[] coords, IntPtr vertexData);

        [DllImport(GLU_DLL)] public static extern void gluTessProperty(IntPtr tess, int which, double value);

        [DllImport(GLU_DLL)] public static extern void gluTessCallback(IntPtr tess, int which, Delegate fn);


        public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        public const uint GL_POINTS = 0x0000; // decimal: 0
        public const uint GL_LINES = 0x0001;        // Each pair of vertices is an independent line
        public const uint GL_LINE_STRIP = 0x0003;   // Connected sequence of lines (polyline)
        public const uint GL_LINE_LOOP = 0x0002;    // Same as LINE_STRIP, but closes the loop
        public const uint GL_TRIANGLES = 0x0004;
        public const uint GL_TRIANGLE_FAN = 0x0006;
        public const uint GL_PROJECTION = 0x1701;
        public const uint GL_MODELVIEW = 0x1700;

        public const uint GL_TEXTURE_2D = 0x0DE1;
        public const uint GL_BLEND = 0x0BE2;
        public const uint GL_RGB = 0x1907;
        public const uint GL_RGBA = 0x1908;

        public const uint GL_BGR = 0x80E0;
        public const uint GL_BGRA = 0x80E1;

        public const uint GL_UNSIGNED_BYTE = 0x1401;

        public const uint GL_TEXTURE_MIN_FILTER = 0x2801;
        public const uint GL_TEXTURE_MAG_FILTER = 0x2800;

        public const uint GL_QUADS = 0x0007;

        public const uint GL_NEAREST = 0x2600;
        public const int GL_LINEAR = 0x2601;

        public const uint GL_SRC_ALPHA = 0x0302;
        public const uint GL_ONE_MINUS_SRC_ALPHA = 0x0303;

        public const int GLU_TESS_BEGIN = 100100;
        public const int GLU_TESS_VERTEX = 100101;
        public const int GLU_TESS_END = 100102;
        public const int GLU_TESS_ERROR = 100103;
        public const int GLU_TESS_COMBINE = 100105;

        public const int GLU_TESS_WINDING_RULE = 100140;
        public const double GLU_TESS_WINDING_ODD = 100130;
        public const double GLU_TESS_WINDING_NONZERO = 100131;
        public const double GLU_TESS_WINDING_POSITIVE = 100132;
        public const double GLU_TESS_WINDING_NEGATIVE = 100133;
        public const double GLU_TESS_WINDING_ABS_GEQ_TWO = 100134;


        public IntPtr hDC;
        private IntPtr hGLRC;

        private void InitializeOpenGL(IntPtr Handle)
        {
            hDC = GetDC(Handle);

            PIXELFORMATDESCRIPTOR pfd = new PIXELFORMATDESCRIPTOR
            {
                nSize = (ushort)Marshal.SizeOf(typeof(PIXELFORMATDESCRIPTOR)),
                nVersion = 1,
                dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,
                iPixelType = PFD_TYPE_RGBA,
                cColorBits = 24,
                cDepthBits = 16,
                iLayerType = PFD_MAIN_PLANE
            };

            int pixelFormat = ChoosePixelFormat(hDC, ref pfd);
            SetPixelFormat(hDC, pixelFormat, ref pfd);

            hGLRC = wglCreateContext(hDC);
            wglMakeCurrent(hDC, hGLRC);
        }

        public OPENGLCORE(IntPtr Handle)
        {
            InitializeOpenGL(Handle);
        }

        public void SwapBuffers()
        {
            SwapBuffers(hDC);
        }
    }
}
