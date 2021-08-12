namespace RenderTest.Renderer.Cameras
{
	using OpenTK.Mathematics;

	public abstract class Camera
	{
		public Vector2 Size = Vector2.One;
		public Vector3 Position = Vector3.Zero;
		public Vector3 Direction = Vector3.UnitZ;
		public Vector3 Up = Vector3.UnitY;

		public float Near = 0.1f;
		public float Far = 1000;

		public abstract Matrix4 GetViewMatrix();
		public abstract Matrix4 GetProjectionMatrix();
	}
}
