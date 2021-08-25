namespace RenderTest.Renderer
{
	using OpenTK.Graphics.OpenGL;
	using System;

	public class Texture : IDisposable
	{
		private readonly int texture;
		public readonly long Handle;

		public Texture(int width, int height, byte[] data)
		{
			this.texture = GL.GenTexture();

			GL.BindTexture(TextureTarget.Texture2D, this.texture);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.Float, data);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			this.Handle = GL.Arb.GetTextureHandle(this.texture);
			GL.Arb.MakeTextureHandleResident(this.Handle);
		}

		public void Dispose()
		{
			GL.DeleteTexture(this.texture);

			GC.SuppressFinalize(this);
		}
	}
}
