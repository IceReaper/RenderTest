namespace RenderTest.Renderer.Buffers
{
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Buffer<TBufferData> : IDisposable
		where TBufferData : IBufferData
	{
		private const int AllocationSize = 16 << 10 << 10; // MB;

		private readonly int buffer;
		private readonly Dictionary<object, int> offsets = new();
		private readonly HashSet<BufferTarget> bound = new();

		private int allocated;
		private int used;

		public Buffer()
		{
			this.buffer = GL.GenBuffer();
		}

		public int Add(TBufferData bufferData)
		{
			if (this.offsets.ContainsKey(bufferData))
				return -1;

			if (this.used + bufferData.Data.Length > this.allocated)
			{
				this.ResizeBuffer(
					this.allocated
					+ (bufferData.Data.Length + Buffer<TBufferData>.AllocationSize - 1)
					/ Buffer<TBufferData>.AllocationSize
					* Buffer<TBufferData>.AllocationSize
				);
			}

			var offset = this.used;
			this.offsets.Add(bufferData, offset);
			this.used += bufferData.Data.Length;
			
			this.Update(bufferData);

			return offset;
		}

		public void Remove(TBufferData bufferData)
		{
			if (!this.offsets.TryGetValue(bufferData, out var offset))
				return;

			this.offsets.Remove(bufferData);
			this.used -= bufferData.Data.Length;

			foreach (var other in this.offsets.Where(entry => entry.Value > offset).Select(entry => entry.Key).ToArray())
				this.offsets[other] -= bufferData.Data.Length;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.buffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.buffer);

			GL.CopyBufferSubData(
				BufferTarget.CopyReadBuffer,
				BufferTarget.CopyWriteBuffer,
				new(offset + bufferData.Data.Length),
				new(offset),
				this.used - offset - bufferData.Data.Length
			);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			if (this.allocated - this.used >= Buffer<TBufferData>.AllocationSize)
				this.ResizeBuffer(this.allocated - (this.allocated - this.used) / Buffer<TBufferData>.AllocationSize * Buffer<TBufferData>.AllocationSize);
		}

		public void Update(TBufferData bufferData)
		{
			if (!this.offsets.TryGetValue(bufferData, out var offset))
				return;

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.buffer);
			GL.BufferSubData(BufferTarget.CopyWriteBuffer, new(offset), bufferData.Data.Length, bufferData.Data);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
		}

		public void Bind(BufferTarget bufferTarget)
		{
			GL.BindBuffer(bufferTarget, this.buffer);
			this.bound.Add(bufferTarget);
		}

		public void Unbind(BufferTarget bufferTarget)
		{
			GL.BindBuffer(bufferTarget, 0);
			this.bound.Remove(bufferTarget);
		}

		private void ResizeBuffer(int size)
		{
			var tempBuffer = GL.GenBuffer();

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.buffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, tempBuffer);

			GL.BufferData(BufferTarget.CopyWriteBuffer, this.used, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.used);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, tempBuffer);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.buffer);

			GL.BufferData(BufferTarget.CopyWriteBuffer, size, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), this.used);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			GL.DeleteBuffer(tempBuffer);
			
			this.allocated = size;
		}

		public void Dispose()
		{
			foreach (var bufferTarget in this.bound)
				this.Unbind(bufferTarget);

			GL.DeleteBuffer(this.buffer);

			GC.SuppressFinalize(this);
		}
	}
}
