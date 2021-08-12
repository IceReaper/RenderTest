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

		protected abstract float[] GetData();

		protected void UpdateData()
		{
			this.scene.SetData(this, this.GetData());
		}

		public void Dispose()
		{
			this.scene.Remove(this);

			GC.SuppressFinalize(this);
		}
	}
}
