#if DEBUG

namespace Substance.Graphics;

public static class ShaderSources
{
    private const string c_glVersion = "#version 330 core";
    private const string c_glesVersion = "#version 300 es";

    public const string VertexShaderSourceGL = c_glVersion + @"
        layout (location = 0) in vec2 aPos;

        void main()
        {
            gl_Position = vec4(aPos, 0.0, 1.0);
        }
    ";
    public const string FragmentShaderSourceGL = c_glVersion + @"
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(0.392f, 0.584f, 0.929f, 1.0f);
        }
    ";

    public const string VertexShaderSourceGLES = c_glesVersion + @"
        layout (location = 0) in vec2 aPos;

        void main()
        {
            gl_Position = vec4(aPos, 0.0, 1.0);
        }
    ";
    public const string FragmentShaderSourceGLES = c_glesVersion + @"
        precision highp float;

        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(0.392f, 0.584f, 0.929f, 1.0f);
        }
    ";
}

#endif