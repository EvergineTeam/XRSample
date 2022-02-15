using Evergine.Common.Attributes;
using Evergine.Common.Attributes.Converters;
using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Components.Primitives;
using Evergine.Components.WorkActions;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace XRSample.Features.Manipulator
{
    public enum JointAxisRotation
    {
        XAxis,
        YAxis,
        ZAxis,
    }

    public class ManipulatorComponent : Behavior
    {
        [BindComponent(source: BindComponentSource.ParentsSkipOwner)]
        public Transform3D JointTransform;
        
        [BindComponent]
        public Transform3D Transform;

        [BindComponent]
        public Spinner spinner;

        private Vector3 refScale;

        private JointAxisRotation axisRotation = JointAxisRotation.XAxis;
        private Vector3 rotationAxis;
        private bool isFocused;
        private bool isSelected;

        [RenderProperty(Tag = 1)]
        public bool LimitAngle { get; set; } = false;
        
        [RenderPropertyAsFInput(AttachToTag = 1, AttachToValue = true, ConverterType = typeof(FloatRadianToDegreeConverter))]
        public float MinAngleLimit { get; set; }

        [RenderPropertyAsFInput(AttachToTag = 1, AttachToValue = true, ConverterType = typeof(FloatRadianToDegreeConverter))]
        public float MaxAngleLimit { get; set; }

        [RenderPropertyAsFInput(ConverterType = typeof(FloatRadianToDegreeConverter))]
        public float AngleRotationSpeed { get; set; }

        private WorkAction action;

        public JointAxisRotation AxisRotation
        {
            get => axisRotation;
            set
            {
                if (this.axisRotation != value)
                {
                    this.axisRotation = value;
                    this.RefreshAxis();
                }
            }
        }

        protected override bool OnAttached()
        {
            this.refScale = this.Transform.LocalScale;
            return base.OnAttached();
        }

        public void SetFocus(bool isFocused)
        {
            if (this.isFocused != isFocused)
            {
                this.isFocused = isFocused;

                this.RefreshFocusVisuals();
            }
        }

        private void RefreshFocusVisuals()
        {
            this.action?.Cancel();

            var initScale = this.Transform.LocalScale;
            var initSpinner = this.spinner.AxisIncrease.Z;

            if (this.isFocused)
            {

                this.action = new FloatAnimationWorkAction(this.Owner, 0, 1, TimeSpan.FromSeconds(0.3), EaseFunction.CubicInOutEase, (lerp) =>
                {
                    this.Transform.LocalScale = Vector3.Lerp(initScale, this.refScale * 1.2f, lerp);
                    this.spinner.AxisIncrease.Z = MathHelper.Lerp(initSpinner, MathHelper.ToRadians(30), lerp);
                });

                this.action.Run();
            }
            else
            {

                this.action = new FloatAnimationWorkAction(this.Owner, 0, 1, TimeSpan.FromSeconds(0.3), EaseFunction.CubicInOutEase, (lerp) =>
                {
                    this.Transform.LocalScale = Vector3.Lerp(initScale, this.refScale, lerp);
                    this.spinner.AxisIncrease.Z = MathHelper.Lerp(initSpinner, 0, lerp);
                });
                
                this.action.Run();
            }
        }


        public void SetSelected(bool isSelected)
        {
            if (this.isSelected != isSelected)
            {
                this.isSelected = isSelected;

                this.RefreshSelectedVisuals();
            }
        }

        private void RefreshSelectedVisuals()
        {
            var initScale = this.Transform.LocalScale;
            var initSpinner = this.spinner.AxisIncrease.Z;

            if (this.isSelected)
            {
                this.action = new FloatAnimationWorkAction(this.Owner, 0, 1, TimeSpan.FromSeconds(0.3), EaseFunction.CubicInOutEase, (lerp) =>
                {
                    this.Transform.LocalScale = Vector3.Lerp(initScale, this.refScale, lerp);
                    this.spinner.AxisIncrease.Z = MathHelper.Lerp(initSpinner, MathHelper.ToRadians(200), lerp);
                });

                this.action.Run();
            }
            else
            {

                this.action = new FloatAnimationWorkAction(this.Owner, 0, 1, TimeSpan.FromSeconds(0.3), EaseFunction.CubicInOutEase, (lerp) =>
                {
                    this.Transform.LocalScale = Vector3.Lerp(initScale, this.refScale, lerp);
                    this.spinner.AxisIncrease.Z = MathHelper.Lerp(initSpinner, 0, lerp);
                });

                this.action.Run();
            }
        }

        private void RefreshAxis()
        {
            switch (axisRotation)
            {
                default:
                case JointAxisRotation.XAxis:
                    this.rotationAxis = Vector3.UnitX;
                    break;
                case JointAxisRotation.YAxis:
                    this.rotationAxis = Vector3.UnitY;
                    break;
                case JointAxisRotation.ZAxis:
                    this.rotationAxis = Vector3.UnitZ;
                    break;
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.RefreshAxis();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (Math.Abs(this.AngleRotationSpeed) > MathHelper.Epsilon)
            {

                float angle = (float)(gameTime.TotalSeconds * this.AngleRotationSpeed);
                Quaternion.CreateFromAxisAngle(ref this.rotationAxis, angle, out var rotation);

                this.JointTransform.LocalOrientation *= rotation;
            }
        }

        public void SetMinLimit()
        {
            var rotation = this.JointTransform.LocalRotation;

            switch (axisRotation)
            {
                default:
                case JointAxisRotation.XAxis:
                    this.MinAngleLimit = rotation.Y;
                    break;
                case JointAxisRotation.YAxis:
                    this.MinAngleLimit = rotation.X;
                    break;
                case JointAxisRotation.ZAxis:
                    this.MinAngleLimit = rotation.Z;
                    break;
            }
        }

        public void SetMaxLimit()
        {
            var rotation = this.JointTransform.LocalRotation;

            switch (axisRotation)
            {
                default:
                case JointAxisRotation.XAxis:
                    this.MaxAngleLimit = rotation.X;
                    break;
                case JointAxisRotation.YAxis:
                    this.MaxAngleLimit = rotation.Y;
                    break;
                case JointAxisRotation.ZAxis:
                    this.MaxAngleLimit = rotation.Z;
                    break;
            }
        }

    }
}
