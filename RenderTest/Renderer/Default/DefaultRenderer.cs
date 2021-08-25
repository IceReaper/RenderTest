namespace RenderTest.Renderer.Default
{
	using Cameras;
	using OpenTK.Graphics.OpenGL;

	public class DefaultRenderer : Renderer
	{
		// language=glsl
		private const string VertexShader = @"
			#version 450 core

			uniform mat4 uProjection;
			uniform mat4 uView;

			in vec3 aPosition;
			in vec3 aNormal;
			in vec2 aUv;

			in mat4 iTransform;
			in vec4 iColor;

			out vec3 vNormal;
			out vec2 vUv;
			out vec4 vColor;

			void main()
			{
				gl_Position = uProjection * uView * iTransform * vec4(aPosition, 1.0);
				vNormal = aNormal;
				vUv = aUv;
				vColor = (iColor * 3.0 + vec4(aNormal, 0.0)) / 4.0;
			}
		";

		// language=glsl
		private const string FragmentShader = @"
			#version 450 core

			in vec3 vNormal;
			in vec2 vUv;
			in vec4 vColor;

			out vec4 fColor;

			void main()
			{
				fColor = vColor;
			}
		";

		private readonly int uProjection;
		private readonly int uView;

		protected override int VertexStride => (3 + 3 + 2) * sizeof(float);
		public override int InstanceStride => (16 + 4) * sizeof(float);

		public DefaultRenderer()
			: base(DefaultRenderer.VertexShader, DefaultRenderer.FragmentShader)
		{
			this.uProjection = this.Shader.GetUniform("uProjection");
			this.uView = this.Shader.GetUniform("uView");
		}

		public override void LayoutVertexAttributes()
		{
			this.Shader.LayoutAttribute("aPosition", 0, 3, this.VertexStride, false);
			this.Shader.LayoutAttribute("aNormal", 3, 3, this.VertexStride, false);
			this.Shader.LayoutAttribute("aUv", 6, 2, this.VertexStride, false);
		}

		public override void LayoutInstanceAttributes()
		{
			this.Shader.LayoutAttribute("iTransform", 0, 16, this.InstanceStride, true, 4);
			this.Shader.LayoutAttribute("iColor", 16, 4, this.InstanceStride, true);
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
