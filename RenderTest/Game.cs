namespace RenderTest
{
	using OpenTK.Graphics.OpenGL;
	using OpenTK.Mathematics;
	using Renderer;
	using Renderer.Cameras;
	using Renderer.Default;
	using System;
	using System.Diagnostics;

	public class Game : IDisposable
	{
		private readonly Window window;
		private readonly DefaultRenderer renderer;
		private readonly Scene scene;
		private readonly Model model;
		private readonly Camera camera;

		public Game(Window window)
		{
			this.window = window;

			this.renderer = new();
			this.scene = new(this.renderer);
			this.model = CubeModel.Create(this.renderer);
			this.camera = new PerspectiveCamera();
			this.camera.Position = new(-20, 20, -20);
			this.camera.Direction = new(20, -20, 20);

			for (var z = 0; z < 1; z++)
			for (var x = 0; x < 2; x++)
			{
				var instance = new DefaultModelInstance(this.model, this.scene);
				instance.Position = new(x * 2, 0, z * 2);
			}

			GL.ClearColor(0.35f, 0.55f, 0.65f, 0.0f);
		}

		public void Resize(Vector2 size)
		{
			this.camera.Size = size;
		}

		public void Update(double delta)
		{
		}

		public void Render(double delta)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			GL.Clear(ClearBufferMask.ColorBufferBit);

			this.renderer.Render(this.scene, this.camera);

			this.window.Context.SwapBuffers();

			stopwatch.Stop();
			this.window.Title = $"{1000 / stopwatch.Elapsed.TotalMilliseconds} FPS";
		}

		public void Dispose()
		{
			this.scene.Dispose();
			this.model.Dispose();
			this.renderer.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
