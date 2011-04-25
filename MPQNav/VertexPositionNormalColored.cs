using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MPQNav
{
    /// <summary>
    /// Struct outlining our custom vertex
    /// </summary>
    public struct VertexPositionNormalColored : IVertexType
    {
        /// <summary>
        /// Vector3 Position for this vertex
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Color of this vertex
        /// </summary>
        public Color Color;

        /// <summary>
        /// Normal vector for this Vertex
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Constructor for a VertexPositionNormalColored
        /// </summary>
        /// <param name="position">Vector3 Position of the vertex</param>
        /// <param name="color">Color of the vertex</param>
        /// <param name="normal">Normal vector of the vertex</param>
        public VertexPositionNormalColored(Vector3 position, Color color, Vector3 normal) {
            Position = position;
            Color = color;
            Normal = normal;
        }

        /// <summary>
        /// Memory size for a VertexPositionNormalColored
        /// </summary>
        public static int SizeInBytes = 7 * 4;

        private static readonly VertexDeclaration vertexDeclaration =
            new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                                  new VertexElement(sizeof (float)*3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                                  new VertexElement(sizeof (float)*4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0));

        public VertexDeclaration VertexDeclaration
        {
            get { return vertexDeclaration; }
        }
    }
}