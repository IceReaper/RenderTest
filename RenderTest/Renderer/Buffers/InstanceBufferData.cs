namespace RenderTest.Renderer.Buffers
{
	public class InstanceBufferData : IBufferData
	{
		public byte[] Data { get; set; }

		public InstanceBufferData(byte[] data)
		{
			this.Data = data;
		}
	}
}
