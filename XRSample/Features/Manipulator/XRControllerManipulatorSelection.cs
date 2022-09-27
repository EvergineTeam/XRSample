using Evergine.Common.Attributes;
using Evergine.Common.Attributes.Converters;
using Evergine.Common.Graphics;
using Evergine.Common.Input.Keyboard;
using Evergine.Common.Input.Mouse;
using Evergine.Components.XR;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace XRSample.Features.Manipulator
{

    public class XRControllerManipulatorSelection : ManipulatorSelection
    {
        [BindComponent(isExactType: false)]
        private TrackXRController controller = null;

        [BindComponent(source: BindComponentSource.ChildrenSkipOwner, tag: "Pointer")]
        private Transform3D pointerTransform = null;

        private Vector3 initDirection;

        public float Sensitivity { get; set; } = 2;


        protected override void Update(TimeSpan gameTime)
        {
            if (this.controller.TrackedDevice != null)
            {
                this.Pointer = this.controller.Pointer;

                this.pointerTransform.Position = this.Pointer.Position;
                this.pointerTransform.LookAt(this.Pointer.GetPoint(1));

                this.controller.TrackedDevice.GetControllerState(out var controlState);

                var btnState = this.controller.ControllerState.TriggerButton;
                if (btnState == Evergine.Common.Input.ButtonState.Pressing)
                {
                    this.Select();

                    this.initDirection = this.GetRotationDirection();
                }
                else if (btnState == Evergine.Common.Input.ButtonState.Releasing)
                {
                    this.Unselect();
                }

                base.Update(gameTime); 
            }
        }

        private Vector3 GetRotationDirection()
        {
            Vector3 direction = this.Pointer.Direction;
            direction.Y = 0;
            direction.Normalize();

            return direction;
        }

        protected override float UpdateRotationSpeed()
        {
            var direction = this.GetRotationDirection();

            return Vector3.SignedAngle(this.initDirection, direction, Vector3.Up) * this.Sensitivity;
        }
    }
}
