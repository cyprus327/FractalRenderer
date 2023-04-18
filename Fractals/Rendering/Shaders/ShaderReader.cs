namespace Fractals.Rendering.Shaders;

internal static class ShaderReader {
    public static string ReadToString(string filepath) {
        string shaderSource = string.Empty;

        try {
            using var sr = new StreamReader(filepath);
            shaderSource = sr.ReadToEnd();
        }
        catch (IOException ex) {
            Console.WriteLine($"Failed to read shader:\n {ex}");
        }

        return shaderSource;
    }
}