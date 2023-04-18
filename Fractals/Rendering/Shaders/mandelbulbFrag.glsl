#version 410
        
precision highp float;

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
    vec3 eps = vec3(0.000001, 0.0, 0.0);
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

    vec3 rayOrigin = vec3(0.0, 0.0, -3.0) * rotationMatrix;
    vec3 rayDir = normalize(vec3(uv, 1.0) * rotationMatrix);

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
            color = vec3(
                0.5 + 2.5 * cos(3.0 + float(i) * 0.2),
                0.5 + 2.5 * cos(1.0 + float(i) * 0.3),
                0.5 + 2.5 * cos(5.0 + float(i) * 0.4)
            );
            color = AMBIENT_COLOR + diffuse * color * DIFFUSE_COLOR + specular * SPECULAR_COLOR;
            break;
        }
        t += d;
        if (t > RAY_LENGTH) break;
    }

    fragColor = vec4(color, 1.0);
}