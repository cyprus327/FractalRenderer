using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering.Helpers;

public sealed class ShaderHandler : IDisposable {
    public ShaderHandler(string vertexShaderCode, string fragmentShaderCode) {
        _vertShaderHandle = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_vertShaderHandle, vertexShaderCode);
        GL.CompileShader(_vertShaderHandle);

        _fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_fragShaderHandle, fragmentShaderCode);
        GL.CompileShader(_fragShaderHandle);

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, _vertShaderHandle);
        GL.AttachShader(Handle, _fragShaderHandle);

        GL.LinkProgram(Handle);

        GL.DetachShader(Handle, _vertShaderHandle);
        GL.DetachShader(Handle, _fragShaderHandle);

        GL.DeleteShader(_vertShaderHandle);
        GL.DeleteShader(_fragShaderHandle);
    }

    public readonly int Handle;

    private readonly int _vertShaderHandle, _fragShaderHandle;

    private bool disposed;


    public void Uniform(string name, int a) {
        GL.Uniform1(GL.GetUniformLocation(Handle, name), a);
    }

    public void Uniform(string name, double a) {
        GL.Uniform1(GL.GetUniformLocation(Handle, name), a);
    }

    public void Uniform(string name, double a, double b) {
        GL.Uniform2(GL.GetUniformLocation(Handle, name), a, b);
    }

    public void Uniform(string name, double a, double b, double c) {
        GL.Uniform3(GL.GetUniformLocation(Handle, name), a, b, c);
    }

    public void Uniform(string name, double a, double b, double c, double d) {
        GL.Uniform4(GL.GetUniformLocation(Handle, name), a, b, c, d);
    }

    ~ShaderHandler() {
        Dispose();
    }

    public void Dispose() {
        if (disposed) return;

        GL.DeleteProgram(Handle);

        disposed = true;
        GC.SuppressFinalize(this);
    }
}
