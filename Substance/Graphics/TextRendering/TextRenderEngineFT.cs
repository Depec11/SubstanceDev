using System.Runtime.InteropServices;
using FreeTypeSharp;
using Substance.Logging;
using Substance.Maths;
using static FreeTypeSharp.FT;

namespace Substance.Graphics.TextRendering;

public unsafe class TextRenderEngineFT : TextRenderEngine
{
    private readonly FT_LibraryRec_* _library;
    private readonly FT_FaceRec_* _face;
    private readonly byte[] _fontData = [];
    private readonly Dictionary<uint, Dictionary<char, GlyphRenderResult>> _glyphCaches= [];
    private uint _fontSize;

    public TextRenderEngineFT(Uri fontPath, uint defaultSize)
    {
        FT_Error error;

        FT_LibraryRec_* pLibrary = null;

        error = FT_Init_FreeType(&pLibrary);

        if (error is not FT_Error.FT_Err_Ok)
        {
            Log.Error($"[{nameof(TextRenderEngineFT)}] 初始化失败：{error}");
        
            return;
        }

        _library = pLibrary;

        _face = LoadFont(fontPath, out _fontData);

        if (!SetFontSize(defaultSize))
        {
            return;
        }

        Log.Info($"[{nameof(TextRenderEngineFT)}] 加载字体成功：{fontPath}");
    }

    private FT_FaceRec_* LoadFont(Uri fontPath, out byte[] fontData)
    {
        using var stream = Assets.Open(fontPath);

        fontData = [];

        if (stream is null)
        {
            Log.Error($"[{nameof(TextRenderEngineFT)}] 打开字体文件失败：{fontPath}");
            return null;
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        fontData = memoryStream.ToArray();

        FT_Error error;
        FT_FaceRec_* pFace;

        fixed (byte* pData = fontData)
        {
            error = FT_New_Memory_Face(_library, pData, fontData.Length, 0, &pFace);
            if (error is not FT_Error.FT_Err_Ok)
            {
                Log.Error($"[{nameof(TextRenderEngineFT)}] 加载字体失败：{error}");
                return null;
            }
        }

        return pFace;
    }

    protected override void MeasureStringOverride(string text, uint fontSize, ref Vector2<int> startPosition, ref Vector2<int> endPosition)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var minY = int.MaxValue;
        var maxY = int.MinValue;

        var currentX = 0;
        var baselineY = GetBaselineY(fontSize);

        foreach (var c in text)
        {
            var glyph = RenderChar(c, fontSize);

            var left = currentX + glyph.Left;
            var top = baselineY - glyph.Top;
            var right = left + (int)glyph.Width;
            var bottom = top + (int)glyph.Height;
        
            minX = Math.Min(minX, left);
            maxX = Math.Max(maxX, right);
            minY = Math.Min(minY, top);
            maxY = Math.Max(maxY, bottom);

            currentX += (int)glyph.AdvanceX;
        }

        startPosition = new Vector2<int>(minX, minY);
        endPosition = new Vector2<int>(maxX, maxY);
    }

    protected override void RenderStringOverride(string text, uint fontSize, Color foregroundColor, Color backgroundColor, ref byte[] texture, ref Vector2<int> size)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var startPosition = new Vector2<int>();
        var endPosition = new Vector2<int>();

        MeasureStringOverride(text, fontSize, ref startPosition, ref endPosition);
        size = endPosition - startPosition;

        if (size.X <= 0 || size.Y <= 0)
        {
            return;
        }

        texture = new byte[size.X * size.Y * 4];

        var baselineY = GetBaselineY(fontSize);
        var offsetX = -startPosition.X;
        var offsetY = -startPosition.Y;

        var cursorX = 0;
        var fr = foregroundColor.R;
        var fg = foregroundColor.G;
        var fb = foregroundColor.B;
        var fa = foregroundColor.A;
        var br = backgroundColor.R;
        var bg = backgroundColor.G;
        var bb = backgroundColor.B;
        var ba = backgroundColor.A;

