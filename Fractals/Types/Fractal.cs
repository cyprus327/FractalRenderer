﻿using Fractals.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Fractals.Types;

internal abstract class Fractal : IDisposable {
    public abstract void HandleInput(double d, OpenTK.Windowing.GraphicsLibraryFramework.KeyboardState k);

    public abstract int Handle { get; init; }
    public abstract string Info { get; }

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