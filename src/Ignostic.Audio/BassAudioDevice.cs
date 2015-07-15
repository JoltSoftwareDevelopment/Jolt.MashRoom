using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using Ignostic.Timing;
using Un4seen.Bass;


namespace Ignostic.Audio
{
    internal class BassAudioDevice : IAudioDevice, IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private bool                _isInitialized;
        private int                 _streamHandle;
        private IntPtr              _streamPtr = IntPtr.Zero;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public bool IsPlaying { get; private set; }
        public double Bpm { get; set; }

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public BassAudioDevice()
        {
        }


        public void Init()
        {
            _isInitialized = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }


        public void Dispose()
        {
            // free the stream
            if (_streamHandle != 0)
            {
                bool freeStreamSucceded = Bass.BASS_StreamFree(_streamHandle);
                if (_streamPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_streamPtr);
                }
            }

            // free BASS
            if (_isInitialized)
            {
                bool freeBassSucceded = Bass.BASS_Free();
                if (!freeBassSucceded)
                {
                    // TODO
                    throw new NotImplementedException();
                }
            }
        }       
        
        
        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public double PlayPosition 
        {
            get 
            {
                var bytePosition = Bass.BASS_ChannelGetPosition(_streamHandle, BASSMode.BASS_POS_BYTES);
                var timePosition = Bass.BASS_ChannelBytes2Seconds(_streamHandle, bytePosition);
                return timePosition;
            }
            set
            {
                Bass.BASS_ChannelSetPosition(_streamHandle, value);
            }
        }


        public double Length
        {
            get 
            {
                var byteLength = Bass.BASS_ChannelGetLength(_streamHandle, BASSMode.BASS_POS_BYTES);
                var timeLength = Bass.BASS_ChannelBytes2Seconds(_streamHandle, byteLength);
                return timeLength;
            }
        }


        public double Volume 
        {
            get { return Bass.BASS_GetVolume(); }
            set { Bass.BASS_SetVolume((float)value); }
        }



        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void Load(Stream stream)
        {
            _streamPtr = Marshal.AllocHGlobal((int)stream.Length);

            var offset = 0;
            var buffer = new byte[stream.Length];
            while (offset < buffer.Length)
            {
                offset += stream.Read(buffer, offset, buffer.Length - offset);
            }
            Marshal.Copy(buffer, 0, _streamPtr, buffer.Length);

            _streamHandle = Bass.BASS_StreamCreateFile(_streamPtr, 0L, stream.Length, BASSFlag.BASS_DEFAULT);
            if (_streamHandle == 0)
            {
                var bassErrorCode = Bass.BASS_ErrorGetCode();
                throw new ApplicationException("Failed to create audio stream. BASS error code: " + bassErrorCode);
            }
        }
        
        
        #warning loop parameter not supported
        public void StartPlaying()
        {
            if (_streamHandle == 0)
            {
                throw new ApplicationException("No audio stream selected");
            }
            IsPlaying = Bass.BASS_ChannelPlay(_streamHandle, false);
            #warning Bass.BASS_ChannelPlay returnerar ibland false, (error BASS_ERROR_BUFLOST) bl.a. när jag pluggat ur hörlurarna
        }

        
        public void StopPlaying()
        {
            // there is not stream to stop
            if (_streamHandle == 0)
            {
                return;
            }

            #warning use the return value
            var stopSucceded = Bass.BASS_ChannelStop(_streamHandle);
            IsPlaying = false;
        }


        /****************************************************************************************************
         * ITimerDevice implementation
         ****************************************************************************************************/
        double ITimerDevice.Time
        {
            get { return PlayPosition; }
            set { PlayPosition = value; }
        }



        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        [Conditional("DEBUG")]
        [Obsolete]
        public void Load(string fileName)
        {
            var length = new FileInfo(fileName).Length;
            _streamHandle = Bass.BASS_StreamCreateFile(fileName, 0L, length, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_LOOP);
            if (_streamHandle == 0)
            {
                var bassErrorCode = Bass.BASS_ErrorGetCode();
                throw new ApplicationException("Failed to create audio stream. BASS error code: " + bassErrorCode);
            }
        }

    
    }
}
