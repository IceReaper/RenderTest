namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ModelInstanceBuffer : IDisposable
	{
		private const int AllocationSize = 16 << 10 << 10; // MB;

		private readonly int stride;
		private readonly int buffer;
		private readonly Dictionary<ModelInstance, int> modelInstanceOffsets = new();

		private int allocated;
		private int used;

		public int Entries => this.used / this.stride;

		public ModelInstanceBuffer(int stride)
		{
			this.stride = stride;

			this.buffer = GL.GenBuffer();
		}

		public void Add(ModelInstance modelInstance)
		{
			if (this.modelInstanceOffsets.ContainsKey(modelInstance))
				return;

			if (this.used + this.stride > this.allocated)
			{
				this.ResizeBuffer(
					this.allocated
					+ (this.stride + ModelInstanceBuffer.AllocationSize - 1) / ModelInstanceBuffer.AllocationSize * ModelInstanceBuffer.AllocationSize
				);
			}

			this.modelInstanceOffsets.Add(modelInstance, this.used);
			this.used += this.stride;
		}

		public void Remove(ModelInstance modelInstance)
		{
			if (!this.modelInstanceOffsets.TryGetValue(modelInstance, out var offset))
				return;

			this.modelInstanceOffsets.Remove(modelInstance);
			this.used -= this.stride;

			foreach (var other in this.modelInstanceOffsets.Where(e => e.Value > offset).Select(e => e.Key).ToArray())
				this.modelInstanceOffsets[other] -= this.stride;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.buffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.buffer);

			GL.CopyBufferSubData(
				BufferTarget.CopyReadBuffer,
				BufferTarget.CopyWriteBuffer,
				new(offset + this.stride),
				new(offset),
				this.used - offset - this.stride
			);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			if (this.allocated - this.used >= ModelInstanceBuffer.AllocationSize)
				this.ResizeBuffer((this.allocated - this.used) / ModelInstanceBuffer.AllocationSize * ModelInstanceBuffer.AllocationSize);
		}

		private void ResizeBuffer(int size)
		{
			var temp = GL.GenBuffer();

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.buffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, temp);

			GL.BufferData(BufferTarget.CopyWriteBuffer, this.used, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.used);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, temp);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.buffer);

			GL.BufferData(BufferTarget.CopyWriteBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.used);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			GL.DeleteBuffer(temp);

			this.allocated = size;
		}

		public void SetData(ModelInstance modelInstance, float[] data)
		{
			if (!this.modelInstanceOffsets.TryGetValue(modelInstance, out var offset))
				return;

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.buffer);
			GL.BufferSubData(BufferTarget.CopyWriteBuffer, new(offset), this.stride, data);

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
		}

		public void Bind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.buffer);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(this.buffer);

			GC.SuppressFinalize(this);
		}
	}
}