        foreach (char c in text)
        {
            var glyph = RenderChar(c, fontSize);
            
            var drawX = cursorX + glyph.Left + offsetX;
            var drawY = baselineY - glyph.Top + offsetY;

            var rowPosition = 0u;

            for (var row = 0; row < glyph.Height; row++)
            {
                for (var col = 0; col < glyph.Width; col++)
                {
                    var alpha = glyph.Data[rowPosition + col];

                    byte r, g, b, a;

                    r = (byte)((fr * alpha + br * (255 - alpha)) / 255);
                    g = (byte)((fg * alpha + bg * (255 - alpha)) / 255);
                    b = (byte)((fb * alpha + bb * (255 - alpha)) / 255);
                    a = (byte)((fa * alpha + ba * (255 - alpha)) / 255);                    

                    var px = drawX + col;
                    var py = drawY + row;

                    if (px >= 0 && px < size.X && py >= 0 && py < size.Y)
                    {
                        var index = (py * size.X + px) * 4;
                        texture[index]      = r;
                        texture[index + 1]  = g;
                        texture[index + 2]  = b;
                        texture[index + 3]  = a;
                    }
                }

                rowPosition += glyph.Width;
            }

            cursorX += glyph.AdvanceX;
        }
    }

    private bool SetFontSize(uint size)
    {
        var error = FT_Set_Pixel_Sizes(_face, 0, size);
        
        if (error is not FT_Error.FT_Err_Ok)
        {
            Log.Error($"[{nameof(TextRenderEngineFT)}] 设置字体大小失败：{error}");
            return false;
        }

        _fontSize = size;
        return true;
    }

    private int GetBaselineY(uint fontSize)
    {
        if (_fontSize != fontSize)
        {
            if (!SetFontSize(fontSize)) 
            {
                return 0;
            }
        }

        var metrics = _face->size->metrics;
        return (int)(metrics.ascender >> 6);
    }

    private GlyphRenderResult RenderChar(char c, uint fontSize)
    {
        if (_glyphCaches.TryGetValue(fontSize, out var cache))
        {
            if (cache.TryGetValue(c, out var glyphCache))
            {
                return glyphCache;
            }
        }
        
        if (_fontSize != fontSize)
        {
            if (!SetFontSize(fontSize))
            {
                return GlyphRenderResult.Empty;
            }
        }

        var glyphIndex = FT_Get_Char_Index(_face, c);

        var error = FT_Load_Glyph(_face, glyphIndex, FT_LOAD.FT_LOAD_RENDER);

        if (error is not FT_Error.FT_Err_Ok)
        {
            Log.Error($"[{nameof(TextRenderEngineFT)}] 加载字符失败：{error}");
            return GlyphRenderResult.Empty;
        }

        var glyph = _face->glyph;
        
        var bitmap = glyph->bitmap;
        var width = bitmap.width;
        var height = bitmap.rows;
        var data = new byte[width * height];

        if (bitmap.buffer is null || width <= 0 || height <= 0)
        {
            Log.Error($"[{nameof(TextRenderEngineFT)}] 获取字符位图失败：{error}");
            return GlyphRenderResult.Empty;
        }

        Marshal.Copy((IntPtr)bitmap.buffer, data, 0, data.Length);

        var res = new GlyphRenderResult(
            data, 
            width, 
            height, 
            glyph->bitmap_left, 
            glyph->bitmap_top, 
            (int)(glyph->advance.x >> 6)
        );

        if (cache is null)
        {
            cache = [];
            _glyphCaches[fontSize] = cache;
        }

        cache[c] = res;
        return res;
    }

    protected override void OnDisposeOverride()
    {
        _glyphCaches.Clear();

        FT_Done_Face(_face);
        FT_Done_Library(_library);
    }
}
