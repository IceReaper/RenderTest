namespace RenderTest.Renderer
{
	public class ModelBufferMapping
	{
		public int VertexOffset;
		public readonly int VertexAmount;
		public int IndexOffset;
		public readonly int IndexAmount;

		public ModelBufferMapping(int vertexOffset, int vertexAmount, int indexOffset, int indexAmount)
		{
			this.VertexOffset = vertexOffset;
			this.VertexAmount = vertexAmount;
			this.IndexOffset = indexOffset;
			this.IndexAmount = indexAmount;
		}
	}
}
