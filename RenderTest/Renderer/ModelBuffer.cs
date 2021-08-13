namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ModelBuffer : IDisposable
	{
		private const int AllocationSize = 16 << 10 << 10; // MB;

		public readonly Dictionary<Model, ModelBufferMapping> ModelBufferMappings = new();

		private readonly int stride;

		private readonly int vertexArrayObject;
		private readonly int vertexBuffer;
		private readonly int indexBuffer;

		private int vertexAllocated;
		private int vertexUsed;
		private int vertexAmount;

		private int indexAllocated;
		private int indexUsed;
		private int indexAmount;

		public ModelBuffer(Action layoutAttributes, int stride)
		{
			this.stride = stride;

			this.vertexArrayObject = GL.GenVertexArray();
			this.vertexBuffer = GL.GenBuffer();
			this.indexBuffer = GL.GenBuffer();

			GL.BindVertexArray(this.vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer);

			layoutAttributes();

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		public void Add(Model model, float[][] newVertices, uint[] newIndices)
		{
			if (this.ModelBufferMappings.ContainsKey(model))
				return;

			var vAddSize = newVertices.Length * this.stride;
			var iAddSize = newIndices.Length * sizeof(uint);

			if (this.vertexUsed + vAddSize > this.vertexAllocated)
			{
				this.ResizeVertexBuffer(
					this.indexAllocated + (vAddSize + ModelBuffer.AllocationSize - 1) / ModelBuffer.AllocationSize * ModelBuffer.AllocationSize
				);
			}

			if (this.indexUsed + vAddSize > this.indexAllocated)
			{
				this.ResizeIndexBuffer(
					this.indexAllocated + (vAddSize + ModelBuffer.AllocationSize - 1) / ModelBuffer.AllocationSize * ModelBuffer.AllocationSize
				);
			}

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBuffer);
			GL.BufferSubData(BufferTarget.CopyWriteBuffer, new(this.vertexUsed), vAddSize, newVertices.SelectMany(value => value).ToArray());

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBuffer);

			GL.BufferSubData(
				BufferTarget.CopyWriteBuffer,
				new(this.indexUsed),
				iAddSize,
				newIndices.Select(value => (uint)(value + this.vertexAmount)).ToArray()
			);

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			this.ModelBufferMappings.Add(model, new(this.vertexUsed, newVertices.Length, this.indexUsed, newIndices.Length));

			this.vertexUsed += vAddSize;
			this.indexUsed += iAddSize;
			this.vertexAmount += newVertices.Length;
			this.indexAmount += newIndices.Length;
		}

		public void Remove(Model model)
		{
			if (!this.ModelBufferMappings.TryGetValue(model, out var bufferMapping))
				return;

			this.ModelBufferMappings.Remove(model);

			this.vertexUsed -= bufferMapping.VertexAmount * this.stride;
			this.indexUsed -= bufferMapping.IndexAmount * sizeof(uint);

			this.vertexAmount -= bufferMapping.VertexAmount;
			this.indexAmount -= bufferMapping.IndexAmount;

			foreach (var other in this.ModelBufferMappings.Values.Where(e => e.VertexOffset > bufferMapping.VertexOffset))
				other.VertexOffset -= bufferMapping.VertexAmount * this.stride;

			foreach (var other in this.ModelBufferMappings.Values.Where(e => e.IndexOffset > bufferMapping.VertexOffset))
				other.IndexOffset -= bufferMapping.IndexAmount * sizeof(uint);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.vertexBuffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBuffer);

			GL.CopyBufferSubData(
				BufferTarget.CopyReadBuffer,
				BufferTarget.CopyWriteBuffer,
				new(bufferMapping.VertexOffset + bufferMapping.VertexAmount * this.stride),
				new(bufferMapping.VertexOffset),
				this.vertexUsed - bufferMapping.VertexOffset - bufferMapping.VertexAmount * this.stride
			);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.indexBuffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBuffer);

			var remapIndices = new uint[this.indexUsed - bufferMapping.IndexOffset - bufferMapping.IndexAmount];

			GL.GetBufferSubData(
				BufferTarget.CopyReadBuffer,
				new(bufferMapping.IndexOffset + bufferMapping.IndexAmount * sizeof(uint)),
				remapIndices.Length * sizeof(uint),
				remapIndices
			);

			GL.BufferSubData(
				BufferTarget.CopyWriteBuffer,
				new(bufferMapping.IndexOffset),
				remapIndices.Length * sizeof(uint),
				remapIndices.Select(index => index - bufferMapping.VertexAmount).ToArray()
			);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			if (this.vertexAllocated - this.vertexUsed >= ModelBuffer.AllocationSize)
				this.ResizeVertexBuffer((this.vertexAllocated - this.vertexUsed) / ModelBuffer.AllocationSize * ModelBuffer.AllocationSize);

			if (this.indexAllocated - this.indexUsed >= ModelBuffer.AllocationSize)
				this.ResizeIndexBuffer((this.indexAllocated - this.indexUsed) / ModelBuffer.AllocationSize * ModelBuffer.AllocationSize);

			this.ModelBufferMappings.Remove(model);
		}

		private void ResizeVertexBuffer(int size)
		{
			var temp = GL.GenBuffer();

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.vertexBuffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, temp);

			GL.BufferData(BufferTarget.CopyWriteBuffer, this.vertexUsed, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.vertexUsed);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, temp);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBuffer);

			GL.BufferData(BufferTarget.CopyWriteBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.vertexUsed);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			GL.DeleteBuffer(temp);

			this.vertexAllocated = size;
		}

		private void ResizeIndexBuffer(int size)
		{
			var temp = GL.GenBuffer();

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.indexBuffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, temp);

			GL.BufferData(BufferTarget.CopyWriteBuffer, this.indexUsed, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.indexUsed);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, temp);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBuffer);

			GL.BufferData(BufferTarget.CopyWriteBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.indexUsed);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			GL.DeleteBuffer(temp);

			this.indexAllocated = size;
		}

		public void Bind()
		{
			GL.BindVertexArray(this.vertexArrayObject);
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(this.vertexArrayObject);
			GL.DeleteBuffer(this.vertexBuffer);
			GL.DeleteBuffer(this.indexBuffer);

			GC.SuppressFinalize(this);
		}
	}
}
