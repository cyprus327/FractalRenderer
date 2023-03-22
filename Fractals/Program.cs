using Fractals.Rendering;

namespace Fractals;

internal sealed class Program {
    private static void Main() {
        using var r = new Renderer(800, 600, "Mandelbrot");
        r.Run();
    }
}
