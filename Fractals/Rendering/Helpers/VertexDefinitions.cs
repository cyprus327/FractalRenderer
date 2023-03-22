using OpenTK.Mathematics;

namespace Fractals.Rendering.Helpers;

public readonly struct VertexAttrib
{
    public VertexAttrib(string name, int index, int componentCount, int offset)
    {
        Name = name;
        Index = index;
        ComponentCount = componentCount;
        Offset = offset;
    }

    public readonly string Name;
    public readonly int Index;
    public readonly int ComponentCount;
    public readonly int Offset;
}

public sealed class VertexInfo
{
    public VertexInfo(Type type, params VertexAttrib[] attributes)
    {
        Type = type;
        SizeInBytes = 0;
        VertexAttributes = attributes;

        for (int i = 0; i < VertexAttributes.Length; i++)
        {
            SizeInBytes += VertexAttributes[i].ComponentCount * sizeof(float);
        }
    }

    public readonly Type Type;
    public readonly int SizeInBytes;
    public readonly VertexAttrib[] VertexAttributes;
}

public readonly struct VertexPosCol
{
    public VertexPosCol(Vector2 pos, Color4 col)
    {
        Position = pos;
        Color = col;
    }

    public readonly Vector2 Position;
    public readonly Color4 Color;

    public static readonly VertexInfo Info = new VertexInfo(typeof(VertexPosCol),
        new VertexAttrib("Position", 0, 2, 0),
        new VertexAttrib("Color", 1, 4, 2 * sizeof(float))
    );
}

public readonly struct VertexPosTex
{
    public VertexPosTex(Vector2 pos, Vector2 tex)
    {
        Position = pos;
        TexCoord = tex;
    }

    public readonly Vector2 Position;
    public readonly Vector2 TexCoord;

    public static readonly VertexInfo Info = new VertexInfo(typeof(VertexPosTex),
        new VertexAttrib("Position", 0, 2, 0),
        new VertexAttrib("TexCoord", 1, 2, 2 * sizeof(float))
    );
}