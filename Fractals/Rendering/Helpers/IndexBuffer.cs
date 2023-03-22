using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering.Helpers;

internal sealed class IndexBuffer : IDisposable {
    public IndexBuffer(int indexCoumt, bool isStatic = true) {
        this.disposed = false;

        if (indexCoumt < 1 || indexCoumt > MaxIndexCount) throw new ArgumentOutOfRangeException(nameof(indexCoumt));

        this.IndexCount = indexCoumt;
        this.IsStatic = isStatic;

        this.IndexBufferHandle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferHandle);
        GL.BufferData(BufferTarget.ElementArrayBuffer, this.IndexCount * sizeof(int), IntPtr.Zero, isStatic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public readonly int IndexBufferHandle;
    public readonly int IndexCount;
    private readonly bool IsStatic;

    public static readonly int MaxIndexCount = 250000;

    private bool disposed;

    public void SetData(int[] data, int count) {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length == 0) throw new ArgumentException(nameof(data));
        if (count <= 0 || count > this.IndexCount || count > data.Length) throw new ArgumentOutOfRangeException(nameof(count));

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.IndexBufferHandle);
        GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(int), data);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    ~IndexBuffer() {
        this.Dispose();
    }

    public void Dispose() {
        if (this.disposed) return;

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(this.IndexBufferHandle);

        this.disposed = true;
        GC.SuppressFinalize(this);
    }
}

