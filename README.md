# Fractal Renderer

This is a fractal renderer made in C# with OpenTK.

## Current supported fractals:

- The Mandelbrot Set
- The Julia Set
- Burning Ship
- The Multibrots
- The Mandelbulb

## Features:

- Realtime rendering.
- Renders all of the above fractals (except for the Mandelbulb) with 64 bit precision.
- Supports zooming and panning.
- Uses the GPU instead of the CPU.

### Example video showing off a little of everything
https://streamable.com/738frg

### Example video of a Mandelbrot zoom and messing around with Julia constants
https://streamable.com/jaijdj

## Installation:

1. Clone this repository or download the .zip file.
2. Open the .sln file in Visual Studio 2022.
3. In release mode, build or run the solution.
4. If building the solution instead of running, find the .exe file generated and run it.

## Usage:

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
