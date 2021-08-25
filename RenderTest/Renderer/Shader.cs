namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;

	public class Shader : IDisposable
	{
		private readonly int program;
		private bool bound;

		public Shader(string vertexShaderSource, string fragmentShaderSource)
		{
			var vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, vertexShaderSource);
			GL.CompileShader(vertexShader);
			GL.GetShaderInfoLog(vertexShader, out var vertexShaderError);

			if (!string.IsNullOrWhiteSpace(vertexShaderError))
				throw new(vertexShaderError);

			var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, fragmentShaderSource);
			GL.CompileShader(fragmentShader);
			GL.GetShaderInfoLog(fragmentShader, out var fragmentShaderError);

			if (!string.IsNullOrWhiteSpace(fragmentShaderError))
				throw new(fragmentShaderError);

			this.program = GL.CreateProgram();
			GL.AttachShader(this.program, vertexShader);
			GL.AttachShader(this.program, fragmentShader);
			GL.LinkProgram(this.program);
			GL.GetProgramInfoLog(this.program, out var programError);

			if (!string.IsNullOrWhiteSpace(programError))
				throw new(programError);

			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		public void LayoutAttribute(
			string name,
			int offset,
			int components,
			VertexAttribPointerType type,
			int vertexStride,
			bool instanced,
			int entries = 1,
			int entryShift = 0
		)
		{
			var index = GL.GetAttribLocation(this.program, name);

			for (var i = 0; i < entries; i++)
			{
				GL.EnableVertexAttribArray(index + i);
				GL.VertexAttribPointer(index + i, components / entries, type, false, vertexStride, offset + entryShift * i);

				if (instanced)
					GL.VertexAttribDivisor(index + i, 1);
			}
		}

		public int GetUniform(string location)
		{
			return GL.GetUniformLocation(this.program, location);
		}

		public void Bind()
		{
			GL.UseProgram(this.program);
			this.bound = true;
		}

		public void Unbind()
		{
			GL.UseProgram(0);
			this.bound = false;
		}

		public void Dispose()
		{
			if (this.bound)
				this.Unbind();

			GL.DeleteProgram(this.program);
			GC.SuppressFinalize(this);
		}
	}
}
