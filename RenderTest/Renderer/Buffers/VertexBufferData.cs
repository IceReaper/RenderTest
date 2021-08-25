namespace RenderTest.Renderer.Buffers
{
	public class VertexBufferData : IBufferData
	{
		public byte[] Data { get; }

		public VertexBufferData(byte[] data)
		{
			this.Data = data;
		}
	}
}
