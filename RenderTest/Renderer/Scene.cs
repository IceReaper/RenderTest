namespace RenderTest.Renderer
{
	using Buffers;
	using OpenTK.Graphics.OpenGL;
	using System;
	using System.Collections.Generic;

	public class Scene : IDisposable
	{
		private readonly Renderer renderer;
		private readonly Buffer<InstanceBufferData> instanceBuffer;
		private readonly Buffer<IndirectBufferData> indirectBuffer;
		private readonly Dictionary<ModelInstance, (InstanceBufferData InstanceBufferData, IndirectBufferData IndirectBufferData)> mappings = new();
		private readonly HashSet<ModelInstance> dirty = new();
		private readonly int vao;

		public Scene(Renderer renderer)
		{
			this.renderer = renderer;
			this.instanceBuffer = new();
			this.indirectBuffer = new();

			this.vao = GL.GenVertexArray();

			GL.BindVertexArray(this.vao);

			renderer.BindBuffers();
			renderer.LayoutVertexAttributes();

			this.instanceBuffer.Bind(BufferTarget.ArrayBuffer);
			renderer.LayoutInstanceAttributes();

			GL.BindVertexArray(0);

			renderer.UnbindBuffers();
			this.instanceBuffer.Unbind(BufferTarget.ArrayBuffer);
		}

		public void Add(ModelInstance modelInstance)
		{
			if (this.mappings.ContainsKey(modelInstance))
				return;

			var instanceBufferData = new InstanceBufferData(modelInstance.GetData());
			var instanceOffset = this.instanceBuffer.Add(instanceBufferData);

			var indirectBufferData = new IndirectBufferData(
				modelInstance.Model.Indices,
				1,
				this.renderer.GetIndex(modelInstance.Model), // TODO this value may change, see comments in Renderer!
				0, // TODO use index shift here to avoid the need to update index data in gpu!
				instanceOffset / this.renderer.InstanceStride // TODO this value may change when removing a ModelInstance!
			);

			this.indirectBuffer.Add(indirectBufferData);
			this.mappings.Add(modelInstance, new(instanceBufferData, indirectBufferData));
			this.Invalidate(modelInstance);
		}

		public void Remove(ModelInstance modelInstance)
		{
			// TODO we need to re-set the indirect buffer baseInstance values!
			if (!this.mappings.TryGetValue(modelInstance, out var mapping))
				return;

			this.dirty.Remove(modelInstance);
			this.mappings.Remove(modelInstance);
			this.instanceBuffer.Remove(mapping.InstanceBufferData);
			this.indirectBuffer.Remove(mapping.IndirectBufferData);
		}

		public void Invalidate(ModelInstance modelInstance)
		{
			if (!this.dirty.Contains(modelInstance))
				this.dirty.Add(modelInstance);
		}

		public void Render()
		{
			foreach (var modelInstance in this.dirty)
			{
				var instanceBufferData = this.mappings[modelInstance].InstanceBufferData;
				instanceBufferData.Data = modelInstance.GetData();
				this.instanceBuffer.Update(instanceBufferData);
			}

			this.dirty.Clear();

			this.indirectBuffer.Bind(BufferTarget.DrawIndirectBuffer);
			GL.BindVertexArray(this.vao);

			GL.MultiDrawElementsIndirect(PrimitiveType.Triangles, DrawElementsType.UnsignedInt, IntPtr.Zero, this.mappings.Count, 5 * sizeof(int));

			GL.BindVertexArray(0);
			this.indirectBuffer.Unbind(BufferTarget.DrawIndirectBuffer);
		}

		public void Dispose()
		{
			GL.DeleteVertexArray(this.vao);

			this.indirectBuffer.Dispose();
			this.instanceBuffer.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
