using Evergine.Common.Input.Mouse;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using System;

namespace XRSample.Features.Manipulator
{

    public class DummyManipulatorSelection : ManipulatorSelection
    {
        [BindService(isRequired: false)]
        private XRPlatform xrPlatform = null;

        private Camera3D camera3D;
        private MouseDispatcher mouse;

        private float initMouseX;
        
        public float Sensitivity { get; set; } = 0.1f;

        protected override bool OnAttached()
        {
            if (this.xrPlatform != null)
            {
                return false;
            }

            return base.OnAttached();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            this.camera3D = this.Managers.RenderManager.ActiveCamera3D;
            var display = this.camera3D?.Display;
            this.mouse = display?.MouseDispatcher;
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.mouse = null;
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.mouse != null)
            {
                var mousePos = this.mouse.Position.ToVector2();
                this.camera3D.CalculateRay(ref mousePos, out this.Pointer);

                var btnState = this.mouse.ReadButtonState(MouseButtons.Left);
                if (btnState == Evergine.Common.Input.ButtonState.Pressing)
                {
                    this.Select();
                    this.initMouseX = (float)this.mouse.Position.X;
                }
                else if (btnState == Evergine.Common.Input.ButtonState.Releasing)
                {
                    this.Unselect();
                }
            }

            base.Update(gameTime);
        }

        protected override float UpdateRotationSpeed()
        {
            var position = (float)this.mouse.Position.X;

            var distance = this.initMouseX - position;

            return distance * Sensitivity;
        }
    }
}
