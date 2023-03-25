using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering;

internal abstract class Fractal : IDisposable {
    public abstract void HandleInput(double d, OpenTK.Windowing.GraphicsLibraryFramework.KeyboardState k);

    public abstract int Handle { get; init; }

    private bool disposed = false;

    ~Fractal() {
        Dispose();
    }

    public void Dispose() {
        if (disposed) return;

        GL.UseProgram(0);
        GL.DeleteProgram(Handle);

        disposed = true;
        GC.SuppressFinalize(this);
    }
}