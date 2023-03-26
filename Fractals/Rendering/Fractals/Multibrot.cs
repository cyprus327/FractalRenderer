using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering;

internal sealed class Multibrot : Fractal {
    public Multibrot(int width, int height, bool showLogs = false) {
        Initialize(Shaders.MultibrotFragCode, out int handle, showLogs, true);
        Handle = handle;

        int viewportLocation = GL.GetUniformLocation(Handle, "ViewportSize");
        GL.Uniform2(viewportLocation, (float)width, (float)height);

        zoomUniformLocation = GL.GetUniformLocation(Handle, "Zoom");
        GL.Uniform1(zoomUniformLocation, ZoomLevel);

        centerUniformLocation = GL.GetUniformLocation(Handle, "Center");
        GL.Uniform2(centerUniformLocation, CenterX, CenterY);

        maxIterUniformLocation = GL.GetUniformLocation(Handle, "MaxIter");
        GL.Uniform1(maxIterUniformLocation, MaxIterations);

        powerUniformLocation = GL.GetUniformLocation(Handle, "Power");
        GL.Uniform1(powerUniformLocation, Power);
    }

    public override int Handle { get; init; }
    public override string Info { get => $"I: {MaxIterations}, P: ({CenterX:F16}, {CenterY:F16}), Z: {ZoomLevel:F4}, E: {Power:F4}"; }

    public double ZoomLevel { get; set; } = 0.5d;
    public double CenterX { get; set; } = -1d;
    public double CenterY { get; set; } = 0d;
    public int MaxIterations { get; set; } = 1000;
    public double Power { get; set; } = Math.PI / 2;

    private readonly int zoomUniformLocation;
    private readonly int centerUniformLocation;
    private readonly int maxIterUniformLocation;
    private readonly int powerUniformLocation;

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

        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.V))
            Power += deltaTime;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.C))
            Power -= deltaTime;

        MaxIterations = Math.Max(400, Math.Min(20000, MaxIterations));

        GL.Uniform1(zoomUniformLocation, ZoomLevel);
        GL.Uniform2(centerUniformLocation, CenterX, CenterY);
        GL.Uniform1(maxIterUniformLocation, MaxIterations);
        GL.Uniform1(powerUniformLocation, Power);
    }
}
