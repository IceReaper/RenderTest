namespace RenderTest
{
	using OpenTK.Mathematics;
	using Renderer;
	using Renderer.Default;
	using System;
	using System.Diagnostics.CodeAnalysis;
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
					CubeModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, 0, -1), new(0.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, -0.5f), new(0, 0, -1), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, 0, -1), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, -0.5f), new(0, 0, -1), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, 0, -1), new(0.0f, 0.0f)),
					CubeModel.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(0, 0, -1), new(0.0f, 1.0f)),
					
					CubeModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, 0, 1), new(0.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, 0, 1), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 0, 1), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 0, 1), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(0, 0, 1), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, 0, 1), new(0.0f, 0.0f)),
					
					CubeModel.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(-1, 0, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(-1, 0, 0), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(-1, 0, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(-1, 0, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(-1, 0, 0), new(0.0f, 0.0f)),
					CubeModel.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(-1, 0, 0), new(1.0f, 0.0f)),
					
					CubeModel.CreateVertex(new(0.5f, 0.5f, 0.5f), new(1, 0, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(1, 0, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, -0.5f), new(1, 0, 0), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(1, 0, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, 0.5f), new(1, 0, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(1, 0, 0), new(0.0f, 0.0f)),
					
					CubeModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, -1, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, -1, 0), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, -1, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, -1, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, -1, 0), new(0.0f, 0.0f)),
					CubeModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, -1, 0), new(0.0f, 1.0f)),
					
					CubeModel.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(0, 1, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 1, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, -0.5f), new(0, 1, 0), new(1.0f, 1.0f)),
					CubeModel.CreateVertex(new(0.5f, 0.5f, 0.5f), new(0, 1, 0), new(1.0f, 0.0f)),
					CubeModel.CreateVertex(new(-0.5f, 0.5f, -0.5f), new(0, 1, 0), new(0.0f, 1.0f)),
					CubeModel.CreateVertex(new(-0.5f, 0.5f, 0.5f), new(0, 1, 0), new(0.0f, 0.0f))
				}.SelectMany(v => v.SelectMany(BitConverter.GetBytes).ToArray()).ToArray(),
				new[]
					{
						new[] { 0, 1, 2, 3, 4, 5 },
						new[] { 6, 7, 8, 9, 10, 11 },
						new[] { 12, 13, 14, 15, 16, 17 },
						new[] { 18, 19, 20, 21, 22, 23 },
						new[] { 24, 25, 26, 27, 28, 29 },
						new[] { 30, 31, 32, 33, 34, 35 }
					}.SelectMany(value => value)
					.ToArray()
			);
		}

		[SuppressMessage("ReSharper", "UseDeconstructionOnParameter")]
		private static float[] CreateVertex(Vector3 position, Vector3 normal, Vector2 uv)
		{
			return new[] { position.X, position.Y, position.Z, normal.X, normal.Y, normal.Z, uv.X, uv.Y };
		}
	}
}
