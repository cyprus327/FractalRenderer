using Fractals.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fractals.Types;

internal sealed class Mandelbulb : Fractal {
    public Mandelbulb(int width, int height) {
        Initialize(Shaders.MandelbulbFragCode, out int handle);
        Handle = handle;

        int viewportLocation = GL.GetUniformLocation(Handle, "ViewportSize");
        GL.Uniform2(viewportLocation, (float)width, (float)height);

        zoomUniformLocation = GL.GetUniformLocation(Handle, "Zoom");
        GL.Uniform1(zoomUniformLocation, ZoomLevel);

        rotationUniformLocation = GL.GetUniformLocation(Handle, "Rotation");
        GL.Uniform3(rotationUniformLocation, RotX, RotY, RotZ);

        maxIterUniformLocation = GL.GetUniformLocation(Handle, "MaxIter");
        GL.Uniform1(maxIterUniformLocation, MaxIterations);
    }

    public override int Handle { get; init; }
    public override string Info { get => $"I: {MaxIterations}, R: ({RotX:F2}, {RotY:F2}, {RotZ:F2}), Z: {ZoomLevel:F4}"; }

    public float ZoomLevel { get; set; } = 1f;
    public float RotX { get; set; } = 0f;
    public float RotY { get; set; } = 0f;
    public float RotZ { get; set; } = 0f;
    public int MaxIterations { get; set; } = 150;

    private readonly int zoomUniformLocation;
    private readonly int rotationUniformLocation;
    private readonly int maxIterUniformLocation;

    public override void HandleInput(double deltaTime, KeyboardState keyboardState, MouseState mouseState) {
        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E))
            ZoomLevel *= (float)Math.Pow(2, deltaTime);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q))
            ZoomLevel *= (float)Math.Pow(0.5, deltaTime);
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.R))
            ZoomLevel = 1f;

        GetMouseDelta(mouseState, out float dx, out float dy);
        RotX += dy / ZoomLevel * 0.002f;
        RotY += dx / ZoomLevel * 0.002f;

        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            RotX -= (float)deltaTime / 3f;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            RotX += (float)deltaTime / 3f;
        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            RotY += (float)deltaTime / 3f;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            RotY -= (float)deltaTime / 3f;


        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z))
            MaxIterations -= 1;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.X))
            MaxIterations += 1;

        MaxIterations = Math.Max(10, Math.Min(500, MaxIterations));

        GL.Uniform1(zoomUniformLocation, (float)ZoomLevel);
        GL.Uniform3(rotationUniformLocation, RotX, RotY, RotZ);
        GL.Uniform1(maxIterUniformLocation, MaxIterations);
    }
}
