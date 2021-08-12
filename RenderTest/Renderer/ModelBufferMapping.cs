namespace RenderTest.Renderer
{
	public readonly struct ModelBufferMapping
	{
		public readonly int VerticesOffset;
		public readonly int VerticesAmount;
		public readonly int IndicesOffset;
		public readonly int IndicesAmount;

		public ModelBufferMapping(int verticesOffset, int verticesAmount, int indicesOffset, int indicesAmount)
		{
			this.VerticesOffset = verticesOffset;
			this.VerticesAmount = verticesAmount;
			this.IndicesOffset = indicesOffset;
			this.IndicesAmount = indicesAmount;
		}
	}
}
