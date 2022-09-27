using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Components.WorkActions;
using Evergine.Components.XR;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Materials;
using Evergine.Framework.Prefabs;
using Evergine.Framework.Services;
using Evergine.Framework.XR;
using Evergine.Framework.XR.TrackedDevices;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRSample.Features.Controller
{
    public class MyControllerTest : Component
    {
        [BindComponent(isExactType: false)]
        private TrackXRDevice trackedDevice = null;

        [BindComponent(isRequired: false)]
        private MaterialComponent overridedMaterialComponent = null;

        private Entity renderableEntity;

        protected override void OnActivated()
        {
            base.OnActivated();
            this.trackedDevice.OnTrackedDeviceChanged += this.TrackedDevice_OnTrackedDeviceChanged;
        }


        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.trackedDevice.OnTrackedDeviceChanged -= this.TrackedDevice_OnTrackedDeviceChanged;
        }

        private void TrackedDevice_OnTrackedDeviceChanged(object sender, XRTrackedDevice newDevice)
        {
            if (this.renderableEntity != null)
            {
                this.Owner.RemoveChild(this.renderableEntity);
                this.renderableEntity = null;
            }

            this.LoadModel();
        }

        protected override void Start()
        {
            base.Start();

            this.LoadModel();
        }

        private async void LoadModel()
        {
            if (this.trackedDevice.TrackedDevice == null)
            {
                return;
            }

            this.renderableEntity = await this.trackedDevice.TrackedDevice.TryGetRenderableModelAsync();

            if (this.renderableEntity != null)
            {
                if (this.overridedMaterialComponent != null)
                {
                    var overridedMaterial = this.overridedMaterialComponent.Material;

                    foreach (var materialComponent in this.renderableEntity.FindComponentsInChildren<MaterialComponent>())
                    {
                        materialComponent.Material = overridedMaterial;
                    }
                }

                new ActionWorkAction(() =>
                {
                    this.Owner.AddChild(this.renderableEntity);
                },
                this.Owner.Scene).Run();
            }
        }
    }
}
