using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using System.Reflection;
using OpenTK.Mathematics;

namespace Fractals.Rendering;

internal sealed class Renderer : GameWindow {
    public Renderer(int width, int height, string title) :
        base(GameWindowSettings.Default,
        new NativeWindowSettings() {
            Size = (width, height),
            Title = title,
            StartVisible = false,
            StartFocused = true,
            MaximumSize = (width, height),
            MinimumSize = (width, height)
        }) {
        this.CenterWindow();
        _windowTitle = title;
    }

    private int vertexArrayHandle;
    private int vertexBufferHandle;
    private int indexBufferHandle;

    private Fractal currentFractal;
    private Mandelbrot mandelbrot;
    private Julia julia;
    private BurningShip multibrot;

    private readonly string _windowTitle;


    protected override void OnLoad() {
        base.OnLoad();

        this.IsVisible = true;

        float[] vertices = {
            0f, this.ClientSize.Y,                  // top left
            this.ClientSize.X, this.ClientSize.Y,   // top right
            this.ClientSize.X, 0f,                  // bottom right
            0f, 0f                                  // bottom left
        };

        int[] indices = {                           // clockwise rotation
            0, 1, 2,
            0, 2, 3
        };

        vertexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        indexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        vertexArrayHandle = GL.GenVertexArray();
        GL.BindVertexArray(vertexArrayHandle);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        mandelbrot = new Mandelbrot(this.ClientSize.X, this.ClientSize.Y, true);
        julia = new Julia(this.ClientSize.X, this.ClientSize.Y, true);
        multibrot = new BurningShip(this.ClientSize.X, this.ClientSize.Y, true);

        currentFractal = multibrot;
    }

    protected override void OnUnload() {
        base.OnUnload();

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(vertexArrayHandle);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(indexBufferHandle);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferHandle);

        mandelbrot?.Dispose();
        julia?.Dispose();
        multibrot?.Dispose();
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(currentFractal.Handle);
        GL.BindVertexArray(vertexArrayHandle);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        this.SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        var keyboardState = this.KeyboardState;

        string iter = "", coords = "", fps = $"FPS: {1 / args.Time:F0}";

        currentFractal.HandleInput(args.Time, this.KeyboardState);

        if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D1)) currentFractal = mandelbrot;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D2)) currentFractal = julia;
        else if (keyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D3)) currentFractal = multibrot;

        this.Title = $"{currentFractal.GetType().Name} | {iter}, {coords} | {fps}";
    }
}

