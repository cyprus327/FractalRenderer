using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering.Helpers;

internal sealed class VertexArray : IDisposable
{
    public VertexArray(VertexBuffer vertexBuffer)
    {
        disposed = false;

        if (vertexBuffer == null) throw new ArgumentNullException(nameof(vertexBuffer));

        VertexBuffer = vertexBuffer;

        VertexArrayHandle = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayHandle);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer.VertexBufferHandle);

        int vertexSizeInBytes = VertexBuffer.Info.SizeInBytes;
        var attributes = VertexBuffer.Info.VertexAttributes;
        foreach (var attr in attributes)
        {
            GL.VertexAttribPointer(attr.Index, attr.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attr.Offset);
            GL.EnableVertexAttribArray(attr.Index);
        }

        GL.BindVertexArray(0);
    }

    public readonly VertexBuffer VertexBuffer;
    public readonly int VertexArrayHandle;

    private bool disposed;

    ~VertexArray()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (disposed) return;

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(VertexArrayHandle);

        disposed = true;
        GC.SuppressFinalize(this);
    }
}
