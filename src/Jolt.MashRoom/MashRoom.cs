using System;
using System.IO;
using System.Windows.Forms;
using Ignostic;
using Ignostic.Audio;
using Ignostic.Studio256.RenderApi;
using Ignostic.Studio256.RenderApi.Tools;
using Ignostic.Timing;
using Jolt.Cuberick.Effects;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Un4seen.Bass;
using Ignostic.Timing.Sync;

namespace Jolt.MashRoom
{
    public static partial class Program
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create main disposer
            var mainDisposer = new Disposer();

            //
            var random = new Random(1415926535);

            #region demo setup
            // show setup dialog and get setup values
            var title = "Mash Room by Jolt";
            var setupView = new StartupView(title);
            var setupModel = new SetupModel();
            var presenter = new SetupPresenter(setupView, setupModel);
            setupModel.TryLoadSettings();
            setupModel.SelectAdapter(0);
            var dialogResult = setupView.ShowDialog();
            if (dialogResult != DialogResult.OK)
                return;
            #if DEBUG
            setupModel.SaveSettings(); // todo: remove save in release mode
            #endif
            #endregion

            // Create Device and SwapChain
            var demo = new Demo().Init(setupModel, title);
            var inputHandler = new InputHandler().Bind(demo);

            #region audio
            //
            var audioDeviceType = (setupModel.UseAudio) ? AudioDeviceType.Bass : AudioDeviceType.Silent;
            var audioDeviceManager = new AudioDeviceManager();
            var audioDevice = mainDisposer.Add(audioDeviceManager.CreateDevice(audioDeviceType));

            //
            if (setupModel.UseAudio)
            {
                //
                BassNet.Registration(
                    setupModel.BassRegistrationEmail ?? "",
                    setupModel.BassRegistrationKey ?? "");

                // 140 bpm
                // delay 3.428 (dvs 2 takter i 140 bpm)
                var bpm = 140;
                var audioName = "Mashup_v1.00.mp3";
                var audioAsset = mainDisposer.Add(demo.AudioManager[audioName]);

                // init and load audio
                audioDevice.Init();
                audioDevice.Load(audioAsset.Value);
                audioDevice.PlayPosition = setupModel.StartTime;
                audioDevice.Bpm = bpm;

                // use the audio device as timer
                demo.Timer = audioDevice;
            }
            #endregion

            #region sync
            demo.SyncManager = new SyncManager().Init(setupModel.SyncRecordMode, demo.Timer.Bpm, 4);
            demo.SyncManager.TimerDevice = demo.Timer;
            #endregion

            #region shaders
            var planeVertexShader = demo.ShaderManager["plane.vs.cso"].VertexShader;
            var planePixelShader = demo.ShaderManager["plane.ps.cso"].PixelShader;
            var postPixelShader = demo.ShaderManager["post.ps.cso"].PixelShader;
            var postVertexShader = demo.ShaderManager["post.vs.cso"].VertexShader;
            var vanillaEffect = mainDisposer.Add(new VanillaEffect(demo).Init());
            var vanillaLayout = mainDisposer.Add(vanillaEffect.InputLayout.InputLayout);
            #endregion

            #region models

            // load all models
            demo.ModelManager.LoadAll();

            // make all models white (todo: why)
            foreach (var model in demo.ModelManager)
            {
                model.Color = new Color(255, 255, 255, 255);
                model.ReCreateBuffer(demo.Device);
            }

            #endregion

            #region textures
            Texture2D backBuffer = null;
            Texture2D depthBuffer = null;
            DepthStencilView depthView = null;

