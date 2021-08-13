namespace RenderTest
{
	using Renderer;
	using Renderer.Default;
	using System.Linq;

	public static class CubeModel
	{
		public static Model Create(DefaultRenderer renderer)
		{
			return new(
				renderer,
				new[]
				{
					// TODO remove duplicates!
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, 0, -1), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, -0.5f), new(0, 0, -1), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, 0, -1), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, -0.5f), new(0, 0, -1), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, 0, -1), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(0, 0, -1), new(0.0f, 1.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, 0, 1), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, 0, 1), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 0, 1), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 0, 1), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(0, 0, 1), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, 0, 1), new(0.0f, 0.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(-1, 0, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(-1, 0, 0), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(-1, 0, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(-1, 0, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(-1, 0, 0), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(-1, 0, 0), new(1.0f, 0.0f)),
					
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, 0.5f), new(1, 0, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(1, 0, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, -0.5f), new(1, 0, 0), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(1, 0, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, 0.5f), new(1, 0, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(1, 0, 0), new(0.0f, 0.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, -1, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, -1, 0), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, -1, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, -1, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, -1, 0), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, -1, 0), new(0.0f, 1.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(0, 1, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 1, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, -0.5f), new(0, 1, 0), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 1, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(0, 1, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(0, 1, 0), new(0.0f, 0.0f))
				},
				new[]
					{
						new uint[] { 0, 1, 2, 3, 4, 5 },
						new uint[] { 6, 7, 8, 9, 10, 11 },
						new uint[] { 12, 13, 14, 15, 16, 17 },
						new uint[] { 18, 19, 20, 21, 22, 23 },
						new uint[] { 24, 25, 26, 27, 28, 29 },
						new uint[] { 30, 31, 32, 33, 34, 35 }
					}.SelectMany(value => value)
					.ToArray()
			);
		}
	}
}
