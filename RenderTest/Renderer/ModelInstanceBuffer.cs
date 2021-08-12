namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;

	public class ModelInstanceBuffer : IDisposable
	{
		private readonly int stride;

		private readonly int attributeBufferObject;
		private readonly int attributeBufferObjectSwap;

		private readonly Dictionary<ModelInstance, int> modelInstanceOffsets = new();

		public int Instances { get; private set; }

		public ModelInstanceBuffer(int stride)
		{
			this.stride = stride;

			this.attributeBufferObject = GL.GenBuffer();
			this.attributeBufferObjectSwap = GL.GenBuffer();
		}

		public void Add(ModelInstance modelInstance)
		{
			if (this.modelInstanceOffsets.ContainsKey(modelInstance))
				return;

			var oldSize = this.Instances * this.stride;
			var newSize = oldSize + this.stride;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.attributeBufferObject);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.attributeBufferObjectSwap);

			GL.BufferData(BufferTarget.CopyWriteBuffer, oldSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), oldSize);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.attributeBufferObjectSwap);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.attributeBufferObject);

			GL.BufferData(BufferTarget.CopyWriteBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), oldSize);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			this.modelInstanceOffsets.Add(modelInstance, oldSize);

			this.Instances++;
		}

		public void Remove(ModelInstance modelInstance)
		{
			if (!this.modelInstanceOffsets.TryGetValue(modelInstance, out var offset))
				return;

			var oldSize = this.Instances * this.stride;
			var newSize = oldSize - this.stride;

			var vAfterStart = offset + this.stride;
			var vAfterLength = oldSize - vAfterStart;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.attributeBufferObject);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.attributeBufferObjectSwap);

			GL.BufferData(BufferTarget.CopyWriteBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			if (offset > 0)
				GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), offset);

			if (vAfterLength > 0)
				GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(vAfterStart), new(offset), vAfterLength);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.attributeBufferObjectSwap);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.attributeBufferObject);

			GL.BufferData(BufferTarget.CopyWriteBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), newSize);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			this.modelInstanceOffsets.Remove(modelInstance);

			this.Instances--;
		}

		public void SetData(ModelInstance modelInstance, float[] data)
		{
			if (!this.modelInstanceOffsets.TryGetValue(modelInstance, out var offset))
				return;

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.attributeBufferObject);
			GL.BufferSubData(BufferTarget.CopyWriteBuffer, new(offset), this.stride, data);

			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
		}

		public void Bind()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.attributeBufferObject);
		}

		public void Dispose()
		{
			GL.DeleteBuffer(this.attributeBufferObjectSwap);
			GL.DeleteBuffer(this.attributeBufferObject);

			GC.SuppressFinalize(this);
		}
	}
}