            // 1D texture for scanline distortion
            var distortionTexture = new Texture1D(demo.Device, new Texture1DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Usage = ResourceUsage.Default,
                Width = 1080,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = BindFlags.ShaderResource,
                OptionFlags = ResourceOptionFlags.None
            });
            var distortionData = new byte[4 * distortionTexture.Description.Width];
            var distortionSRV = new ShaderResourceView(demo.Device, distortionTexture);

            // foreground texture
            var foregroundTexture = new Texture2D(demo.Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8_UNorm,
                Usage = ResourceUsage.Default,
                Width = 1920,
                Height = 1080,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = BindFlags.ShaderResource,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
            });
            var foregroundSRV = new ShaderResourceView(demo.Device, foregroundTexture);
            var foregroundData = new byte[foregroundTexture.Description.Width * foregroundTexture.Description.Height];

            // noise texture
            var noiseTexture = new Texture2D(demo.Device, new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8_UNorm,
                Usage = ResourceUsage.Default,
                Width = 1920 / 5,
                Height = 1080 / 5,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = BindFlags.ShaderResource,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
            });
            var noiseSRV = new ShaderResourceView(demo.Device, noiseTexture);
            var noiseData = new byte[noiseTexture.Description.Width * noiseTexture.Description.Height];
            #endregion

            #region acts
            var act0 = mainDisposer.Add(new Act0(demo).Init());
            #endregion

            #region effects
            //var fractalCityEffect = mainDisposer.Add(new FractalCityEffect(demo).Init());
            //var oceanEffect = mainDisposer.Add(new OceanEffect(demo).Init());
            //var dustEffect = mainDisposer.Add(new DustEffect(demo).Init());
            //var cloudEffect = mainDisposer.Add(new CloudEffect(demo).Init());
            //var mandelbulbEffect = mainDisposer.Add(new MandelbulbEffect(demo).Init());
            //var redPlanetEffect = mainDisposer.Add(new Effect(demo).Init(new EffectDescription
            //{
            //    PixelShaderName = "redPlanet.ps.cso",
            //    TextureNames = "redPlanet0.png,redPlanet1.jpg".Split(',')
            //}));
            //var pseudoKleinianEffect = mainDisposer.Add(new Effect(demo).Init(new EffectDescription
            //{
            //    PixelShaderName = "pseudoKleinian.ps.cso",
            //}));
            //var nonameEffect00 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname00.ps.cso", TextureNames = "tex09.jpg".Split(',') }));
            //var nonameEffect01 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname01.ps.cso" }));
            //var nonameEffect02 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname02.ps.cso", TextureNames = "tex09.jpg,tex07.jpg".Split(',') }));
            //var nonameEffect03 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname03.ps.cso" }));
            //var nonameEffect04 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname04.ps.cso", TextureNames = "noise256.png".Split(',') }));
            //var nonameEffect05 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname05.ps.cso" }));
            //var nonameEffect06 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname06.ps.cso" }));
            //var nonameEffect07 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname07.ps.cso" }));
            //var nonameEffect08 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname08.ps.cso" }));
            //var nonameEffect09 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname09.ps.cso" }));
            //var nonameEffect10 = mainDisposer.Add(new Effect(demo).Init(new EffectDescription { PixelShaderName = "noname10.ps.cso" }));
            #endregion

            var fillStateMsaadesc = RasterizerStateDescription.Default();
            fillStateMsaadesc.IsMultisampleEnabled = true;
            var msaaFillState = new RasterizerState(demo.Device, fillStateMsaadesc);
            var fillState = new RasterizerState(demo.Device, RasterizerStateDescription.Default());
            var vanillaRenderContext = null as RenderContext;
            var postRenderContext = null as RenderContext;
            var planeRenderContext = null as RenderContext;

            // Main loop
            using (var renderDisposer = new Disposer())
            {
                RenderLoop.Run(demo.Form, () =>
                {
                    if (demo.SyncManager.RowIndex == 0x000006d0)
                    {
                        demo.Form.Close();
                        return;
                    }

                    // TODO refactor: move the resize code
                    #region resize
                    if (demo.OutputWasResized)
                    {
                        demo.OutputWasResized = false;
                        renderDisposer.DisposeAll();

                        // dispose old swapchain
                        if (demo.SwapChain != null)
                        {
                            demo.SwapChain.Dispose();
                        }

                        // create new swapchain
                        demo.SwapChain = renderDisposer.Add(new SwapChain(setupModel.Factory, demo.Device, new SwapChainDescription
                        {
                            BufferCount = 1,
                            ModeDescription = setupModel.Mode,
                            IsWindowed = !setupModel.FullScreen,
                            OutputHandle = demo.Form.Handle,
                            //SampleDescription = new SampleDescription(setupModel.MultiSampleCount, setupModel.MultiSampleQuality),
                            SampleDescription = new SampleDescription(1, 0),
                            SwapEffect = SwapEffect.Discard,
                            Usage = Usage.RenderTargetOutput
                        }));
                        if (setupModel.FullScreen)
                        {
                            demo.SwapChain.SetFullscreenState(true, setupModel.Output);
                        }

                        //
                        var postOutputRenderTarget = new RenderTarget(demo.Device, demo.SwapChain);

                        // Get the backbuffer from the swapchain
                        backBuffer = renderDisposer.Add(demo.SwapChain.GetBackBuffer<Texture2D>(0));

                        // Setup targets and viewport for rendering
                        var postInputRenderTarget = new RenderTarget(
                            device: demo.Device,
                            width: setupModel.Mode.Width / 1,   // todo: introduce a scalefactor
                            height: setupModel.Mode.Height / 1, // todo: introduce a scalefactor
                            sampleCount: 1,                     // todo: use setupModel.MultiSampleCount,
                            sampleQuality: 0,                   // todo: use setupModel.MultiSampleQuality,
                            format: Format.R8G8B8A8_UNorm       // todo: use setupModel.Format
                        );

                        // Create the depth buffer
                        depthBuffer = renderDisposer.Add(new Texture2D(demo.Device, new Texture2DDescription
                        {
                            Format = Format.D32_Float_S8X24_UInt,
                            ArraySize = 1,
                            MipLevels = 1,
                            Width = postInputRenderTarget.Width,
                            Height = postInputRenderTarget.Height,
                            SampleDescription = postInputRenderTarget.Texture.Description.SampleDescription,
                            Usage = ResourceUsage.Default,
                            BindFlags = BindFlags.DepthStencil,
                            CpuAccessFlags = CpuAccessFlags.None,
                            OptionFlags = ResourceOptionFlags.None
                        }));

                        // Create the depth buffer view
                        depthView = renderDisposer.Add(new DepthStencilView(demo.Device, depthBuffer));

                        vanillaRenderContext = renderDisposer.Add(new RenderContext
                        {
                            PrimitiveTopology = PrimitiveTopology.TriangleList,
                            InputLayout = vanillaLayout,
                            VertexShader = null,
                            PixelShader = null,
                            RasterizerState = msaaFillState,
                            DepthStencilView = depthView,
                            RenderTarget = postInputRenderTarget,
                        });

                        postRenderContext = renderDisposer.Add(new RenderContext
                        {
                            PrimitiveTopology = PrimitiveTopology.TriangleList,
                            InputLayout = vanillaLayout,
                            VertexShader = postVertexShader,
                            PixelShader = postPixelShader,
                            RasterizerState = fillState,
                            DepthStencilView = null,
                            RenderTarget = postOutputRenderTarget,
                        });

                        planeRenderContext = new RenderContext
                        {
                            PrimitiveTopology = PrimitiveTopology.TriangleList,
                            InputLayout = vanillaLayout,
                            VertexShader = planeVertexShader,
                            PixelShader = planePixelShader,
                            RasterizerState = fillState,
                            DepthStencilView = null,
                            RenderTarget = postOutputRenderTarget,
                        };

                        // todo: remove this hack
                        if (!setupModel.SyncRecordMode)
                        {
                            demo.Timer.StartPlaying();
                        }

                        if (setupModel.FullScreen)
                        {
                            Cursor.Hide();
                        }
                    }
                    #endregion

                    // todo: remove
                    demo.SyncManager.Update(demo.Timer.Time);
                    var syncRow = new
                    {
                        demo.SyncManager.Data.Part,
                        demo.SyncManager.Data.Lead,
                    };

                    // Present!
                    // a crash here is quite common when VertexShader or PixelShader is null
                    demo.SwapChain.Present(setupModel.UseVerticalSync ? 1 : 0, PresentFlags.None);

                    // horizontal distortion and color separation
                    random.NextBytes(distortionData);
                    demo.DeviceContext.UpdateSubresource(distortionData, distortionTexture);

                    // static noise
                    random.NextBytes(noiseData);
                    demo.DeviceContext.UpdateSubresource(noiseData, noiseTexture, 0, noiseTexture.Description.Width);

                    // setup for normal rendering
                    {
                        // use normal rendering
                        demo.DeviceContext.PixelShader.SetShaderResource(0, null);
                        demo.RenderContext = vanillaRenderContext;

                        // clear
                        demo.DeviceContext.ClearDepthStencilView(demo.RenderContext.DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
                        demo.DeviceContext.ClearRenderTargetView(demo.RenderContext.RenderTarget.RenderTargetView, Color.Black);
                    }

                    // render act
                    act0.Render();

                    //
                    demo.DeviceContext.ClearRenderTargetView(postRenderContext.RenderTarget.RenderTargetView, Color.Black);

                    // render plane to screen
                    if (true)
                    {
                        // TODO is this really needed ?
                        demo.DeviceContext.OutputMerger.ResetTargets();
                        demo.DeviceContext.VertexShader.SetConstantBuffer(0, null);

                        // use post rendering
                        demo.RenderContext = postRenderContext;

                        // set textures
                        demo.DeviceContext.PixelShader.SetShaderResource(0, vanillaRenderContext.RenderTarget.ShaderResourceView);
                        demo.DeviceContext.PixelShader.SetShaderResource(1, distortionSRV);
                        demo.DeviceContext.PixelShader.SetShaderResource(2, noiseSRV);

                        // use plane model
                        var model = demo.ModelManager["plane2"];

                        // todo: make sure we have no z-buffer conflicts (might be hw dependent)
                        var camera = new NoCamera();
                        demo.Draw(model, camera);
                    }
                });
            }

            // dispose
            Disposer.Dispose(ref mainDisposer);
        }
    }
}