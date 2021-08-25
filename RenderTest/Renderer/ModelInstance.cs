namespace RenderTest.Renderer
{
	using System;

	public abstract class ModelInstance : IDisposable
	{
		public readonly Model Model;
		private readonly Scene scene;

		protected ModelInstance(Model model, Scene scene)
		{
			this.Model = model;
			this.scene = scene;

			this.scene.Add(this);
		}

		public abstract byte[] GetData();

		protected void Invalidate()
		{
			this.scene.Invalidate(this);
		}

		public void Dispose()
		{
			this.scene.Remove(this);

			GC.SuppressFinalize(this);
		}
	}
}
