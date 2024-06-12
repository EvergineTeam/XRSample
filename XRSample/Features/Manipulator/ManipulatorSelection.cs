using Evergine.Framework;
using Evergine.Framework.Managers;
using Evergine.Mathematics;
using System;

namespace XRSample.Features.Manipulator
{

    public abstract class ManipulatorSelection : Behavior
    {
        [BindSceneManager]
        private PhysicManager3D physicManager = null;

        public Ray Pointer;

        private ManipulatorComponent selectedManipulator;

        private ManipulatorComponent pointedManipulator;

        public float RayDistance { get; set; } = 4f;

        public ManipulatorComponent PointedManipulator 
        { 
            get => pointedManipulator;
            set
            {
                if (value != this.pointedManipulator)
                {
                    this.pointedManipulator?.SetFocus(false);
                 
                    this.pointedManipulator = value;
                    this.pointedManipulator?.SetFocus(true);
                }
            }
        }

        public ManipulatorComponent SelectedManipulator
        {
            get => selectedManipulator;
            set
            {
                if (value != this.selectedManipulator)
                {
                    this.selectedManipulator?.SetSelected(false);

                    this.selectedManipulator = value;
                    this.selectedManipulator?.SetSelected(true);
                }
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (this.selectedManipulator == null)
            {
                if (this.Pointer != default)
                {
                    var hitResult = physicManager.RayCast(ref this.Pointer, this.RayDistance);
                    if (hitResult.Succeeded)
                    {
                        var pointedManipulator = hitResult.Collider?.ColliderComponent?.Owner?.FindComponent<ManipulatorComponent>();
                        this.PointedManipulator = pointedManipulator;
                    }
                    else
                    {
                        this.PointedManipulator = null;
                    }
                }
            }
            else
            {
                this.SelectedManipulator.AngleRotationSpeed = this.UpdateRotationSpeed();
            }
        }

        protected abstract float UpdateRotationSpeed();

        protected void Select()
        {
            if (this.selectedManipulator == null && this.PointedManipulator != null)
            {
                this.SelectedManipulator = this.PointedManipulator;
            }
        }

        protected void Unselect()
        {
            if (this.SelectedManipulator != null)
            {
                this.SelectedManipulator.AngleRotationSpeed = 0;
                this.SelectedManipulator = null; 
            }
        }
    }
}
