using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyDraw
{
    public class GLGRAPHICS
    {
        OPENGLCORE core;
        GLPointF[] unitCircle = null;
        static int bezierLen = 16;
        static float curveTension = 0.5f;
        static int curveStepsPerSegment = 12;

        GLPointF[] PrecomputeUnitCircle(int segments)
        {
            var pts = new GLPointF[segments];
            for (int i = 0; i < segments; i++)
            {
                float theta = 2.0f * (float)Math.PI * i / segments;
                pts[i] = new GLPointF((float)Math.Cos(theta), (float)Math.Sin(theta));
            }
            return pts;
        }
        public GLGRAPHICS(Control control)
        {
            core = new OPENGLCORE(control.Handle);

            OPENGLCORE.glViewport(0, 0, control.Width, control.Height);

            OPENGLCORE.glMatrixMode(OPENGLCORE.GL_PROJECTION);
            OPENGLCORE.glLoadIdentity();

            OPENGLCORE.glOrtho(0, control.Width, control.Height, 0, -1, 1);

            OPENGLCORE.glMatrixMode(OPENGLCORE.GL_MODELVIEW);
            OPENGLCORE.glLoadIdentity();

            unitCircle = PrecomputeUnitCircle(64);
        }
        public void FrameDone()
        {
            core.SwapBuffers();
        }
        public void SetSize(int w, int h)
        {
            OPENGLCORE.glViewport(0, 0, w, h);

            OPENGLCORE.glMatrixMode(OPENGLCORE.GL_PROJECTION);
            OPENGLCORE.glLoadIdentity();

            OPENGLCORE.glOrtho(0, w, h, 0, -1, 1);

            OPENGLCORE.glMatrixMode(OPENGLCORE.GL_MODELVIEW);
            OPENGLCORE.glLoadIdentity();
        }
        public class GLColor
        {
            public float R, G, B, A;
        }
        public class GLPointF
        {
            public float X, Y;
            public GLPointF(float _X, float _Y)
            {
                X = _X;
                Y = _Y;
            }
        }
        public class GLRectangle
        {
            public GLPointF Location, Size;

            public GLRectangle(GLPointF location, GLPointF size)
            {
                Location = location;
                Size = size;
            }
        }
        public void Clear(GLColor color)
        {
            OPENGLCORE.glClearColor(color.R, color.G, color.B, color.A);
            OPENGLCORE.glClear(OPENGLCORE.GL_COLOR_BUFFER_BIT);
        }
        public void DrawPoint(GLColor color, float x, float y, float r)
        {
            OPENGLCORE.glPointSize(r); // Size of each dot
            OPENGLCORE.glBegin(OPENGLCORE.GL_POINTS);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            OPENGLCORE.glVertex2f(x, y);
            OPENGLCORE.glEnd();
        }
        public void DrawPoints(GLColor color, float[] path, float r)
        {
            OPENGLCORE.glPointSize(r); // Size of each dot
            OPENGLCORE.glBegin(OPENGLCORE.GL_POINTS);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            for (int i = 0; i < path.Length; i += 2)
            {
                OPENGLCORE.glVertex2f(path[i], path[i + 1]);
            }
            OPENGLCORE.glEnd();
        }

        public void DrawLine(GLColor color, GLPointF point1, GLPointF point2)
        {
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINES);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            OPENGLCORE.glVertex2f(point1.X, point1.Y);
            OPENGLCORE.glVertex2f(point2.X, point2.Y);
            OPENGLCORE.glEnd();
        }
        public void DrawLines(GLColor color, GLPointF[] path)
        {
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_STRIP);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            foreach (GLPointF point in path)
            {
                OPENGLCORE.glVertex2f(point.X, point.Y);
            }
            OPENGLCORE.glEnd();
        }
        public void DrawPolygon(GLColor color, GLPointF[] path)
        {
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_LOOP);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            foreach (GLPointF point in path)
            {
                OPENGLCORE.glVertex2f(point.X, point.Y);
            }
            OPENGLCORE.glEnd();
        }

        public void DrawEllipse(GLColor color, GLRectangle rect)
        {
            OPENGLCORE.glPushMatrix();
            OPENGLCORE.glTranslatef(rect.Location.X, rect.Location.Y, 0);
            OPENGLCORE.glScalef(rect.Size.X * 0.5f, rect.Size.Y * 0.5f, 1);
            OPENGLCORE.glTranslatef(1, 1, 0);
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_LOOP);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            foreach (GLPointF pt in unitCircle)
            {
                OPENGLCORE.glVertex2f(pt.X, pt.Y);
            }
            OPENGLCORE.glEnd();
            OPENGLCORE.glPopMatrix();
        }
        public void DrawArc(GLColor color, GLRectangle rect, float startAngle, float sweepAngle)
        {
            OPENGLCORE.glPushMatrix();
            OPENGLCORE.glTranslatef(rect.Location.X, rect.Location.Y, 0);
            OPENGLCORE.glScalef(rect.Size.X * 0.5f, rect.Size.Y * 0.5f, 1);
            OPENGLCORE.glTranslatef(1, 1, 0);
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_STRIP);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            int i0 = (int)(unitCircle.Length * startAngle / 360.0);
            int i1 = (int)(unitCircle.Length * (startAngle + sweepAngle) / 360.0);
            for (int i = i0; i <= i1; i++)
            {
                if (i >= 0 && i < unitCircle.Length)
                {
                    GLPointF pt = unitCircle[i];
                    OPENGLCORE.glVertex2f(pt.X, pt.Y);
                }
            }
            OPENGLCORE.glEnd();
            OPENGLCORE.glPopMatrix();
        }


        public void DrawRectangle(GLColor color, GLRectangle rect)
        {
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_LOOP);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            OPENGLCORE.glVertex2f(rect.Location.X, rect.Location.Y);
            OPENGLCORE.glVertex2f(rect.Location.X + rect.Size.X, rect.Location.Y);
            OPENGLCORE.glVertex2f(rect.Location.X + rect.Size.X, rect.Location.Y + rect.Size.Y);
            OPENGLCORE.glVertex2f(rect.Location.X, rect.Location.Y + rect.Size.Y);
            OPENGLCORE.glEnd();
        }
        public void DrawRectangles(GLColor color, GLRectangle[] rects)
        {
            foreach (GLRectangle rect in rects)
            {
                DrawRectangle(color, rect);
            }
        }

        public void DrawBezier(GLColor color, GLPointF point1, GLPointF point2, GLPointF point3, GLPointF point4)
        {
            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_STRIP);

            // Move color setup outside the loop
            OPENGLCORE.glColor3f(color.R, color.G, color.B);

            float fbl = (float)bezierLen;
            float px1 = point1.X, py1 = point1.Y;
            float px2 = point2.X, py2 = point2.Y;
            float px3 = point3.X, py3 = point3.Y;
            float px4 = point4.X, py4 = point4.Y;

            for (int i = 0; i <= bezierLen; i++)
            {
                float t = i / fbl;
                float it = 1f - t;

                float it2 = it * it;
                float t2 = t * t;

                float b0 = it * it2;
                float b1 = 3f * it2 * t;
                float b2 = 3f * it * t2;
                float b3 = t * t2;

                float x = b0 * px1 + b1 * px2 + b2 * px3 + b3 * px4;
                float y = b0 * py1 + b1 * py2 + b2 * py3 + b3 * py4;

                OPENGLCORE.glVertex2f(x, y);
            }

            OPENGLCORE.glEnd();
        }

        public void DrawBeziers(GLColor color, GLPointF[] points)
        {
            if (points.Length > 3)
            {
                for (int i = 0; i < points.Length - 3; i += 3)
                {
                    DrawBezier(color, points[i], points[i + 1], points[i + 2], points[i + 3]);
                }
            }
        }


        public void DrawClosedCurve(GLColor color, GLPointF[] path)
        {
            int count = path.Length;
            if (count < 4)
            {
                return; // Need at least 4 points
            }
            else
            {
                OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_STRIP);
                OPENGLCORE.glColor3f(color.R, color.G, color.B);
                float cspF = (float)curveStepsPerSegment;

                for (int i = 0; i < count; i++)
                {
                    GLPointF p0 = path[(i - 1 + count) % count];
                    GLPointF p1 = path[i];
                    GLPointF p2 = path[(i + 1) % count];
                    GLPointF p3 = path[(i + 2) % count];
                    float m0x = curveTension * (p2.X - p0.X);
                    float m0y = curveTension * (p2.Y - p0.Y);
                    float m1x = curveTension * (p3.X - p1.X);
                    float m1y = curveTension * (p3.Y - p1.Y);

                    for (int j = 0; j <= curveStepsPerSegment; j++)
                    {
                        float t = j / cspF;
                        float t2 = t * t;
                        float t3 = t2 * t;

                        float a0 = 2 * t3 - 3 * t2 + 1;
                        float a1 = t3 - 2 * t2 + t;
                        float a2 = t3 - t2;
                        float a3 = -2 * t3 + 3 * t2;

                        float x = a0 * p1.X + a1 * m0x + a2 * m1x + a3 * p2.X;
                        float y = a0 * p1.Y + a1 * m0y + a2 * m1y + a3 * p2.Y;

                        OPENGLCORE.glVertex2f(x, y);
                    }
                }

                OPENGLCORE.glEnd();
            }
        }
        public void DrawCurve(GLColor color, GLPointF[] path)
        {
            int count = path.Length;
            if (count < 4) return;

            float tension = curveTension;
            int steps = curveStepsPerSegment;

            OPENGLCORE.glBegin(OPENGLCORE.GL_LINE_STRIP);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);

            for (int i = 0; i < count - 1; i++)
            {
                // Prevent repeated modulus and conditional logic per point
                int i0 = i == 0 ? 0 : i - 1;
                int i1 = i;
                int i2 = i + 1;
                int i3 = (i + 2 < count) ? i + 2 : i + 1; // Clamp to last point

                GLPointF p0 = path[i0];
                GLPointF p1 = path[i1];
                GLPointF p2 = path[i2];
                GLPointF p3 = path[i3];

                float dx1 = tension * (p2.X - p0.X);
                float dy1 = tension * (p2.Y - p0.Y);
                float dx2 = tension * (p3.X - p1.X);
                float dy2 = tension * (p3.Y - p1.Y);

                for (int j = 0; j <= steps; j++)
                {
                    float t = j / (float)steps;
                    float t2 = t * t;
                    float t3 = t2 * t;

                    float a0 = 2f * t3 - 3f * t2 + 1f;
                    float a1 = t3 - 2f * t2 + t;
                    float a2 = t3 - t2;
                    float a3 = -2f * t3 + 3f * t2;

                    float x = a0 * p1.X + a1 * dx1 + a2 * dx2 + a3 * p2.X;
                    float y = a0 * p1.Y + a1 * dy1 + a2 * dy2 + a3 * p2.Y;

                    OPENGLCORE.glVertex2f(x, y);
                }
            }

            OPENGLCORE.glEnd();
        }

        public void FillEllipse(GLColor color, GLRectangle rect)
        {
            OPENGLCORE.glPushMatrix();
            OPENGLCORE.glTranslatef(rect.Location.X, rect.Location.Y, 0);
            OPENGLCORE.glScalef(rect.Size.X * 0.5f, rect.Size.Y * 0.5f, 1);
            OPENGLCORE.glTranslatef(1, 1, 0);
            OPENGLCORE.glBegin(OPENGLCORE.GL_TRIANGLE_FAN);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            OPENGLCORE.glVertex2f(0, 0);
            GLPointF ft = unitCircle[0];
            foreach (GLPointF pt in unitCircle)
            {
                OPENGLCORE.glVertex2f(pt.X, pt.Y);
            }
            OPENGLCORE.glVertex2f(ft.X, ft.Y);
            OPENGLCORE.glEnd();
            OPENGLCORE.glPopMatrix();
        }

        public void FillPie(GLColor color, GLRectangle rect, float startAngle, float sweepAngle)
        {
            OPENGLCORE.glPushMatrix();
            OPENGLCORE.glTranslatef(rect.Location.X, rect.Location.Y, 0);
            OPENGLCORE.glScalef(rect.Size.X * 0.5f, rect.Size.Y * 0.5f, 1);
            OPENGLCORE.glTranslatef(1, 1, 0);
            OPENGLCORE.glBegin(OPENGLCORE.GL_TRIANGLE_FAN);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            int i0 = (int)(unitCircle.Length * startAngle / 360.0);
            int i1 = (int)(unitCircle.Length * (startAngle + sweepAngle) / 360.0);

            OPENGLCORE.glVertex2f(0, 0);
            for (int i = i0; i <= i1; i++)
            {
                if (i >= 0 && i < unitCircle.Length)
                {
                    GLPointF pt = unitCircle[i];
                    OPENGLCORE.glVertex2f(pt.X, pt.Y);
                }
            }
            OPENGLCORE.glEnd();
            OPENGLCORE.glPopMatrix();
        }
        public void FillPolygon(GLColor color, GLPointF[] points)
        {
            if (points == null || points.Length < 3)
                return;

            // Convert to double[3] list (z = 0)
            List<double[]> vertices = new List<double[]>();
            foreach (GLPointF p in points)
            {
                vertices.Add(new double[] { p.X, p.Y, 0 });
            }

            IntPtr tess = OPENGLCORE.gluNewTess();


            OPENGLCORE.BeginCallback beginCallback = (mode) => OPENGLCORE.glBegin(mode);
            OPENGLCORE.VertexCallback vertexCallback = (vertexData) =>
            {
                // vertexData is pointer to double[3]
                unsafe
                {
                    double* coords = (double*)vertexData;
                    OPENGLCORE.glVertex2d(coords[0], coords[1]);
                }
            };

            OPENGLCORE.CombineCallback combineCallback = (
                double[] coords,
                IntPtr[] vertexData,
                float[] weights,
                out IntPtr outData
            ) =>
            {
                int size = sizeof(double) * 3;
                IntPtr newVertex = Marshal.AllocHGlobal(size);

                unsafe
                {
                    double* ptr = (double*)newVertex.ToPointer();
                    ptr[0] = coords[0];
                    ptr[1] = coords[1];
                    ptr[2] = coords[2];
                }

                outData = newVertex;
            };

            OPENGLCORE.EndCallback endCallback = () => OPENGLCORE.glEnd();


            // Set callbacks
            OPENGLCORE.gluTessCallback(tess, OPENGLCORE.GLU_TESS_BEGIN, new OPENGLCORE.BeginCallback(type => OPENGLCORE.glBegin(type)));
            OPENGLCORE.gluTessCallback(tess, OPENGLCORE.GLU_TESS_VERTEX, new OPENGLCORE.VertexCallback(ptr =>
            {
                double[] coords = (double[])GCHandle.FromIntPtr(ptr).Target;
                OPENGLCORE.glVertex2f((float)coords[0], (float)coords[1]);
            }));

            OPENGLCORE.gluTessCallback(tess, OPENGLCORE.GLU_TESS_END, new OPENGLCORE.EndCallback(() => OPENGLCORE.glEnd()));

            OPENGLCORE.glColor3f(color.R, color.G, color.B);

            OPENGLCORE.gluTessProperty(tess, OPENGLCORE.GLU_TESS_WINDING_RULE, (double)OPENGLCORE.GLU_TESS_WINDING_NONZERO);

            OPENGLCORE.gluTessBeginPolygon(tess, IntPtr.Zero);
            OPENGLCORE.gluTessBeginContour(tess);

            // Send vertices to tessellator
            foreach (double[] vertex in vertices)
            {
                GCHandle handle = GCHandle.Alloc(vertex);
                OPENGLCORE.gluTessVertex(tess, vertex, GCHandle.ToIntPtr(handle));
            }

            OPENGLCORE.gluTessEndContour(tess);
            OPENGLCORE.gluTessEndPolygon(tess);

            OPENGLCORE.gluDeleteTess(tess);
        }
        public void FillPolygon0(GLColor color, GLPointF[] loopedPoints)
        {
            if (loopedPoints == null || loopedPoints.Length < 3)
            {
                return;
            }
            else
            {
                // Compute centroid of polygon
                float cx = 0, cy = 0;
                int count = loopedPoints.Length;
                for (int i = 0; i < count; i++)
                {
                    cx += loopedPoints[i].X;
                    cy += loopedPoints[i].Y;
                }
                cx /= count;
                cy /= count;

                OPENGLCORE.glBegin(OPENGLCORE.GL_TRIANGLE_FAN);
                OPENGLCORE.glColor3f(color.R, color.G, color.B);

                // Add center point
                OPENGLCORE.glVertex2f(cx, cy);

                // Add outer loop points
                for (int i = 0; i < count; i++)
                {
                    OPENGLCORE.glVertex2f(loopedPoints[i].X, loopedPoints[i].Y);
                }

                // Close the loop by repeating the first point
                OPENGLCORE.glVertex2f(loopedPoints[0].X, loopedPoints[0].Y);

                OPENGLCORE.glEnd();
            }
        }

        public void FillRectangle(GLColor color, GLRectangle rect)
        {
            OPENGLCORE.glBegin(OPENGLCORE.GL_TRIANGLES);
            OPENGLCORE.glColor3f(color.R, color.G, color.B);
            OPENGLCORE.glVertex2f(rect.Location.X, rect.Location.Y);
            OPENGLCORE.glVertex2f(rect.Location.X + rect.Size.X, rect.Location.Y);
            OPENGLCORE.glVertex2f(rect.Location.X + rect.Size.X, rect.Location.Y + rect.Size.Y);

            OPENGLCORE.glVertex2f(rect.Location.X + rect.Size.X, rect.Location.Y + rect.Size.Y);
            OPENGLCORE.glVertex2f(rect.Location.X, rect.Location.Y + rect.Size.Y);
            OPENGLCORE.glVertex2f(rect.Location.X, rect.Location.Y);
            OPENGLCORE.glEnd();
        }
        public void FillRectangles(GLColor color, GLRectangle[] rects)
        {
            foreach (GLRectangle rect in rects)
            {
                FillRectangle(color, rect);
            }
        }


        public void DrawText(FontAtlas atlas, GLColor Color, string text, float x, float y)
        {
            OPENGLCORE.glEnable(OPENGLCORE.GL_TEXTURE_2D);
            OPENGLCORE.glEnable(OPENGLCORE.GL_BLEND);
            OPENGLCORE.glBlendFunc(OPENGLCORE.GL_SRC_ALPHA, OPENGLCORE.GL_ONE_MINUS_SRC_ALPHA);

            OPENGLCORE.glBindTexture(OPENGLCORE.GL_TEXTURE_2D, atlas.textureId);
            OPENGLCORE.glColor3f(Color.R, Color.G, Color.B);

            OPENGLCORE.glBegin(OPENGLCORE.GL_QUADS);

            foreach (char c in text)
            {
                if (!atlas.glyphs.TryGetValue(c, out var g))
                    continue;

                float x2 = x + g.width;
                float y2 = y + g.height;

                OPENGLCORE.glTexCoord2f(g.u1, g.v1); OPENGLCORE.glVertex2f(x, y);
                OPENGLCORE.glTexCoord2f(g.u2, g.v1); OPENGLCORE.glVertex2f(x2, y);
                OPENGLCORE.glTexCoord2f(g.u2, g.v2); OPENGLCORE.glVertex2f(x2, y2);
                OPENGLCORE.glTexCoord2f(g.u1, g.v2); OPENGLCORE.glVertex2f(x, y2);

                x += g.advance;
            }

            OPENGLCORE.glEnd();

            OPENGLCORE.glBindTexture(OPENGLCORE.GL_TEXTURE_2D, 0);
            OPENGLCORE.glDisable(OPENGLCORE.GL_TEXTURE_2D);
            OPENGLCORE.glDisable(OPENGLCORE.GL_BLEND);
        }



        /*            OPENGLCORE.glBegin(OPENGLCORE.GL_TRIANGLES);
            OPENGLCORE.glColor3f(1.0f, 0.0f, 0.0f);
            OPENGLCORE.glVertex2f(50f, 50f);
            OPENGLCORE.glColor3f(0.0f, 1.0f, 0.0f);
            OPENGLCORE.glVertex2f(100f, 50f);
            OPENGLCORE.glColor3f(0.0f, 0.0f, 1.0f);
            OPENGLCORE.glVertex2f(75.0f, 100f);
            OPENGLCORE.glEnd();

*/
    }
}
