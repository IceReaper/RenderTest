namespace RenderTest
{
	using OpenTK.Graphics.OpenGL;
	using OpenTK.Windowing.Common;
	using OpenTK.Windowing.Desktop;

	public class Window : GameWindow
	{
		private Game? game;

		public static void Main()
		{
			using var game = new Window();
			game.Run();
		}

		private Window()
			: base(
				new() { IsMultiThreaded = false, RenderFrequency = 60, UpdateFrequency = 60 },
				new()
				{
					API = ContextAPI.OpenGL,
					Profile = ContextProfile.Core,
					AutoLoadBindings = true,
					APIVersion = new(4, 6),
					IsEventDriven = false,
					NumberOfSamples = 4,
					Flags = ContextFlags.Debug
				}
			)
		{
		}

		protected override void OnLoad()
		{
			this.game = new(this);
			this.OnResize(new(this.ClientSize));
		}

		protected override void OnResize(ResizeEventArgs resizeEventArgs)
		{
			GL.Viewport(0, 0, resizeEventArgs.Width, resizeEventArgs.Height);
			this.game?.Resize(new(resizeEventArgs.Width, resizeEventArgs.Height));
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			this.game?.Update(args.Time);
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			this.game?.Render(args.Time);
		}
	}
}
