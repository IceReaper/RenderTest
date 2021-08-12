namespace RenderTest.Renderer
{
	using System;

	public class Model : IDisposable
	{
		private readonly Renderer renderer;

		public Model(Renderer renderer, float[][] vertices, uint[] indices)
		{
			this.renderer = renderer;
			this.renderer.Add(this, vertices, indices);
		}

		public void Dispose()
		{
			this.renderer.Remove(this);

			GC.SuppressFinalize(this);
		}
	}
}
