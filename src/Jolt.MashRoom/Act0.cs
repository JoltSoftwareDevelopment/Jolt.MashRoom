using System;
using Ignostic;
using Ignostic.Studio256.RenderApi;
using Jolt.Cuberick.Effects;
using Jolt.MashRoom.Effects;

namespace Jolt.MashRoom
{
    public class Act0 : IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Demo _demo;
        private Disposer _disposer;
        private IEffect _introEffect;
        private IEffect _rippleBallEffect;
        private IEffect _artificialEffect;
        private IEffect _greetingsEffect;
        private IEffect _fractalBallEffect;
        private IEffect _lightSpeedEffect;
        private IEffect _creditsEffect;
        private IEffect _metamonolithEffect;
        private IEffect _crystalBeaconEffect;
        private IEffect _splattEffect;
        private IEffect _origamiEffect;
        private IEffect _cubePulsEffect;
        private IEffect _gyroEffect;
        private IEffect _fluxEffect;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public Act0(Demo demo)
        {
            _demo = demo;
            _disposer = new Disposer();
        }


        public void Dispose()
        {
            Disposer.Dispose(ref _disposer);
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private Effect CreateEffect(string pixelShaderName, string textureNames)
        {
            return _disposer.Add(new Effect(_demo).Init(new EffectDescription
            {
                PixelShaderName = pixelShaderName + ".ps.cso",
                TextureNames = textureNames.Split(',')
            }));
        }


        public Act0 Init()
        {
            _greetingsEffect = _disposer.Add(new GreetingsEffect(_demo).Init(new EffectDescription
            {
                PixelShaderName = "ryk.frontierOfWires.ps.cso",
                TextureNames = "greetings.png".Split(',')
            }));
            _creditsEffect = _disposer.Add(new EndCreditsEffect(_demo).Init(new EffectDescription
            {
                PixelShaderName = "foxhuntd.mistRing.ps.cso",
                TextureNames = "endcredits.png".Split(',')
            }));
            _introEffect         = CreateEffect("aiekick.alienCavern",          "intro.png,mashup.png");
            _rippleBallEffect    = CreateEffect("bear.inchworm",                "");
            _fractalBallEffect   = CreateEffect("otavioGood.symmetricOrigins",  "");
            _lightSpeedEffect    = CreateEffect("srtuss.relentless",            "");
            _metamonolithEffect  = CreateEffect("ryk.metamonolith",             "");
            _crystalBeaconEffect = CreateEffect("ryk.crystalBeacon",            "");
            _splattEffect        = CreateEffect("netgrind.ngRay1",              "");
            _cubePulsEffect      = CreateEffect("skaven.cubePulse",             "");
            _fluxEffect          = CreateEffect("pauloFalcao.torusJourney",     "intro.png");

            return this;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void Update()
        {
        }

        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void Render()
        {
            var syncRow = new
            {
                Part = (Part)(int)(float)_demo.SyncManager.Data.Part
            };

            switch (syncRow.Part)
            {
                case Part.Silent:
                    break;
                case Part.Intro:
                    _introEffect.Render();
                    break;
                case Part.Slow:
                    _rippleBallEffect.Render();
                    break;
                case Part.Fast0:
                    _artificialEffect.Render();
                    break;
                case Part.Greetings:
                    _greetingsEffect.Render();
                    break;
                case Part.Fast1:
                    _fractalBallEffect.Render();
                    break;
                case Part.Fast2:
                    _lightSpeedEffect.Render();
                    break;
                case Part.Credits:
                    _creditsEffect.Render();
                    break;
                case Part.CrystalBeacon:
                    _crystalBeaconEffect.Render();
                    break;
                case Part.MetaMonolith:
                    _metamonolithEffect.Render();
                    break;
                case Part.Splatt:
                    _splattEffect.Render();
                    break;
                case Part.Origami:
                    _origamiEffect.Render();
                    break;
                case Part.CubePulse:
                    _cubePulsEffect.Render();
                    break;
                case Part.Gyro:
                    _gyroEffect.Render();
                    break;
                case Part.Flux:
                    _fluxEffect.Render();
                    break;
                default:
                    break;
            }
            //_origamiEffect.Render();
        }
        
        private enum Part
        {
            Silent = 0,
            Intro = 1,
            Slow = 2,
            Fast0 = 3,
            Greetings = 4,
            Fast1 = 5,
            Fast2 = 6,
            Credits = 7,
            CrystalBeacon = 8,
            MetaMonolith = 9,
            Splatt = 10,
            Origami = 11,
            CubePulse = 12,
            Gyro = 13,
            Flux = 14,
        }
    }
}
