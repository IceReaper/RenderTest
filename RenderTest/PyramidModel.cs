namespace RenderTest
{
	using OpenTK.Mathematics;
	using Renderer;
	using Renderer.Default;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;

	public static class PyramidModel
	{
		public static Model Create(DefaultRenderer renderer)
		{
			return new(
				renderer,
				new[]
					{
						PyramidModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, 0, -1), new(0.0f, 0.0f)),
						PyramidModel.CreateVertex(new(0.0f, 0.5f, 0.0f), new(0, 0, -1), new(1.0f, 1.0f)),
						PyramidModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, 0, -1), new(1.0f, 0.0f)),

						PyramidModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, 0, 1), new(0.0f, 0.0f)),
						PyramidModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, 0, 1), new(1.0f, 0.0f)),
						PyramidModel.CreateVertex(new(0.0f, 0.5f, 0.0f), new(0, 0, 1), new(1.0f, 1.0f)),

						PyramidModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(-1, 0, 0), new(1.0f, 1.0f)),
						PyramidModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(-1, 0, 0), new(1.0f, 0.0f)),
						PyramidModel.CreateVertex(new(0.0f, 0.5f, 0.0f), new(-1, 0, 0), new(0.0f, 1.0f)),

						PyramidModel.CreateVertex(new(0.0f, 0.5f, 0.0f), new(1, 0, 0), new(0.0f, 1.0f)),
						PyramidModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(1, 0, 0), new(1.0f, 0.0f)),
						PyramidModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(1, 0, 0), new(1.0f, 1.0f)),

						PyramidModel.CreateVertex(new(-0.5f, -0.5f, -0.5f), new(0, -1, 0), new(0.0f, 1.0f)),
						PyramidModel.CreateVertex(new(0.5f, -0.5f, -0.5f), new(0, -1, 0), new(1.0f, 1.0f)),
						PyramidModel.CreateVertex(new(0.5f, -0.5f, 0.5f), new(0, -1, 0), new(1.0f, 0.0f)),
						PyramidModel.CreateVertex(new(-0.5f, -0.5f, 0.5f), new(0, -1, 0), new(0.0f, 0.0f))
					}.SelectMany(v => v.SelectMany(BitConverter.GetBytes).ToArray())
					.ToArray(),
				new[]
					{
						new[] { 0, 1, 2 },
						new[] { 3, 4, 5 },
						new[] { 6, 7, 8 },
						new[] { 9, 10, 11 },
						new[] { 12, 13, 14, 14, 15, 12 }
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
