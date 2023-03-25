using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering;

internal sealed class Mandelbrot : Fractal {
    public Mandelbrot(int width, int height, bool showLogs = false) {
        int vertShaderHandle = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShaderHandle, Shaders.VertexCode);
        GL.CompileShader(vertShaderHandle);

        if (showLogs) {
            string vertShaderInfoLog = GL.GetShaderInfoLog(vertShaderHandle);
            if (vertShaderInfoLog != string.Empty) {
                Console.WriteLine(vertShaderInfoLog);
            }
        }

        int fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShaderHandle, Shaders.MandelbrotFragCode);
        GL.CompileShader(fragShaderHandle);

        if (showLogs) {
            string fragShaderInfoLog = GL.GetShaderInfoLog(fragShaderHandle);
            if (fragShaderInfoLog != string.Empty) {
                Console.WriteLine(fragShaderInfoLog);
            }
        }

        GL.UseProgram(0);
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertShaderHandle);
        GL.AttachShader(Handle, fragShaderHandle);

        GL.LinkProgram(Handle);

        GL.DetachShader(Handle, vertShaderHandle);
        GL.DetachShader(Handle, fragShaderHandle);

        GL.DeleteShader(vertShaderHandle);
        GL.DeleteShader(fragShaderHandle);

        GL.UseProgram(Handle);

        int viewportLocation = GL.GetUniformLocation(Handle, "ViewportSize");
        GL.Uniform2(viewportLocation, (float)width, (float)height);

        ZoomUniformLocation = GL.GetUniformLocation(Handle, "Zoom");
        GL.Uniform1(ZoomUniformLocation, ZoomLevel);

        CenterUniformLocation = GL.GetUniformLocation(Handle, "Center");
        GL.Uniform2(CenterUniformLocation, CenterX, CenterY);

        MaxIterUniformLocation = GL.GetUniformLocation(Handle, "MaxIter");
        GL.Uniform1(MaxIterUniformLocation, MaxIterations);
    }

    public override int Handle { get; init; }

    public int ZoomUniformLocation { get; init; }
    public int CenterUniformLocation { get; init; }
    public int MaxIterUniformLocation { get; init; }

    public double ZoomLevel { get; set; } = 0.5d;
    public double CenterX { get; set; } = -1d;
    public double CenterY { get; set; } = 0d;
    public int MaxIterations { get; set; } = 1000;

    public override void HandleInput(double deltaTime, OpenTK.Windowing.GraphicsLibraryFramework.KeyboardState keyboardState) {
        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E))
            ZoomLevel *= Math.Pow(2, deltaTime);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q))
            ZoomLevel *= Math.Pow(0.5, deltaTime);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.R))
            ZoomLevel = 1f;

        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            CenterY += deltaTime / ZoomLevel;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            CenterY -= deltaTime / ZoomLevel;
        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            CenterX -= deltaTime / ZoomLevel;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            CenterX += deltaTime / ZoomLevel;

        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z))
            MaxIterations -= (int)(deltaTime * MaxIterations);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.X))
            MaxIterations += (int)(deltaTime * MaxIterations);

        MaxIterations = Math.Max(400, Math.Min(20000, MaxIterations));

        GL.Uniform1(ZoomUniformLocation, ZoomLevel);
        GL.Uniform2(CenterUniformLocation, CenterX, CenterY);
        GL.Uniform1(MaxIterUniformLocation, MaxIterations);
    }
}
