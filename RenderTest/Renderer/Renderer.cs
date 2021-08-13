namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;

	public abstract class Renderer : IDisposable
	{
		protected readonly Shader Shader;
		private readonly ModelBuffer modelBuffer;

		public abstract int InstanceStride { get; }

		protected Renderer(string vertexShader, string fragmentShader, int vertexStride)
		{
			this.Shader = new(vertexShader, fragmentShader);
			this.modelBuffer = new(this.LayoutAttributes, vertexStride);
		}

		protected abstract void LayoutAttributes();

		protected abstract void LayoutInstancedAttributes();

		public void Add(Model model, float[][] newVertices, uint[] newIndices)
		{
			this.modelBuffer.Add(model, newVertices, newIndices);
		}

		public void Remove(Model model)
		{
			this.modelBuffer.Remove(model);
		}

		protected void Render(Scene scene)
		{
			this.modelBuffer.Bind();

			// TODO find out if we can somehow buffer the loop into a single call.
			foreach (var (model, modelInstanceBuffer) in scene.ModelInstanceBuffers)
			{
				modelInstanceBuffer.Bind();
				this.LayoutInstancedAttributes();
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

				var bufferMapping = this.modelBuffer.ModelBufferMappings[model];

				GL.DrawElementsInstanced(
					PrimitiveType.Triangles,
					bufferMapping.IndexAmount,
					DrawElementsType.UnsignedInt,
					new(bufferMapping.IndexOffset),
					modelInstanceBuffer.Entries
				);
			}

			GL.BindVertexArray(0);
		}

		public void Dispose()
		{
			this.Shader.Dispose();
			this.modelBuffer.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
