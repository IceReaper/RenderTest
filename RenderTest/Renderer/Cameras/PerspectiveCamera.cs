namespace RenderTest.Renderer.Cameras
{
	using OpenTK.Mathematics;

	public class PerspectiveCamera : Camera
	{
		public float Fov = 90;

		public override Matrix4 GetViewMatrix()
		{
			return Matrix4.LookAt(this.Position, this.Position + this.Direction, this.Up);
		}

		public override Matrix4 GetProjectionMatrix()
		{
			return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(this.Fov), this.Size.X / this.Size.Y, this.Near, this.Far);
		}
	}
}
