using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering.Helpers;

public sealed class VertexBuffer : IDisposable {
    public VertexBuffer(VertexInfo info, int vertexCount, bool isStatic = true) { 
        this.disposed = false;

        if (vertexCount < 1 || vertexCount > MaxVertexCount) throw new ArgumentException(nameof(vertexCount));

        this.Info = info;
        this.VertexCount = vertexCount;
        this.IsStatic = isStatic;

        this.VertexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferHandle);
        GL.BufferData(BufferTarget.ArrayBuffer, this.VertexCount * this.Info.SizeInBytes, IntPtr.Zero, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public readonly int VertexCount;
    public readonly VertexInfo Info;
    public readonly int VertexBufferHandle;
    public readonly bool IsStatic;

    public static readonly int MaxVertexCount = 100000;

    private bool disposed;

    public void SetData<T>(T[] data, int count) where T : struct {
        if (typeof(T) != this.Info.Type) throw new ArgumentException(nameof(data));
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length == 0) throw new ArgumentOutOfRangeException(nameof(data));
        if (count <= 0 || count > this.VertexCount || count > data.Length) throw new ArgumentOutOfRangeException(nameof(count));

        GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferHandle);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, count * this.Info.SizeInBytes, data);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    ~VertexBuffer() {
        this.Dispose();
    }

    public void Dispose() {
        if (this.disposed) return;

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(this.VertexBufferHandle);

        disposed = true;
        GC.SuppressFinalize(this);
    }
}