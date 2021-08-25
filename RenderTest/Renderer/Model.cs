namespace RenderTest.Renderer
{
	using System;

	public class Model : IDisposable
	{
		private readonly Renderer renderer;
		public readonly int Indices;

		public Model(Renderer renderer, byte[] vertices, int[] indices)
		{
			this.renderer = renderer;
			this.Indices = indices.Length;

			this.renderer.Add(this, vertices, indices);
		}

		public void Dispose()
		{
			this.renderer.Remove(this);

			GC.SuppressFinalize(this);
		}
	}
}
