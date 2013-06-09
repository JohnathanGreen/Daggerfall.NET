using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public class ViewerComponent : GameComponent
    {
        public ViewerComponent(Game game)
            : base(game)
        {
            FieldOfView = Angle.Degrees(45);
            NearPlaneDistance = 1;
            FarPlaneDistance = 10000;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Matrix.CreatePerspectiveFieldOfView(FieldOfView.InRadians, GraphicsDevice.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance, out projection);
            frustum.Matrix = view * projection;
            viewPoint = Matrix.Invert(view).Translation;
        }

        public Angle FieldOfView { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }

        Vector3 viewPoint;
        Matrix view, projection;
        readonly BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);

        /// <summary>
        /// Get the view Single4x4.
        /// </summary>
        public Matrix View { get { return view; } protected set { view = value; } }

        /// <summary>
        /// Get the world position of the viewer.
        /// </summary>
        public Vector3 ViewPoint { get { return viewPoint; } }

        /// <summary>
        /// Get the projection matrix.
        /// </summary>
        public Matrix Projection { get { return projection; } }

        /// <summary>
        /// Get the bounding frustum.
        /// </summary>
        public BoundingFrustum Frustum { get { return frustum; } }
    }
}
