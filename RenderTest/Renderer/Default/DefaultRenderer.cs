namespace RenderTest.Renderer.Default
{
	using Cameras;
	using OpenTK.Graphics.OpenGL;
	using OpenTK.Mathematics;
	using System.Diagnostics.CodeAnalysis;

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

		private const int VertexStride = (3 + 3 + 2) * sizeof(float);

		private readonly int uProjection;
		private readonly int uView;

		public override int InstanceStride => (16 + 4) * sizeof(float);

		public DefaultRenderer()
			: base(DefaultRenderer.VertexShader, DefaultRenderer.FragmentShader, DefaultRenderer.VertexStride)
		{
			this.uProjection = GL.GetUniformLocation(this.Shader.Program, "uProjection");
			this.uView = GL.GetUniformLocation(this.Shader.Program, "uView");
		}

		[SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
		public static float[] CreateVertex(Vector3 position, Vector3 normal, Vector2 uv)
		{
			return new[] { position.X, position.Y, position.Z, normal.X, normal.Y, normal.Z, uv.X, uv.Y };
		}

		protected override void LayoutAttributes()
		{
			this.Shader.LayoutAttribute("aPosition", 0, 3, DefaultRenderer.VertexStride, false);
			this.Shader.LayoutAttribute("aNormal", 3, 3, DefaultRenderer.VertexStride, false);
			this.Shader.LayoutAttribute("aUv", 6, 2, DefaultRenderer.VertexStride, false);
		}

		protected override void LayoutInstancedAttributes()
		{
			this.Shader.LayoutAttribute("iTransform", 0, 16, this.InstanceStride, true, 4);
			this.Shader.LayoutAttribute("iColor", 16, 4, this.InstanceStride, true);
		}

		public void Render(Scene scene, Camera camera)
		{
			GL.UseProgram(this.Shader.Program);

			var projection = camera.GetProjectionMatrix();
			var view = camera.GetViewMatrix();

			GL.UniformMatrix4(this.uProjection, false, ref projection);
			GL.UniformMatrix4(this.uView, false, ref view);

			base.Render(scene);

			GL.UseProgram(0);
		}
	}
}
