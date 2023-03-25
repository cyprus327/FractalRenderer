namespace Fractals.Rendering;

internal static class Shaders {
    public static readonly string VertexCode = @"
        #version 410
        
        uniform vec2 ViewportSize;

        layout (location = 0) in vec2 aPosition;

        out vec2 Resolution;

        void main() {
            Resolution = ViewportSize;
            float nx = aPosition.x / ViewportSize.x * 2 - 1;
            float ny = aPosition.y / ViewportSize.y * 2 - 1;
            gl_Position = vec4(nx, ny, 0, 1);
        }
    ";

    public static readonly string MandelbrotFragCode = @"
        #version 410

        uniform double Zoom;
        uniform dvec2 Center;
        uniform int MaxIter;
        
        in vec2 Resolution;

        out vec4 fragColor;

        void main() {
            dvec2 uv = (gl_FragCoord.xy - dvec2(Resolution.x / 2.0, Resolution.y / 2.0)) / Resolution.y / Zoom + Center;
    
            dvec2 z = dvec2(0.0, 0.0);
    
            int iter = 0;
            while (iter < MaxIter && dot(z, z) < 4.0) { // maybe replace dot with z.x < 2.0 && z.y < 2.0
                z = dvec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + uv;
                iter++;
            }
    
            float t = float(iter) / float(MaxIter);
            fragColor = vec4(
                8.0 * (1.0 - t) * t * t * t, 
                15.0 * (1.0 - t) * (1.0 - t) * t * t,
                9.5 * (1.0 - t) * (1.0 - t) * (1.0 - t) * t,
                1.0
            );
        }
    ";

    public static readonly string JuliaFragCode = @"
        #version 410

        uniform double Zoom;
        uniform dvec2 Center;
        uniform dvec2 Constant;
        uniform int MaxIter;

        in vec2 Resolution;

        out vec4 fragColor;

        void main() {
            dvec2 z = (gl_FragCoord.xy - dvec2(Resolution.x / 2.0, Resolution.y / 2.0)) / Resolution.y / Zoom + Center;
    
            int iter = 0;
            while (iter < MaxIter && dot(z, z) < 4.0) {
                z = dvec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + Constant;
                iter++;
            }
    
            float t = float(iter) / float(MaxIter);
            fragColor = vec4(
                8.0 * (1.0 - t) * t * t * t, 
                15.0 * (1.0 - t) * (1.0 - t) * t * t,
                9.5 * (1.0 - t) * (1.0 - t) * (1.0 - t) * t,
                1.0
            );
        }
    ";
}