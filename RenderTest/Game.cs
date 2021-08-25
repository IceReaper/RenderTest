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
		private readonly Model model1;
		private readonly Model model2;
		private readonly Camera camera;

		public Game(Window window)
		{
			this.window = window;

			this.renderer = new();
			this.scene = new(this.renderer);
			this.model1 = CubeModel.Create(this.renderer);
			this.model2 = PyramidModel.Create(this.renderer);
			this.camera = new PerspectiveCamera();
			this.camera.Position = new(0, 20, 0);
			this.camera.Direction = new(20, -20, 20);

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var random = new Random();

			for (var z = 0; z < 1000; z++)
			for (var x = 0; x < 1000; x++)
			{
				var instance = new DefaultModelInstance(random.NextSingle() < .5 ? this.model1 : this.model2, this.scene);
				instance.Position = new(x * 2, 0, z * 2);
				instance.Scale = new(.5f + random.NextSingle(), .5f + random.NextSingle(), .5f + random.NextSingle());
				instance.Rotation = new(random.NextSingle(), random.NextSingle(), random.NextSingle());
				instance.Color = new(random.NextSingle(), random.NextSingle(), random.NextSingle(), 1);
			}

			stopwatch.Stop();
			Console.WriteLine($"Start took {stopwatch.Elapsed.TotalMilliseconds}");

			GL.ClearColor(0.35f, 0.55f, 0.65f, 0.0f);
			GL.Enable(EnableCap.DepthTest);
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
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			this.renderer.Render(this.scene, this.camera);
			this.window.Context.SwapBuffers();
			stopwatch.Stop();
			this.window.Title = $"Frame took {stopwatch.Elapsed.TotalMilliseconds}";
		}

		public void Dispose()
		{
			this.scene.Dispose();
			this.model1.Dispose();
			this.model2.Dispose();
			this.renderer.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
