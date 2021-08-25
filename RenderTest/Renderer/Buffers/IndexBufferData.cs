namespace RenderTest.Renderer.Buffers
{
	using System;
	using System.Linq;

	public class IndexBufferData : IBufferData
	{
		private readonly int[] indices;
		public int Shift;

		public byte[] Data => this.indices.SelectMany(index => BitConverter.GetBytes(index + this.Shift)).ToArray();

		public IndexBufferData(int[] indices, int shift)
		{
			this.indices = indices;
			this.Shift = shift;
		}
	}
}
