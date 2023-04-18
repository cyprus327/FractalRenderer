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