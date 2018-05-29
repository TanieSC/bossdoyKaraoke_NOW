using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Models;
using bossdoyKaraoke_NOW.Nlog;
using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using D2D1 = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;

namespace bossdoyKaraoke_NOW.CDG
{
    public class GraphicUtilDX
    {
        private static readonly DXGI.Format Format = DXGI.Format.B8G8R8A8_UNorm;
        public static readonly D2D1.PixelFormat D2PixelFormat = new D2D1.PixelFormat(Format, D2D1.AlphaMode.Premultiplied);
        private static D2D1.BitmapProperties1 BitmapProps1 = new D2D1.BitmapProperties1(D2PixelFormat, 96, 96, D2D1.BitmapOptions.Target);

        private Control m_RenderingSurface;
        private Control m_RenderingSurface2;
        private D2D1.Bitmap m_VideoBitmap = null;
        private D2D1.Bitmap m_CDGBitmap = null;
        private D2D1.Bitmap m_TitleName = null;
        private D2D1.Bitmap1 m_Bitmap1;
        private D2D1.Bitmap1 m_Bitmap1FullScreen;

        private D2D1.Effects.Shadow m_Shadow;
        private D2D1.Effects.Shadow m_ShadowFullScreen;
        private D2D1.Effects.AffineTransform2D m_affineTransformEffect;
        private D2D1.Effects.Composite m_compositeEffect;
        private DXD2D1 m_D2DContext;
        private DataStream dataStream;

        private RawRectangleF m_videoBitmapRectangle;
        private RawRectangleF m_destCdgRectangle;
        private RawRectangleF m_destCdgRectangle2;
        private D2D1.RoundedRectangle m_roundedRecNextSong;
        private D2D1.RoundedRectangle m_roundedRec;
        private D2D1.SolidColorBrush roundedRecOutColor;
        private D2D1.SolidColorBrush roundedRecInColor;

        private System.Drawing.FontFamily fontFamily;
        private TextFormat m_textFormat15;
        private TextFormat m_textFormat10;
        private D2D1.SolidColorBrush m_textBrush;

        float fontSize10 = 10f;
        float fontSize15 = 15f;
        float fontSize30 = 30f;
        private int SurfaceW;
        private int SurfaceH;
        private int SurfaceT;
        private int SurfaceB;
        private int SurfaceL;
        private int SurfaceR;
        private int SurfaceW2;
        private int SurfaceH2;
        private int SurfaceT2;
        private int SurfaceB2;
        private int SurfaceL2;
        private int SurfaceR2;


        public void Initialize(Control surface, Control surfaceFullScreen)
        {
            try
            {
                // Create the rendering surface from the control.
                m_RenderingSurface = new Control();
                m_RenderingSurface2 = new Control();
                m_RenderingSurface = surface;
                m_RenderingSurface2 = surfaceFullScreen;

                // Initialize the Direct2D rendering context.
                DeviceSettings settings = new DeviceSettings()
                {
                    Width = surface.ClientSize.Width,
                    Height = surface.ClientSize.Height
                };

                DeviceSettings settingsFullScreen = new DeviceSettings()
                {
                    Width = surfaceFullScreen.ClientSize.Width,
                    Height = surfaceFullScreen.ClientSize.Height
                };

                m_D2DContext = new DXD2D1();

                m_D2DContext.InitScreen1(surface.Handle);
                m_D2DContext.InitScreen2(surfaceFullScreen.Handle, settingsFullScreen);

                InitSurfaceSize();
                LoadResources();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "Initialize", ex.LineNumber(), "GraphicUtilDX");

            }
        }


