using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace EasyDraw
{
    public class GLFONT
    {
        public GLFONT(string name, float h)
        {
            Build(name, h);
        }
        int UploadToOpenGL(Bitmap bmp)
        {
            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int tex;
            OPENGLCORE.glGenTextures(1, out tex);
            OPENGLCORE.glBindTexture(OPENGLCORE.GL_TEXTURE_2D, tex);

            OPENGLCORE.glTexImage2D(
                OPENGLCORE.GL_TEXTURE_2D,
                0,
                (int)OPENGLCORE.GL_RGBA,
                bmp.Width,
                bmp.Height,
                0,
                OPENGLCORE.GL_BGRA,
                OPENGLCORE.GL_UNSIGNED_BYTE,
                data.Scan0);

            OPENGLCORE.glTexParameteri(OPENGLCORE.GL_TEXTURE_2D,
                OPENGLCORE.GL_TEXTURE_MIN_FILTER,
                //(int)OPENGLCORE.GL_NEAREST
                OPENGLCORE.GL_LINEAR
                 );

            OPENGLCORE.glTexParameteri(OPENGLCORE.GL_TEXTURE_2D,
                OPENGLCORE.GL_TEXTURE_MAG_FILTER,
                //(int)OPENGLCORE.GL_NEAREST
                OPENGLCORE.GL_LINEAR
                 );

            bmp.UnlockBits(data);

            return tex;
        }
        public class Glyph
        {
            public float u1, v1, u2, v2;
            public int width;
            public int height;
            public int advance;
        }

        public Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();

        public int textureId;
        public int atlasWidth;
        public int atlasHeight = 512;
        public Bitmap bmp;

        public float fontHeight;

        public void Build(string fontName, float _fontSize)
        {
            fontHeight = _fontSize;
            Font font = new Font(fontName, _fontSize, GraphicsUnit.Pixel);

            bmp = new Bitmap(atlasHeight, atlasHeight, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Transparent);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            int x = 0;
            int y = 0;
            int rowHeight = 0;

            for (int c = 32; c < 127; c++)
            {
                char ch = (char)c;

                string s = ch.ToString();
                SizeF size = g.MeasureString(s, font);

                if (x + size.Width >= atlasHeight)
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }

                g.DrawString(s, font, Brushes.White, x, y);

                Glyph glyph = new Glyph
                {
                    width = (int)size.Width,
                    height = (int)size.Height,
                    advance = (int)size.Width,
                };

                glyph.u1 = x / (float)atlasHeight;
                glyph.v1 = y / (float)atlasHeight;
                glyph.u2 = (x + size.Width) / (float)atlasHeight;
                glyph.v2 = (y + size.Height) / (float)atlasHeight;

                glyphs[ch] = glyph;

                x += (int)size.Width;
                rowHeight = Math.Max(rowHeight, (int)size.Height);
            }
            textureId = UploadToOpenGL(bmp);
        }
    }
}
