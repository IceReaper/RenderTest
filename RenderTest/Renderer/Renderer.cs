namespace RenderTest.Renderer
{
	using Buffers;
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public abstract class Renderer : IDisposable
	{
		protected readonly Shader Shader;
		private readonly Buffer<VertexBufferData> vertexBuffer;
		private readonly Buffer<IndexBufferData> indexBuffer;
		private readonly Dictionary<Model, (VertexBufferData VertexBufferData, IndexBufferData IndexBufferData)> mappings = new();

		private bool bound;

		protected abstract int VertexStride { get; }
		public abstract int InstanceStride { get; }

		protected Renderer(string vertexShader, string fragmentShader)
		{
			this.Shader = new(vertexShader, fragmentShader);
			this.vertexBuffer = new();
			this.indexBuffer = new();
		}

		public abstract void LayoutVertexAttributes();
		public abstract void LayoutInstanceAttributes();

		public void Add(Model model, byte[] newVertices, int[] newIndices)
		{
			if (this.mappings.ContainsKey(model))
				return;

			var vertexBufferData = new VertexBufferData(newVertices);
			var vertexOffset = this.vertexBuffer.Add(vertexBufferData);
			var indexBufferData = new IndexBufferData(newIndices, vertexOffset / this.VertexStride);
			this.indexBuffer.Add(indexBufferData);

			this.mappings.Add(model, new(vertexBufferData, indexBufferData));
		}

		public void Remove(Model model)
		{
			// TODO When removing a model, we need to ensure there is no ModelInstance in any scene using it!
			// TODO The index offset will be changed, so we need to update the indirect buffers too!
			if (!this.mappings.TryGetValue(model, out var mapping))
				return;

			this.mappings.Remove(model);
			this.vertexBuffer.Remove(mapping.VertexBufferData);
			this.indexBuffer.Remove(mapping.IndexBufferData);

			var removedVertices = mapping.IndexBufferData.Data.Length / this.VertexStride;

			foreach (var indexBufferData in this.mappings.Values.Select(value => value.IndexBufferData)
				.Where(indexBufferData => indexBufferData.Shift > mapping.IndexBufferData.Shift))
			{
				indexBufferData.Shift -= removedVertices;
				this.indexBuffer.Update(indexBufferData);
			}
		}

		public int GetIndex(Model model)
		{
			if (!this.mappings.TryGetValue(model, out var mapping))
				return -1;

			return mapping.IndexBufferData.Shift;
		}

		public void BindBuffers()
		{
			this.vertexBuffer.Bind(BufferTarget.ArrayBuffer);
			this.indexBuffer.Bind(BufferTarget.ElementArrayBuffer);
			this.bound = true;
		}

		public void UnbindBuffers()
		{
			this.vertexBuffer.Unbind(BufferTarget.ArrayBuffer);
			this.indexBuffer.Unbind(BufferTarget.ElementArrayBuffer);
			this.bound = false;
		}


		public void Dispose()
		{
			if (this.bound)
				this.UnbindBuffers();

			this.Shader.Dispose();
			this.vertexBuffer.Dispose();
			this.indexBuffer.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