        public void InitScreen2(Control surfaceFullScreen) {
            try
            {
                m_RenderingSurface2 = new Control();
                m_RenderingSurface2 = surfaceFullScreen;
                DeviceSettings settingsFullScreen = new DeviceSettings()
                {
                    Width = m_RenderingSurface2.ClientSize.Width,
                    Height = m_RenderingSurface2.ClientSize.Height
                };

                m_D2DContext.InitScreen2(m_RenderingSurface2.Handle, settingsFullScreen);
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "InitScreen2", ex.LineNumber(), "GraphicUtilDX");

            }
        }

        public static unsafe System.Drawing.Bitmap StreamToBitmap(ref Stream stream, int width, int height)
        {
            //create a new bitmap
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            try
            {
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                stream.Seek(0, SeekOrigin.Begin);
                //copy the stream of pixel

                byte[] myBytes = new byte[stream.Length];
                stream.Read(myBytes, 0, Convert.ToInt32(stream.Length));

                byte* p = (byte*)bmpData.Scan0.ToPointer();
                for (int n = 0; n < stream.Length; n++)
                {
                    p[n] = myBytes[n];
                }

                bmp.UnlockBits(bmpData);
                bmp.MakeTransparent(bmp.GetPixel(1, 1));
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "StreamToBitmap", ex.LineNumber(), "GraphicUtilDX");

            }
            return bmp;
        }

        public D2D1.Bitmap ConvertToSlimDXBitmap(D2D1.DeviceContext context, System.Drawing.Bitmap bmp)
        {

            D2D1.Bitmap Image = null;
            try
            {
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                dataStream = new DataStream(bmpData.Scan0, bmpData.Stride * bmpData.Height, true, false);
                D2D1.BitmapProperties d2dBitmapProperties = new D2D1.BitmapProperties();
                d2dBitmapProperties.PixelFormat = D2PixelFormat;

                Image = new D2D1.Bitmap(context, new Size2(bmpData.Width, bmpData.Height), dataStream, bmpData.Stride, d2dBitmapProperties);
                dataStream.Dispose();
                bmp.UnlockBits(bmpData);
                bmp.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "ConvertToSlimDXBitmap", ex.LineNumber(), "GraphicUtilDX");

            }
            return Image;

        }

        public void DoRender(System.Drawing.Bitmap pic1, System.Drawing.Bitmap pic2, string addSong, bool IsPlaying)
        {
            try
            {    
                InitSurfaceSize();

                if (pic1 != null)
                {
                    m_VideoBitmap = ConvertToSlimDXBitmap(m_D2DContext.d2dContext, pic1);
                    pic1.Dispose();

                    int VideoW = (int)m_VideoBitmap.Size.Width;
                    int VideoH = (int)m_VideoBitmap.Size.Height;

                    if (SurfaceH2 > 0)
                    {
                        fontSize10 = (SurfaceH2) / 40;
                        fontSize15 = (SurfaceH2) / 20;
                        fontSize30 = (SurfaceH2) / 10;
                    }
                    else
                    { //just guesing the font size
                        fontSize10 = 10f;
                        fontSize15 = 15f;
                        fontSize30 = 30f;
                    }

                    LoadResources();

                    m_D2DContext.d2dContextCdgText.Target = m_Bitmap1; // associate bitmap with the d2d context
                    m_D2DContext.d2dContextCdgText.BeginDraw();
                    m_D2DContext.d2dContextCdgText.Clear(Color.Transparent);

                    m_D2DContext.d2dContextCdgText2.Target = m_Bitmap1FullScreen; // associate bitmap with the d2d context
                    m_D2DContext.d2dContextCdgText2.BeginDraw();
                    m_D2DContext.d2dContextCdgText2.Clear(Color.Transparent);

                    m_Shadow.SetInput(0, m_Bitmap1, true);

                    // Create image shadow effect
                    m_ShadowFullScreen.SetInput(0, m_Bitmap1FullScreen, true);
                    m_ShadowFullScreen.Optimization = D2D1.ShadowOptimization.Quality;
                    
                    // Create image transform effect
                    m_affineTransformEffect.SetInputEffect(0, m_ShadowFullScreen);
                    m_affineTransformEffect.TransformMatrix = Matrix3x2.Translation(0,0);
                    
                    // Create composite effect
                   // m_compositeEffect.InputCount = 2;
                    m_compositeEffect.SetInputCount(2);
                    m_compositeEffect.SetInputEffect(0, m_ShadowFullScreen);
                    m_compositeEffect.SetInputEffect(1, m_affineTransformEffect);
                    m_compositeEffect.SetInput(2, m_Bitmap1FullScreen, true);


                    if (pic2 != null)
                    {
                        m_CDGBitmap = ConvertToSlimDXBitmap(m_D2DContext.d2dContextCdgText, pic2);
                        pic2.Dispose();

                        //float Height = (SurfaceH * 2) / 3f;
                        //float Width = (SurfaceW * 2) / 3f;
                        float Height2 = (SurfaceH2 * 2) / 3f;
                        float Width2 = (SurfaceW2 * 2) / 3f;

                        float CdgW = (m_CDGBitmap.Size.Width * 2) / 3f;
                        float CdgH = (m_CDGBitmap.Size.Height * 2) / 3f;

                        //  float CdgWFullScreen = (m_CDGBitmap.Size.Width + SurfaceW2) / 2f;
                        //  float CdgHFullScreen = (m_CDGBitmap.Size.Height + SurfaceH2) / 2f;
                        m_destCdgRectangle = new RawRectangleF((SurfaceR / 2) - (CdgW / 2), SurfaceH, (SurfaceR / 2) + (CdgW / 2), SurfaceB - CdgH);
                        m_destCdgRectangle2 = new RawRectangleF(((SurfaceR2 / 2) - (Width2 / 2)), SurfaceH2, ((SurfaceR2 / 2) + (Width2 / 2)), SurfaceB2 - Height2);
                        m_D2DContext.d2dContextCdgText.DrawBitmap(m_CDGBitmap, m_destCdgRectangle, 1.0f, D2D1.BitmapInterpolationMode.Linear);
                        m_D2DContext.d2dContextCdgText2.DrawBitmap(m_CDGBitmap, m_destCdgRectangle2, 1.0f, D2D1.BitmapInterpolationMode.Linear);

                    }

                    m_videoBitmapRectangle = new RawRectangleF((SurfaceR / 2) - (VideoW / 6), 0, (SurfaceR / 2) + (VideoW / 6), SurfaceH);

                    m_D2DContext.d2dContextCdgText.EndDraw();
                    m_D2DContext.d2dContextCdgText2.EndDraw();

                    m_D2DContext.d2dContext.BeginDraw();
                    m_D2DContext.d2dContext.Clear(Color.Transparent);
                    m_D2DContext.d2dContext.DrawBitmap(m_VideoBitmap, m_videoBitmapRectangle, 1.0f, D2D1.BitmapInterpolationMode.Linear);
                    if (!IsPlaying)
                    {
                        m_TitleName = DrawString(VideoW, VideoH, fontSize15, fontSize30);
                        m_D2DContext.d2dContext.DrawBitmap(m_TitleName, new RawRectangleF((SurfaceR / 2) - (VideoW / 4), 0, (SurfaceR / 2) + (VideoW / 4), SurfaceH), 1.0f, D2D1.BitmapInterpolationMode.Linear);
                    }

                    if (PlayerControl.IsAddToReserve)
                    {             
                        m_D2DContext.d2dContext.DrawRectangle(m_videoBitmapRectangle, roundedRecInColor, 10);
                    }

                    m_D2DContext.d2dContext.DrawImage(m_Shadow, new RawVector2(0, 0), D2D1.InterpolationMode.Linear, D2D1.CompositeMode.Xor);
                    m_D2DContext.d2dContext.DrawBitmap(m_Bitmap1, 1f, D2D1.BitmapInterpolationMode.Linear);
                    m_D2DContext.d2dContext.EndDraw();
                    m_D2DContext.swapChain.Present(0, DXGI.PresentFlags.None);

                    m_D2DContext.d2dContext2.BeginDraw();
                    m_D2DContext.d2dContext2.Clear(Color.Transparent);
                    m_D2DContext.d2dContext2.DrawBitmap(m_VideoBitmap, new RawRectangleF(SurfaceL2, SurfaceT2, SurfaceR2, SurfaceH2), 1.0f, D2D1.BitmapInterpolationMode.Linear);
                    if (!IsPlaying)
                    {
                        //m_D2DContext.d2dContext2.DrawBitmap(m_TitleName, new RawRectangleF(SurfaceL2, SurfaceT2, SurfaceR2, SurfaceH2), 1.0f, D2D1.BitmapInterpolationMode.Linear);
                        m_D2DContext.d2dContext2.DrawBitmap(m_TitleName, new RawRectangleF((SurfaceR2 / 2) - (VideoW / 2), 0, (SurfaceR2 / 2) + (VideoW / 2), SurfaceH2), 1.0f, D2D1.BitmapInterpolationMode.Linear);
                    }
                    else
                    {
                        string reservedSong = "R".PadRight(2) + addSong;
                        var stringSize = MeasureStringDX(reservedSong, SurfaceW2, m_textFormat15);

                        // m_roundedRec = new D2D1.RoundedRectangle() { Rect = new RawRectangleF(SurfaceW2 - stringSize.Width, (stringSize.Height / 3), (SurfaceR2 - 10), stringSize.Height + (stringSize.Height / 3)), RadiusX = stringSize.Height / 8, RadiusY = stringSize.Height / 8 };
                        m_roundedRec = new D2D1.RoundedRectangle() { Rect = new RawRectangleF((SurfaceW2 - 10) - (stringSize.Width + 10), (SurfaceT2 + 10), (SurfaceR2 - 10), (stringSize.Height + 10)), RadiusX = stringSize.Height / 8, RadiusY = stringSize.Height / 8 };
 
                        m_D2DContext.d2dContext2.DrawRoundedRectangle(m_roundedRec, roundedRecOutColor, 10f);
                        m_D2DContext.d2dContext2.FillRoundedRectangle(m_roundedRec, roundedRecInColor);
                       // m_D2DContext.d2dContext2.DrawText(reservedSong, m_textFormat15, new RawRectangleF(m_roundedRec.Rect.Left + (stringSize.Width / 12), (stringSize.Height / 2.5f), SurfaceR2, stringSize.Height + (stringSize.Height / 3)), m_textBrush);

                        m_D2DContext.d2dContext2.DrawText(reservedSong, m_textFormat15, new RawRectangleF((m_roundedRec.Rect.Left + 5), (m_roundedRec.Rect.Top - 2), SurfaceR2, m_roundedRec.Rect.Bottom), m_textBrush);

                        if (PlayerControl.GetNextSong != "")
                        {
                            string nextSong = "Next song: " + PlayerControl.GetNextSong;
                            var stringSize2 = MeasureStringDX(nextSong, SurfaceW2, m_textFormat10);
                            m_roundedRecNextSong = new D2D1.RoundedRectangle() { Rect = new RawRectangleF((SurfaceL2 + 10), (SurfaceT2 + 10), (m_roundedRec.Rect.Left - 15), (stringSize.Height + 10)), RadiusX = stringSize2.Height / 8, RadiusY = stringSize2.Height / 8 };

                            m_D2DContext.d2dContext2.DrawRoundedRectangle(m_roundedRecNextSong, roundedRecOutColor, 10f);
                            m_D2DContext.d2dContext2.FillRoundedRectangle(m_roundedRecNextSong, roundedRecInColor);
                            // m_D2DContext.d2dContext2.DrawText(nextSong, m_textFormat10, new RawRectangleF((m_roundedRecNextSong.Rect.Right / 2) - (stringSize2.Width / 2), (m_roundedRecNextSong.Rect.Bottom / 2) - ((stringSize2.Height - 10) / 2), (SurfaceR2 - 10), m_roundedRecNextSong.Rect.Bottom), m_textBrush, D2D1.DrawTextOptions.Clip);
                            m_D2DContext.d2dContext2.DrawText(nextSong, m_textFormat10, new RawRectangleF((SurfaceL2 + 10), (SurfaceT2 + 10), (m_roundedRec.Rect.Left - 15), (stringSize.Height + 10)), m_textBrush, D2D1.DrawTextOptions.Clip);

                        }
                    }
                    m_D2DContext.d2dContext2.DrawImage(m_compositeEffect, new RawVector2(0, 0), D2D1.InterpolationMode.Linear, D2D1.CompositeMode.Xor);
                    m_D2DContext.d2dContext2.DrawBitmap(m_Bitmap1FullScreen, 1f, D2D1.BitmapInterpolationMode.Linear);
                    m_D2DContext.d2dContext2.EndDraw();
                    m_D2DContext.swapChain2.Present(0, DXGI.PresentFlags.None);

                    UnloadResources();
                }

            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "DoRender", ex.LineNumber(), "GraphicUtilDX");

            }
        }

        public System.Drawing.SizeF MeasureStringDX(string Message, float Width, TextFormat format)
        {
            TextLayout layout = new TextLayout(m_D2DContext.dw_Factory, Message, format, Width, format.FontSize);

            return new System.Drawing.SizeF(layout.Metrics.Width, layout.Metrics.Height);
        }

        private System.Drawing.SizeF MeasureString(string txtstring, float fontSize)
        {
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            var stringSize = gr.MeasureString(txtstring, new System.Drawing.Font(fontFamily, fontSize));
            gr.Dispose();
            return stringSize;
        }

        private SharpDX.Direct2D1.Bitmap DrawString(int width, int height, float fontSize15, float fontSize30)
        {
            //Dont know how to draw this on SharpDx so i'm using system drawing to draw and convet it to SharpDX Bitmap.
            System.Drawing.Graphics gr = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(width, height, gr);

            gr.Dispose();
            gr = System.Drawing.Graphics.FromImage(bm);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            var strformat = new System.Drawing.StringFormat
            {
                Alignment = System.Drawing.StringAlignment.Center,
                LineAlignment = System.Drawing.StringAlignment.Center
            };

            gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            string b = "BossDoy KaraokeNow";
            var stringSize = MeasureString(b, fontSize15);
            path.AddString(b, fontFamily, (int)System.Drawing.FontStyle.Bold, fontSize15, new System.Drawing.Point((bm.Width / 2), (bm.Height / 2) - ((int)stringSize.Height) / 2), strformat);
            path.AddString("Select a song", fontFamily,
            (int)System.Drawing.FontStyle.Bold, fontSize30, new System.Drawing.Point(bm.Width / 2, (bm.Height / 2) + ((int)stringSize.Height) / 2), strformat);


            System.Drawing.Pen penOut = new System.Drawing.Pen(System.Drawing.Color.FromArgb(32, 117, 81), (fontSize30 / 4));
            penOut.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
            gr.DrawPath(penOut, path);

            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(234, 137, 6), (fontSize30 / 8));
            pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
            gr.DrawPath(pen, path);
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(128, 0, 255));
            gr.FillPath(brush, path);

            path.Dispose();
            penOut.Dispose();
            pen.Dispose();
            brush.Dispose();
            gr.Dispose();

            return ConvertToSlimDXBitmap(m_D2DContext.d2dContext, bm);

        }

        private void InitSurfaceSize()
        {
            SurfaceW = m_RenderingSurface.ClientRectangle.Width;
            SurfaceH = m_RenderingSurface.ClientRectangle.Height;
            SurfaceT = m_RenderingSurface.ClientRectangle.Top;
            SurfaceL = m_RenderingSurface.Left;
            SurfaceR = m_RenderingSurface.Right;
            SurfaceB = m_RenderingSurface.Bottom;

            SurfaceW2 = m_RenderingSurface2.ClientRectangle.Width;
            SurfaceH2 = m_RenderingSurface2.ClientRectangle.Height;
            SurfaceT2 = m_RenderingSurface2.ClientRectangle.Top;
            SurfaceL2 = m_RenderingSurface2.Left;
            SurfaceR2 = m_RenderingSurface2.Right;
            SurfaceB2 = m_RenderingSurface2.Bottom;
        }

        public void ResizeScreen1()
        {
            UnloadResources();
            InitSurfaceSize();
            LoadResources();
            m_D2DContext.ResizeScreen1();
        }


        public void ResizeScreen2()
        {
            UnloadResources();
            InitSurfaceSize();    
            LoadResources();
            m_D2DContext.ResizeScreen2();
        }

        ~GraphicUtilDX()
        {
            Dispose(false);
        }

        private void LoadResources()
        {
            m_textBrush = new D2D1.SolidColorBrush(m_D2DContext.d2dContext2, new SharpDX.Color(128, 0, 255));
            roundedRecOutColor = new D2D1.SolidColorBrush(m_D2DContext.d2dContext2, new SharpDX.Color(32, 117, 81));
            roundedRecInColor = new D2D1.SolidColorBrush(m_D2DContext.d2dContext2, new SharpDX.Color(234, 137, 6));
            m_textFormat15 = new TextFormat(m_D2DContext.dw_Factory, "Arial", FontWeight.UltraBold, FontStyle.Normal, fontSize15);
            m_textFormat10 = new TextFormat(m_D2DContext.dw_Factory, "Arial", FontWeight.Bold, FontStyle.Normal, fontSize10);
            //m_textFormat10.ParagraphAlignment = ParagraphAlignment.Center;
            //m_textFormat10.TextAlignment = TextAlignment.Center;
            m_textFormat10.SetParagraphAlignment(ParagraphAlignment.Center);
            m_textFormat10.SetTextAlignment(TextAlignment.Center);
            fontFamily = new System.Drawing.FontFamily("Arial");
            m_Bitmap1 = new D2D1.Bitmap1(m_D2DContext.d2dContextCdgText, new Size2(SurfaceW, SurfaceH), BitmapProps1);
            m_Bitmap1FullScreen = new D2D1.Bitmap1(m_D2DContext.d2dContextCdgText2, new Size2(SurfaceW2, SurfaceH2), BitmapProps1);
            m_Shadow = new D2D1.Effects.Shadow(m_D2DContext.d2dContextCdgText);
            m_ShadowFullScreen = new D2D1.Effects.Shadow(m_D2DContext.d2dContextCdgText2);
            m_affineTransformEffect = new D2D1.Effects.AffineTransform2D(m_D2DContext.d2dContextCdgText2);
            m_compositeEffect = new D2D1.Effects.Composite(m_D2DContext.d2dContextCdgText2);
        }

        private void UnloadResources()
        {
            m_compositeEffect.Dispose();
            m_affineTransformEffect.Dispose();
            m_ShadowFullScreen.Dispose();
            m_Shadow.Dispose();
            m_Bitmap1FullScreen.Dispose();
            m_Bitmap1.Dispose();
            fontFamily.Dispose();
            m_textFormat15.Dispose();
            m_textFormat10.Dispose();
            roundedRecInColor.Dispose();
            roundedRecOutColor.Dispose();
            m_textBrush.Dispose();

            if (m_TitleName != null)
                m_TitleName.Dispose();
            if (m_VideoBitmap != null)
                m_VideoBitmap.Dispose();
            if (m_CDGBitmap != null)
                m_CDGBitmap.Dispose();
        }

        public void Dispose(bool _bDisposeManagedResources)
        {
            UnloadResources();

            if (m_VideoBitmap != null)
            {
                m_VideoBitmap.Dispose();
            }

            if (m_CDGBitmap != null)
            {
                m_CDGBitmap.Dispose();
            }

            if (_bDisposeManagedResources)
            {
                m_D2DContext.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
