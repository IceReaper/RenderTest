namespace RenderTest.Renderer.Default
{
	using OpenTK.Mathematics;

	public class DefaultModelInstance : ModelInstance
	{
		public Vector3 Position
		{
			get => this.position;
			set
			{
				this.position = value;
				this.UpdateData();
			}
		}

		private Vector3 position;

		public DefaultModelInstance(Model model, Scene scene)
			: base(model, scene)
		{
			this.UpdateData();
		}

		protected override float[] GetData()
		{
			var transform = Matrix4.CreateTranslation(this.position);

			return new[]
			{
				transform.M11,
				transform.M12,
				transform.M13,
				transform.M14,
				transform.M21,
				transform.M22,
				transform.M23,
				transform.M24,
				transform.M31,
				transform.M32,
				transform.M33,
				transform.M34,
				transform.M41,
				transform.M42,
				transform.M43,
				transform.M44
			};
		}
	}
}
