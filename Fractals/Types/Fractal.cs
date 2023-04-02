using Fractals.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fractals.Types;

internal abstract class Fractal : IDisposable {
    public abstract void HandleInput(double d, KeyboardState k, MouseState m);

    public abstract int Handle { get; init; }
    public abstract string Info { get; }

    private bool disposed = false;
    
    public static void Initialize(string fragCode, out int handle) {
        int vertShaderHandle = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShaderHandle, Shaders.VertexCode);
        GL.CompileShader(vertShaderHandle);

        string vertShaderInfoLog = GL.GetShaderInfoLog(vertShaderHandle);
        if (vertShaderInfoLog != string.Empty) {
            Console.WriteLine(vertShaderInfoLog);
        }

        int fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShaderHandle, fragCode);
        GL.CompileShader(fragShaderHandle);

        string fragShaderInfoLog = GL.GetShaderInfoLog(fragShaderHandle);
        if (fragShaderInfoLog != string.Empty) {
            Console.WriteLine(fragShaderInfoLog);
        }

        GL.UseProgram(0);
        handle = GL.CreateProgram();

        GL.AttachShader(handle, vertShaderHandle);
        GL.AttachShader(handle, fragShaderHandle);

        GL.LinkProgram(handle);

        GL.DetachShader(handle, vertShaderHandle);
        GL.DetachShader(handle, fragShaderHandle);

        GL.DeleteShader(vertShaderHandle);
        GL.DeleteShader(fragShaderHandle);

        GL.UseProgram(handle);
    }

    public void GetMouseDelta(MouseState state, out float dx, out float dy) {
        if (state.IsButtonDown(MouseButton.Button1)) {
            dx = state.Delta.X;
            dy = state.Delta.Y;
        }
        else {
            dx = 0;
            dy = 0;
        }
    }

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