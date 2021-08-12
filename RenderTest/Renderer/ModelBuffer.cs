namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ModelBuffer : IDisposable
	{
		public readonly Dictionary<Model, ModelBufferMapping> ModelBufferMappings = new();

		private readonly int stride;

		private readonly int vertexBufferObject;
		private readonly int vertexBufferObjectSwap;
		private readonly int indexBufferObject;
		private readonly int indexBufferObjectSwap;
		private readonly int vertexArrayObject;

		private int vertices;
		private int indices;

		public ModelBuffer(Action layoutAttributes, int stride)
		{
			this.stride = stride;

			this.vertexBufferObject = GL.GenBuffer();
			this.vertexBufferObjectSwap = GL.GenBuffer();
			this.indexBufferObject = GL.GenBuffer();
			this.indexBufferObjectSwap = GL.GenBuffer();

			this.vertexArrayObject = GL.GenVertexArray();

			GL.BindVertexArray(this.vertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferObject);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferObject);

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
			var vOldSize = this.vertices * this.stride;
			var vNewSize = vOldSize + vAddSize;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.vertexBufferObject);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBufferObjectSwap);

			GL.BufferData(BufferTarget.CopyWriteBuffer, vOldSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), vOldSize);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.vertexBufferObjectSwap);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBufferObject);

			GL.BufferData(BufferTarget.CopyWriteBuffer, vNewSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), vOldSize);
			GL.BufferSubData(BufferTarget.CopyWriteBuffer, new(vOldSize), vAddSize, newVertices.SelectMany(value => value).ToArray());

			var iAddSize = newIndices.Length * sizeof(uint);
			var iOldSize = this.indices * sizeof(uint);
			var iNewSize = iOldSize + iAddSize;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.indexBufferObject);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBufferObjectSwap);

			GL.BufferData(BufferTarget.CopyWriteBuffer, iOldSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), iOldSize);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.indexBufferObjectSwap);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBufferObject);

			GL.BufferData(BufferTarget.CopyWriteBuffer, iNewSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), iOldSize);
			GL.BufferSubData(BufferTarget.CopyWriteBuffer, new(iOldSize), iAddSize, newIndices.Select(value => (uint)(value + this.indices)).ToArray());

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			this.ModelBufferMappings.Add(model, new(vOldSize, newVertices.Length, iOldSize, newIndices.Length));

			this.vertices += newVertices.Length;
			this.indices += newIndices.Length;
		}

		public void Remove(Model model)
		{
			if (!this.ModelBufferMappings.TryGetValue(model, out var bufferMapping))
				return;

			var vDelSize = bufferMapping.VerticesAmount * this.stride;
			var vOldSize = this.vertices * this.stride;
			var vNewSize = vOldSize - vDelSize;
			var vDelStart = bufferMapping.VerticesOffset;
			var vDelEnd = vDelStart + vDelSize;
			var vRemaining = vOldSize - vDelEnd;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.vertexBufferObject);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBufferObjectSwap);

			GL.BufferData(BufferTarget.CopyWriteBuffer, vNewSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			if (vDelStart > 0)
				GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), vDelStart);

			if (vRemaining > 0)
				GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(vDelEnd), new(vDelStart), vRemaining);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.vertexBufferObjectSwap);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.vertexBufferObject);

			GL.BufferData(BufferTarget.CopyWriteBuffer, vNewSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), vNewSize);

			var iDelSize = bufferMapping.IndicesAmount * sizeof(uint);
			var iOldSize = this.indices * sizeof(uint);
			var iNewSize = iOldSize - iDelSize;
			var iDelStart = bufferMapping.IndicesOffset;
			var iDelEnd = iDelStart + iDelSize;
			var iRemaining = iOldSize - iDelEnd;

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.indexBufferObject);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBufferObjectSwap);

			GL.BufferData(BufferTarget.CopyWriteBuffer, iNewSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);

			if (iDelStart > 0)
				GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), iDelStart);

			if (iRemaining > 0)
			{
				var remapIndices = new uint[iRemaining / sizeof(uint)];

				GL.GetBufferSubData(BufferTarget.CopyReadBuffer, new(iDelEnd), iRemaining, remapIndices);

				GL.BufferSubData(
					BufferTarget.CopyWriteBuffer,
					new(iDelStart),
					iRemaining,
					remapIndices.Select(index => index - bufferMapping.IndicesAmount).ToArray()
				);
			}

			GL.BindBuffer(BufferTarget.CopyReadBuffer, this.indexBufferObjectSwap);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, this.indexBufferObject);

			GL.BufferData(BufferTarget.CopyWriteBuffer, iNewSize, IntPtr.Zero, BufferUsageHint.DynamicCopy);
			GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, new(0), new(0), iNewSize);

			GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
			GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);

			this.ModelBufferMappings.Remove(model);

			this.vertices -= bufferMapping.VerticesAmount;
			this.indices -= bufferMapping.IndicesAmount;
		}

		public void Bind()
		{
			GL.BindVertexArray(this.vertexArrayObject);
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(this.vertexArrayObject);
			GL.DeleteBuffer(this.vertexBufferObjectSwap);
			GL.DeleteBuffer(this.vertexBufferObject);
			GL.DeleteBuffer(this.indexBufferObjectSwap);
			GL.DeleteBuffer(this.indexBufferObject);

			GC.SuppressFinalize(this);
		}
	}
}
