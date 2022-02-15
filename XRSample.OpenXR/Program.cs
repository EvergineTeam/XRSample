using System.Diagnostics;
using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.OpenXR;

namespace XRSample.OpenXR
{
    class Program
    {
        private static OpenXRPlatform openXRPlatform;

        static void Main(string[] args)
        {
            // Create app
            MyApplication application = new MyApplication();

            // Create Services
            uint width = 1280;
            uint height = 720;
            WindowsSystem windowsSystem = new Evergine.Forms.FormsWindowsSystem();
            application.Container.RegisterInstance(windowsSystem);
            var window = windowsSystem.CreateWindow("XRSample - DX11", width, height);

            ConfigureGraphicsContext(application, window);
			
			// Creates XAudio device
            var xaudio = new Evergine.XAudio2.XAudioDevice();
            application.Container.RegisterInstance(xaudio);

            Stopwatch clockTimer = Stopwatch.StartNew();
            windowsSystem.Run(
            () =>
            {
                application.Initialize();
            },
            () =>
            {
                var gameTime = clockTimer.Elapsed;
                clockTimer.Restart();

                openXRPlatform.Update();
                application.UpdateFrame(gameTime);
                application.DrawFrame(gameTime);
            });
        }

        private static void ConfigureGraphicsContext(Application application, Window window)
        {
            GraphicsContext graphicsContext = new Evergine.DirectX11.DX11GraphicsContext();
            graphicsContext.CreateDevice();
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = window.SurfaceInfo,
                Width = window.Width,
                Height = window.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60
            };

            graphicsContext.CreateDevice();
            application.Container.RegisterInstance(graphicsContext);

            // Create mirror display...
            FrameBuffer frameBuffer = null;
            var mirrorDisplay = new Display(window, frameBuffer);

            // Create OpenXR Platform
            openXRPlatform = new OpenXRPlatform(null, new OpenXRInteractionProfile[] { DefaultInteractionProfiles.OculusTouchProfile })
            {
                RenderMirrorTexture = false,
                ReferenceSpace = ReferenceSpaceType.Stage,
                MirrorDisplay = mirrorDisplay
            };

            application.Container.RegisterInstance(openXRPlatform);

            // Register the displays...
            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            graphicsPresenter.AddDisplay("DefaultDisplay", openXRPlatform.Display);
            graphicsPresenter.AddDisplay("MirrorDisplay", mirrorDisplay);
        }
    }
}
