using System.Runtime.InteropServices;
using Sandbox;

class Helpers{
    public static VertexAttribute[] MeshLayout { get; } =
    {
        new VertexAttribute(VertexAttributeType.Position, VertexAttributeFormat.Float32),
        new VertexAttribute(VertexAttributeType.Normal, VertexAttributeFormat.Float32),
        new VertexAttribute(VertexAttributeType.Tangent, VertexAttributeFormat.Float32),
        new VertexAttribute(VertexAttributeType.TexCoord, VertexAttributeFormat.Float32)
    };
}

[StructLayout( LayoutKind.Sequential )]
public readonly struct VoxelVertex
{
    public static VertexAttribute[] Layout { get; } =
    {
        new VertexAttribute(VertexAttributeType.Position, VertexAttributeFormat.Float32),
        new VertexAttribute(VertexAttributeType.Normal, VertexAttributeFormat.Float32),
        new VertexAttribute(VertexAttributeType.Tangent, VertexAttributeFormat.Float32),
        new VertexAttribute(VertexAttributeType.TexCoord, VertexAttributeFormat.Float32)
    };

    public readonly Vector3 Position;
    public readonly Vector3 Normal;
    public readonly Vector3 Tangent;
    public readonly Vector3 TexCoord;

    public VoxelVertex(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 texCoord)
    {
        Position = position;
        Normal = normal;
        Tangent = tangent;
        TexCoord = texCoord;
    }
}