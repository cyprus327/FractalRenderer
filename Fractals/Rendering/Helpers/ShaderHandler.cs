using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Fractals.Rendering.Helpers;

public sealed class ShaderHandler : IDisposable
{
    public ShaderHandler(string vertexShaderSource, string fragmentShaderSource)
    {
        _vertShaderHandle = CreateShaderObject(ShaderType.VertexShader, vertexShaderSource);
        _fragShaderHandle = CreateShaderObject(ShaderType.FragmentShader, fragmentShaderSource);

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


    public void Uniform(string name, double value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, value);
    }

    public void Uniform(string name, double a, double b)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform2(location, a, b);
    }

    public void Uniform(string name, double a, double b, double c)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(location, a, b, c);
    }

    public void Uniform(string name, double a, double b, double c, double d)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform4(location, a, b, c, d);
    }

    private int CreateShaderObject(ShaderType shaderType, string shaderSource)
    {
        int shaderObjectId = GL.CreateShader(shaderType);
        GL.ShaderSource(shaderObjectId, shaderSource);
        GL.CompileShader(shaderObjectId);

        //Console.WriteLine(GL.GetShaderInfoLog(shaderObjectId));

        return shaderObjectId;
    }

    ~ShaderHandler()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (disposed) return;

        GL.DeleteProgram(Handle);

        disposed = true;
        GC.SuppressFinalize(this);
    }
}
