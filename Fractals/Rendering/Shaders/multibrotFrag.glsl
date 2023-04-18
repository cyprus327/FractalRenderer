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
}