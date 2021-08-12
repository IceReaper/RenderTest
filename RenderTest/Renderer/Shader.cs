﻿namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;

	public class Shader : IDisposable
	{
		public readonly int Program;

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

			this.Program = GL.CreateProgram();
			GL.AttachShader(this.Program, vertexShader);
			GL.AttachShader(this.Program, fragmentShader);
			GL.LinkProgram(this.Program);
			GL.GetProgramInfoLog(this.Program, out var programError);

			if (!string.IsNullOrWhiteSpace(programError))
				throw new(programError);

			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		public void LayoutAttribute(string name, int offset, int amount, int stride, bool instanced)
		{
			var index = GL.GetAttribLocation(this.Program, name);
			GL.EnableVertexAttribArray(index);
			GL.VertexAttribPointer(index, amount, VertexAttribPointerType.Float, false, stride, offset);

			if (instanced)
				GL.VertexAttribDivisor(index, 1);
		}

		public void Dispose()
		{
			GL.DeleteProgram(this.Program);
			GC.SuppressFinalize(this);
		}
	}
}