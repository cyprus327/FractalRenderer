using Fractals.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fractals.Types;

internal sealed class Mandelbrot : Fractal {
    public Mandelbrot(int width, int height) {
        Initialize(Shaders.MandelbrotFragCode, out int handle);
        Handle = handle;

        int viewportLocation = GL.GetUniformLocation(Handle, "ViewportSize");
        GL.Uniform2(viewportLocation, (float)width, (float)height);

        zoomUniformLocation = GL.GetUniformLocation(Handle, "Zoom");
        GL.Uniform1(zoomUniformLocation, ZoomLevel);

        centerUniformLocation = GL.GetUniformLocation(Handle, "Center");
        GL.Uniform2(centerUniformLocation, CenterX, CenterY);

        maxIterUniformLocation = GL.GetUniformLocation(Handle, "MaxIter");
        GL.Uniform1(maxIterUniformLocation, MaxIterations);
    }

    public override int Handle { get; init; }
    public override string Info { get => $"I: {MaxIterations}, P: ({CenterX:F16}, {CenterY:F16}), Z: {ZoomLevel:F4}"; }

    public double ZoomLevel { get; set; } = 0.5d;
    public double CenterX { get; set; } = -1d;
    public double CenterY { get; set; } = 0d;
    public int MaxIterations { get; set; } = 1000;

    private readonly int zoomUniformLocation;
    private readonly int centerUniformLocation;
    private readonly int maxIterUniformLocation;

    public override void HandleInput(double deltaTime, KeyboardState keyboardState, MouseState mouseState) {
        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E))
            ZoomLevel *= Math.Pow(2, deltaTime);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q))
            ZoomLevel *= Math.Pow(0.5, deltaTime);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.R))
            ZoomLevel = 1f;

        GetMouseDelta(mouseState, out float dx, out float dy);
        CenterX -= dx / ZoomLevel * 0.002;
        CenterY += dy / ZoomLevel * 0.002;

        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z))
            MaxIterations -= (int)(deltaTime * MaxIterations);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.X))
            MaxIterations += (int)(deltaTime * MaxIterations);

        MaxIterations = Math.Max(400, Math.Min(20000, MaxIterations));

        GL.Uniform1(zoomUniformLocation, ZoomLevel);
        GL.Uniform2(centerUniformLocation, CenterX, CenterY);
        GL.Uniform1(maxIterUniformLocation, MaxIterations);
    }
}
