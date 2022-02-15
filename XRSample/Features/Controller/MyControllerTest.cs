using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Components.XR;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Graphics.Materials;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace XRSample.Features.Controller
{
    public class MyControllerTest : Behavior
    {
        [BindComponent]
        private Transform3D transform;

        [BindComponent]
        private TrackXRController trackXRController;

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var ray = trackXRController.Pointer;
            var color = Color.Red;
            ////this.Managers.RenderManager.LineBatch3D.DrawRay(ref ray, ref color);
        }
    }
}
