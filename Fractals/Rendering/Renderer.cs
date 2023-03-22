using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace Fractals.Rendering;
internal sealed class Renderer : GameWindow {
    public Renderer(int width, int height, string title) :
        base(GameWindowSettings.Default,
        new NativeWindowSettings() {
            Size = (width, height),
            Title = title,
            StartVisible = false,
            StartFocused = true
        }) {
        this.CenterWindow();
        _windowTitle = title;
    }

    public Color BackgroundColor { get; set; } = Color.FromArgb(80, 80, 80);

    private int vertexArrayHandle;
    private int vertexBufferHandle;
    private int indexBufferHandle;
    private int shaderProgramHandle;

    private readonly string _windowTitle;

    private int zoomUniformLocation, centerUniformLocation;
    private double zoomLevel = 1.1d;
    private double centerX = 0d, centerY = 0d;

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

        string vertexShaderCode = @"
            #version 410
        
            uniform vec2 ViewportSize;

            layout (location = 0) in vec2 aPosition;

            void main() {
                float nx = aPosition.x / ViewportSize.x * 2 - 1;
                float ny = aPosition.y / ViewportSize.y * 2 - 1;
                gl_Position = vec4(nx, ny, 0, 1);
            }
        ";

        string fragmentShaderCode = @"
            #version 410

            precision highp float;

            #define MAX_ITER 1000

            uniform double Zoom;
            uniform dvec2 Center;
            uniform vec2 Resolution;

            in vec4 vColor;

            out vec4 fragColor;

            void main() {
                dvec2 uv = (gl_FragCoord.xy - dvec2(Resolution.x / 2.0, Resolution.y / 2.0)) / Resolution.y / Zoom + Center;
    
                dvec2 z = dvec2(0.0, 0.0);
    
                int iter = 0;
                while (iter < MAX_ITER && dot(z, z) < 4.0) {
                    z = dvec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + uv;
                    iter++;
                }
    
                float t = float(iter) / float(MAX_ITER);
                fragColor = vec4(
                    9.5 * (1.0 - t) * (1.0 - t) * (1.0 - t) * t,
                    15.0 * (1.0 - t) * (1.0 - t) * t * t,
                    8.0 * (1.0 - t) * t * t * t, 
                    1.0
                );
            }
        ";

        int vertShaderHandle = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShaderHandle, vertexShaderCode);
        GL.CompileShader(vertShaderHandle);

        int fragShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShaderHandle, fragmentShaderCode);
        GL.CompileShader(fragShaderHandle);

        shaderProgramHandle = GL.CreateProgram();

        GL.AttachShader(shaderProgramHandle, vertShaderHandle);
        GL.AttachShader(shaderProgramHandle, fragShaderHandle);

        GL.LinkProgram(shaderProgramHandle);

        GL.DetachShader(shaderProgramHandle, vertShaderHandle);
        GL.DetachShader(shaderProgramHandle, fragShaderHandle);

        GL.DeleteShader(vertShaderHandle);
        GL.DeleteShader(fragShaderHandle);

        GL.UseProgram(shaderProgramHandle);

        int viewportLocation = GL.GetUniformLocation(shaderProgramHandle, "ViewportSize");
        GL.Uniform2(viewportLocation, (float)this.ClientSize.X, (float)this.ClientSize.Y);

        int resolutionLocation = GL.GetUniformLocation(this.shaderProgramHandle, "Resolution");
        GL.Uniform2(resolutionLocation, (float)this.ClientSize.X, (float)this.ClientSize.Y);

        zoomUniformLocation = GL.GetUniformLocation(this.shaderProgramHandle, "Zoom");
        GL.Uniform1(zoomUniformLocation, zoomLevel);

        centerUniformLocation = GL.GetUniformLocation(this.shaderProgramHandle, "Center");
        GL.Uniform2(centerUniformLocation, centerX, centerY);
    }

    protected override void OnUnload() {
        base.OnUnload();

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(vertexArrayHandle);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.DeleteBuffer(indexBufferHandle);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferHandle);

        GL.UseProgram(0);
        GL.DeleteProgram(shaderProgramHandle);
    }

    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs args) {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.UseProgram(shaderProgramHandle);
        GL.BindVertexArray(vertexArrayHandle);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferHandle);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        this.SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        base.OnUpdateFrame(args);

        var input = this.KeyboardState;

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E))
            zoomLevel *= Math.Pow(2, args.Time);
        else if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q))
            zoomLevel *= Math.Pow(0.5, args.Time);

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            centerY += args.Time / zoomLevel;
        else if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            centerY -= args.Time / zoomLevel;
        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            centerX -= args.Time / zoomLevel;
        else if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            centerX += args.Time / zoomLevel;

        if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.R)) 
            zoomLevel = 1f;

        GL.Uniform1(zoomUniformLocation, zoomLevel);
        GL.Uniform2(centerUniformLocation, centerX, centerY);

        this.Title = $"{_windowTitle} | FPS: {1 / args.Time:F0}";
    }
}

