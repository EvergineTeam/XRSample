using Evergine.Common.Curves;
using Evergine.Common.Graphics;
using Evergine.Components.XR;
using Evergine.Framework;
using Evergine.Framework.Services;
using Evergine.Framework.XR.Passthrough;
using System;
using System.Linq;

namespace XRSample.Features.PassthroughController
{
    public class PassthroughController : Behavior
    {
        private enum PassthroughState
        {
            None = 0,
            BgPassthrough,
            MeshPassthrough,
            CombinedRampColor,
            CombinedRampMono,
        }

        [BindService]
        private XRPlatform xrPlatform = null;

        [BindComponent]
        private TrackXRController controller = null;

        [BindEntity(source: BindEntitySource.Scene, tag: "Scenery")]
        private Entity scenery = null;

        [BindEntity(source: BindEntitySource.Scene, tag: "PassthtoughHelp", isRequired:false)]
        private Entity help = null;

        private XRPassthroughLayerComponent backgroundPassthrough;
        private XRPassthroughLayerComponent meshPassthrough;

        private XRPassthroughLayerComponent selectedPassthrough;

        private PassthroughState passthroughState = PassthroughState.None;

        protected override bool OnAttached()
        {
            base.OnAttached();

            if (!Application.Current.IsEditor && this.help != null)
            {
                this.help.IsEnabled = this.xrPlatform.Passthrough != null;
            }

            if (this.xrPlatform.Passthrough == null)
            {
                return false;
            }

            return true;
        }

        protected override void Start()
        {
            var layers = this.Managers.EntityManager.FindComponentsOfType<XRPassthroughLayerComponent>().ToList();
            this.backgroundPassthrough = layers.Where(p => p.ProjectionSurface == Evergine.Framework.XR.Passthrough.XRProjectionSurfaceType.Reconstructed).FirstOrDefault();
            this.meshPassthrough = layers.Where(p => p.ProjectionSurface == Evergine.Framework.XR.Passthrough.XRProjectionSurfaceType.UserDefined).FirstOrDefault();

            this.meshPassthrough.EdgeRendering = false;
            this.meshPassthrough.EdgeColor = Color.Red;
            this.backgroundPassthrough.EdgeRendering = false;
            this.backgroundPassthrough.EdgeColor = Color.Blue;

            ColorCurve colorMap = new ColorCurve();
            colorMap.Keyframes.Clear();
            colorMap.AddKey(0, Color.Black);
            colorMap.AddKey(0.1f, Color.Blue);
            colorMap.AddKey(0.4f, Color.Green);
            colorMap.AddKey(0.6f, Color.Red);
            colorMap.AddKey(0.8f, Color.Yellow);
            colorMap.AddKey(1, Color.White);

            this.meshPassthrough.ColorMapMonoToRGBA = colorMap;

            ////this.layer.Contrast = 2;
            ////this.layer.Saturation = 4;


            FloatCurve monoMap = new FloatCurve();
            monoMap.Keyframes.Clear();
            monoMap.AddKey(0, 1);
            monoMap.AddKey(1, 0);

            this.meshPassthrough.ColorMapMonoToMono = monoMap;

            this.SetPassthroughState(PassthroughState.None);

            base.Start();
        }

        protected override void Update(TimeSpan gameTime)
        {
            var controllerState = this.controller.ControllerState;
            if (controllerState.Button2 == Evergine.Common.Input.ButtonState.Pressing)
            {
                this.SetPassthroughState((PassthroughState)(((int)this.passthroughState + 1 + 5) % 5));
            }

            if (controllerState.Button1 == Evergine.Common.Input.ButtonState.Pressing)
            {
                this.SetPassthroughState((PassthroughState)(((int)this.passthroughState - 1 + 5) % 5));
            }

            if (this.selectedPassthrough != null)
            {
                if (controllerState.Menu == Evergine.Common.Input.ButtonState.Pressing)
                {
                    this.selectedPassthrough.EdgeRendering = !this.selectedPassthrough.EdgeRendering;
                }

                if (controllerState.ThumbStickButton == Evergine.Common.Input.ButtonState.Pressing)
                {
                    this.backgroundPassthrough.Opacity = 1;
                    this.meshPassthrough.Opacity = 1;
                }

                if (Math.Abs(controllerState.ThumbStick.X) > Math.Abs(controllerState.ThumbStick.Y))
                {
                    this.selectedPassthrough.Opacity *= (controllerState.ThumbStick.X * (float)gameTime.TotalSeconds * 2) + 1;
                }
            }
        }

        private void SetPassthroughState(PassthroughState state)
        {
            this.passthroughState = state;

            switch (state)
            {
                case PassthroughState.None:
                    this.scenery.IsEnabled = true;
                    this.backgroundPassthrough.IsEnabled = false;
                    this.meshPassthrough.IsEnabled = false;
                    this.meshPassthrough.ColorControl = XRPassthroughColorControlType.None;
                    this.selectedPassthrough = null;
                    break;

                case PassthroughState.BgPassthrough:
                    this.scenery.IsEnabled = false;
                    this.backgroundPassthrough.IsEnabled = true;
                    this.meshPassthrough.IsEnabled = false;
                    this.meshPassthrough.ColorControl = XRPassthroughColorControlType.None;
                    this.selectedPassthrough = this.backgroundPassthrough;
                    break;

                case PassthroughState.MeshPassthrough:
                    this.scenery.IsEnabled = true;
                    this.backgroundPassthrough.IsEnabled = false;
                    this.meshPassthrough.IsEnabled = true;
                    this.meshPassthrough.ColorControl = XRPassthroughColorControlType.None;
                    this.selectedPassthrough = this.meshPassthrough;
                    break;

                case PassthroughState.CombinedRampColor:
                    this.scenery.IsEnabled = false;
                    this.backgroundPassthrough.IsEnabled = true;
                    this.meshPassthrough.IsEnabled = true;
                    this.meshPassthrough.ColorControl = XRPassthroughColorControlType.ColorMap;
                    this.selectedPassthrough = this.meshPassthrough;
                    break;

                case PassthroughState.CombinedRampMono:
                    this.scenery.IsEnabled = false;
                    this.backgroundPassthrough.IsEnabled = true;
                    this.meshPassthrough.IsEnabled = true;
                    this.meshPassthrough.ColorControl = XRPassthroughColorControlType.GrayscaleMap;
                    this.selectedPassthrough = this.meshPassthrough;
                    break;
            }
        }
    }
}
