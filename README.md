# Fractal Renderer

A fractal explorer.

## Features:

- Realtime rendering.
- All fractal's states are saved (i.e. your zoom and position is persistent when switching back and forth between fractals).
- 64 bit precision for very long zooms (except for the Mandelbulb).
- GPU instead of the CPU for extremely fast and responsive rendering.

## Current supported fractals:

- The Mandelbrot Set
- The Julia Set
- Burning Ship
- The Multibrot
- The Mandelbulb

## Screenshots:

- Mandelbrot

![Mandelbrot Base](https://github.com/cyprus327/FractalRenderer/assets/76965606/f1670f96-0761-4c3a-a1a1-74a1d05a9861)
![Mandelbrot Zoom 1](https://github.com/cyprus327/FractalRenderer/assets/76965606/af06e80f-3e80-476b-9913-2e789745d57d)
![Mandelbrot Zoom 2](https://github.com/cyprus327/FractalRenderer/assets/76965606/a03ef7ac-c51e-4cfe-b7c6-28e4c9ebe4e6)

- Julia Set

Coords: (-0.7800, 0.1360i)
![Julia Base](https://github.com/cyprus327/FractalRenderer/assets/76965606/4c1cec2a-6ad6-47d6-9822-413c6501ed62)
Coords: (0.1066, 0.6483i)
![Julia Changed](https://github.com/cyprus327/FractalRenderer/assets/76965606/3349e63e-7c37-41d7-8520-a39f65fa7c09)

- Burning Ship

![Burning Ship](https://github.com/cyprus327/FractalRenderer/assets/76965606/9a612b55-cd6a-49b9-9424-0989ae174b7f)

- Multibrot

Power: 0.5
![Multibrot 0.5](https://github.com/cyprus327/FractalRenderer/assets/76965606/35ddc74e-63a6-4e2a-8797-7492d4e26b23)
Power: ~3.0
![Multibrot 3.0](https://github.com/cyprus327/FractalRenderer/assets/76965606/d76339ea-84a3-43fd-81bd-4ac38078bb0e)
Power: ~7.0
![Multibrot 7.0](https://github.com/cyprus327/FractalRenderer/assets/76965606/43984f58-a381-45d1-a003-773b30fe8067)

- Mandelbulb (see [Raymarching](https://github.com/cyprus327/Raymarching) for a much better render)

![Mandelbulb Base](https://github.com/cyprus327/FractalRenderer/assets/76965606/9087c8b0-1f75-4327-8f12-d7473e65a881)
![Mandelbulb Surface](https://github.com/cyprus327/FractalRenderer/assets/76965606/303e4f68-8832-4f8f-b6f4-eb56ca547fc2)

- What happens when you reach maximum supported precision

![Precision Limit](https://github.com/cyprus327/FractalRenderer/assets/76965606/00c72bc3-9eaf-4a02-bb8d-33737289c858)

## Installation:

1. Clone this repository or download the .zip file.
2. Open the .sln file in Visual Studio 2022.
3. In release mode, build or run the solution.
4. If building the solution instead of running, find the .exe file generated and run it.

## Usage:

### Information Format

name of fractal | iteration count, coordinates, zoom level | fps

![Info Example](https://github.com/cyprus327/FractalRenderer/assets/76965606/3851bc68-6b26-47cc-88b6-7287976060a9)

### For all other than Mandelbulb:
- Left click and move the mouse to pan
- Q to zoom out
- E to zoom in
- R to reset the zoom
- Z to decrease max iterations
- X to increase max iterations
### For Mandelbulb:
- WASD or the mouse to rotate
- Q to zoom out
- E to zoom in
### Additional keybinds for Julia
- C to decrease the real constant
- V to increase the real constant
- B to decrease the imaginary constant
- N to increase the imaginary constant
### Additional keybinds for Multibrot
- C to decrease the power
- V to increase the power

## License

This project is licensed under the MIT License.
