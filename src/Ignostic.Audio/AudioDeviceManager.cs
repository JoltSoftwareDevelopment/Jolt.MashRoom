using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignostic.Audio
{
    public enum AudioDeviceType
    {
        Unknown = 0,
        Silent = 1,
        Bass = 2,
    }

    public class AudioDeviceManager
    {
        public IAudioDevice CreateDevice(AudioDeviceType audioDeviceType)
        {
            switch (audioDeviceType)
            {
                case AudioDeviceType.Silent:
                    return new SilentAudioDevice();
                case AudioDeviceType.Bass:
                    return new BassAudioDevice();
                default:
                    throw new AudioDeviceException("unknown audio device type");
            }
        }
    }
}
