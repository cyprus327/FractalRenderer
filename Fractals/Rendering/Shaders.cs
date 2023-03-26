namespace Fractals.Rendering;

internal static class Shaders {
    private static readonly string _coloringCode = @"
        if (iter == MaxIter) {
            fragColor = vec4(0.0, 0.0, 0.0, 1.0);
            return;
        }

        float log_zn = log(float(dot(z, z)));
        float nu = log(log_zn / log(2.0)) / log(2.0);
        float t = float(iter) + 1.0 - nu;
        fragColor = vec4(
            0.5 + 0.5 * cos(3.0 + t * 0.2),
            0.5 + 0.5 * cos(1.0 + t * 0.3),
            0.5 + 0.5 * cos(5.0 + t * 0.4),
            1.0
        );
    ";

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
            while (iter < MaxIter && dot(z, z) < 4.0) {
                z = dvec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + uv;
                iter++;
            }
    
            " + _coloringCode + @"
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
    
            " + _coloringCode + @"
        }
    ";

    public static readonly string BurningShipFragCode = @"
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
            while (iter < MaxIter && dot(z, z) < 4.0) {
                z = dvec2(abs(z.x * z.x) - abs(z.y * z.y), -abs(2.0 * z.x * z.y)) + uv;
                iter++;
            }
    
            " + _coloringCode + @"
        }
    ";

    public static readonly string MultibrotFragCode = @"
        #version 410

        uniform double Zoom;
        uniform dvec2 Center;
        uniform int MaxIter;
        uniform double Power;        

        in vec2 Resolution;

        out vec4 fragColor;

        void main() {
            dvec2 uv = (gl_FragCoord.xy - dvec2(Resolution.x / 2.0, Resolution.y / 2.0)) / Resolution.y / Zoom + Center;
            dvec2 z = dvec2(0.0, 0.0);
    
            int iter = 0;
            while (iter < MaxIter && dot(z, z) < 4.0) { // maybe replace dot with z.x < 2.0 && z.y < 2.0
                double r = pow(float(dot(z, z)), float(Power / 2.0));
                double theta = atan(float(z.y), float(z.x)) * Power;
                z = dvec2(r * cos(float(theta)), r * sin(float(theta))) + uv;
                iter++;
            }
    
            " + _coloringCode + @"
        }
    ";

    public static readonly string MandelbulbFragCode = @"
        #version 410

        uniform float Zoom;
        uniform int MaxIter;
        uniform vec3 Rotation;

        in vec2 Resolution;

        #define RAY_LENGTH 4.0
        #define MIN_DIST 0.0001
        #define POWER 8.0
        #define LIGHT_POS vec3(0.0, 0.0, -4.0)
        #define AMBIENT_COLOR vec3(0.1, 0.1, 0.1)
        #define DIFFUSE_COLOR vec3(0.5, 0.5, 0.5)
        #define SPECULAR_COLOR vec3(1.0, 1.0, 1.0)
        #define SPECULAR_EXPONENT 64.0

        out vec4 fragColor;

        mat3 rotateX(float angle) {
            float c = cos(angle);
            float s = sin(angle);
            return mat3(
                1, 0, 0,
                0, c, s,
                0, -s, c
            );
        }

        mat3 rotateY(float angle) {
            float c = cos(angle);
            float s = sin(angle);
            return mat3(
                c, 0, -s,
                0, 1, 0,
                s, 0, c
            );
        }

        mat3 rotateZ(float angle) {
            float c = cos(angle);
            float s = sin(angle);
            return mat3(
                c, s, 0,
                -s, c, 0,
                0, 0, 1
            );
        }

        float DE(vec3 p) {
            vec3 z = p / Zoom;
            float dr = 1.0;
            float r = 0.0;
            for (int i = 0; i < 10; i++) {
                r = length(z);
                if (r > RAY_LENGTH) break;
                float theta = acos(z.z / r);
                float phi = atan(z.y, z.x);
                dr = pow(r, POWER - 1.0) * POWER * dr + 1.0;
                float zr = pow(r, POWER);
                theta = theta * POWER;
                phi = phi * POWER;
                vec3 dz = vec3(
                    sin(theta) * cos(phi),
                    sin(phi) * sin(theta),
                    cos(theta)
                );
                dz = rotateX(Rotation.x) * dz;
                dz = rotateY(Rotation.y) * dz;
                dz = rotateZ(Rotation.z) * dz;
                z = zr * dz + p / Zoom;
                z = rotateX(Rotation.x) * z;
                z = rotateY(Rotation.y) * z;
                z = rotateZ(Rotation.z) * z;
            }

            return 0.5 * log(r) * r / dr * Zoom;
        }

        vec3 calcNormal(vec3 p) {
            vec3 eps = vec3(0.001, 0.0, 0.0);
            float d = DE(p) * Zoom;
            float dx = DE(p + eps.xyy) - d;
            float dy = DE(p + eps.yxy) - d;
            float dz = DE(p + eps.yyx) - d;
            return normalize(vec3(dx, dy, dz));
        }

        void main() {
            vec2 uv = (gl_FragCoord.xy / Resolution.xy) * 2.0 - 1.0;
            uv.x *= Resolution.x / Resolution.y;
            mat3 rotX = mat3(
                vec3(1.0, 0.0, 0.0),
                vec3(0.0, cos(Rotation.x), -sin(Rotation.x)),
                vec3(0.0, sin(Rotation.x), cos(Rotation.x))
            );
            mat3 rotY = mat3(
                vec3(cos(Rotation.y), 0.0, sin(Rotation.y)),
                vec3(0.0, 1.0, 0.0),
                vec3(-sin(Rotation.y), 0.0, cos(Rotation.y))
            );
            mat3 rotZ = mat3(
                vec3(cos(Rotation.z), -sin(Rotation.z), 0.0),
                vec3(sin(Rotation.z), cos(Rotation.z), 0.0),
                vec3(0.0, 0.0, 1.0)
            );
            mat3 rotationMatrix = rotX * rotY * rotZ;

            vec3 rayOrigin = vec3(0.0, 0.0, -3.0) * rotationMatrix; // Apply rotation to camera position
            vec3 rayDir = normalize(vec3(uv, 1.0) * rotationMatrix); // Apply rotation to ray direction

            vec3 color = vec3(0.0);
            float t = 0.0;
            for (int i = 0; i < MaxIter; i++) {
                vec3 p = rayOrigin + t * rayDir;
                float d = DE(p);
                if (d < MIN_DIST) {
                    vec3 n = calcNormal(p);
                    vec3 lightDir = normalize(LIGHT_POS - p);
                    float diffuse = max(dot(n, lightDir), 0.0);
                    vec3 reflectedDir = reflect(-lightDir, n);
                    vec3 viewDir = normalize(-p);
                    float specular = pow(max(dot(reflectedDir, viewDir), 0.0), SPECULAR_EXPONENT);
                    color = AMBIENT_COLOR + diffuse * DIFFUSE_COLOR + specular * SPECULAR_COLOR;
                    break;
                }
                t += d;
                if (t > RAY_LENGTH) break;
            }

            fragColor = vec4(color, 1.0);
        }
    ";
}