namespace RenderTest
{
	using Renderer;
	using Renderer.Default;
	using System.Linq;

	public static class PyramidModel
	{
		public static Model Create(DefaultRenderer renderer)
		{
			return new(
				renderer,
				new[]
				{
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, 0, -1), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.0f, 0.5f, 0.0f), new(0, 0, -1), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, 0, -1), new(1.0f, 0.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, 0, 1), new(0.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, 0, 1), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.0f, 0.5f, 0.0f), new(0, 0, 1), new(1.0f, 1.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(-1, 0, 0), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(-1, 0, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.0f, 0.5f, 0.0f), new(-1, 0, 0), new(0.0f, 1.0f)),
					
					DefaultRenderer.CreateVertex(new(0.0f, 0.5f, 0.0f), new(1, 0, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(1, 0, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(1, 0, 0), new(1.0f, 1.0f)),
					
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, -1, 0), new(0.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, -1, 0), new(1.0f, 1.0f)),
					DefaultRenderer.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, -1, 0), new(1.0f, 0.0f)),
					DefaultRenderer.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, -1, 0), new(0.0f, 0.0f))
				},
				new[]
					{
						new uint[] { 0, 1, 2 },
						new uint[] { 3, 4, 5 },
						new uint[] { 6, 7, 8 },
						new uint[] { 9, 10, 11 },
						new uint[] { 12, 13, 14, 14, 15, 12 }
					}.SelectMany(value => value)
					.ToArray()
			);
		}
	}
}
