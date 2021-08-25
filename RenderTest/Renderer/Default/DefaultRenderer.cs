namespace RenderTest.Renderer.Default
{
	using Cameras;
	using OpenTK.Graphics.OpenGL;

	public class DefaultRenderer : Renderer
	{
		// language=glsl
		private const string VertexShader = @"
			#version 450 core

			#extension GL_ARB_gpu_shader_int64 : require

			uniform mat4 uProjection;
			uniform mat4 uView;

			in vec3 aPosition;
			in vec3 aNormal;
			in vec2 aUv;

			in mat4 iTransform;
			in uint64_t iTexture;
			in vec4 iColor;

			out vec3 vNormal;
			out vec2 vUv;
			out flat uint64_t vTexture;
			out vec4 vColor;

			void main()
			{
				gl_Position = uProjection * uView * iTransform * vec4(aPosition, 1.0);
				vNormal = aNormal;
				vUv = aUv;
				vTexture = iTexture;
				vColor = (iColor * 3.0 + vec4(aNormal, 0.0)) / 4.0;
			}
		";

		// language=glsl
		private const string FragmentShader = @"
			#version 450 core

			#extension GL_ARB_gpu_shader_int64 : require
			#extension GL_ARB_bindless_texture : require

			in vec3 vNormal;
			in vec2 vUv;
			in flat uint64_t vTexture;
			in vec4 vColor;

			out vec4 fColor;

			void main()
			{
				vec4 texture = texture(sampler2D(vTexture), vUv);
				fColor = texture * vColor;
				// TODO when we comment out this line, we crash!
				fColor = vColor;
			}
		";

		private readonly int uProjection;
		private readonly int uView;

		protected override int VertexStride => 32;
		public override int InstanceStride => 88;

		public DefaultRenderer()
			: base(DefaultRenderer.VertexShader, DefaultRenderer.FragmentShader)
		{
			this.uProjection = this.Shader.GetUniform("uProjection");
			this.uView = this.Shader.GetUniform("uView");
		}

		public override void LayoutVertexAttributes()
		{
			this.Shader.LayoutAttribute("aPosition", 0, 3, VertexAttribPointerType.Float, this.VertexStride, false);
			this.Shader.LayoutAttribute("aNormal", 12, 3, VertexAttribPointerType.Float, this.VertexStride, false);
			this.Shader.LayoutAttribute("aUv", 24, 2, VertexAttribPointerType.Float, this.VertexStride, false);
		}

		public override void LayoutInstanceAttributes()
		{
			// TODO get rid of that ugly All casting.
			this.Shader.LayoutAttribute("iTransform", 0, 16, VertexAttribPointerType.Float, this.InstanceStride, true, 4, 16);
			this.Shader.LayoutAttribute("iTexture", 64, 1, (VertexAttribPointerType)All.UnsignedInt64Arb, this.InstanceStride, true);
			this.Shader.LayoutAttribute("iColor", 72, 4, VertexAttribPointerType.Float, this.InstanceStride, true);
		}

		public void Render(Scene scene, Camera camera)
		{
			var projection = camera.GetProjectionMatrix();
			var view = camera.GetViewMatrix();

			this.Shader.Bind();

			GL.UniformMatrix4(this.uProjection, false, ref projection);
			GL.UniformMatrix4(this.uView, false, ref view);

			scene.Render();

			this.Shader.Unbind();
		}
	}
}
