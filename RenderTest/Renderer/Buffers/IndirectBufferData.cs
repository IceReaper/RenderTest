namespace RenderTest.Renderer.Buffers
{
	using System;
	using System.Linq;

	public class IndirectBufferData : IBufferData
	{
		public byte[] Data { get; }

		public IndirectBufferData(int count, int instanceCount, int firstIndex, int baseVertex, int baseInstance)
		{
			this.Data = new[] { count, instanceCount, firstIndex, baseVertex, baseInstance }.SelectMany(BitConverter.GetBytes).ToArray();
		}
	}
}
