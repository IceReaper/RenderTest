namespace RenderTest.Renderer
{
	using System;
	using System.Collections.Generic;

	public class Scene : IDisposable
	{
		public readonly Dictionary<Model, ModelInstanceBuffer> ModelInstanceBuffers = new();

		private readonly int stride;

		public Scene(Renderer renderer)
		{
			this.stride = renderer.InstanceStride;
		}

		public void Add(ModelInstance modelInstance)
		{
			if (!this.ModelInstanceBuffers.TryGetValue(modelInstance.Model, out var modelInstanceBuffer))
				this.ModelInstanceBuffers.Add(modelInstance.Model, modelInstanceBuffer = new(this.stride));

			modelInstanceBuffer.Add(modelInstance);
		}

		public void Remove(ModelInstance modelInstance)
		{
			if (this.ModelInstanceBuffers.TryGetValue(modelInstance.Model, out var modelInstanceBuffer))
				modelInstanceBuffer.Remove(modelInstance);
		}

		public void SetData(ModelInstance modelInstance, float[] data)
		{
			if (!this.ModelInstanceBuffers.TryGetValue(modelInstance.Model, out var modelInstanceBuffer))
				return;

			modelInstanceBuffer.SetData(modelInstance, data);
		}

		public void Dispose()
		{
			foreach (var modelInstanceBuffer in this.ModelInstanceBuffers.Values)
				modelInstanceBuffer.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
