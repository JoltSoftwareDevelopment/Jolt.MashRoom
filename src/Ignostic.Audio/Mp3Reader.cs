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
    public class AudioStream : Stream
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
        public AudioStream()
        {
        }


        public void Init()
        {
            throw new Exception("license for bass not available");

            _isInitialized = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_MONO, IntPtr.Zero);
        }


        protected override void Dispose(bool disposing)
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
                    //throw new NotImplementedException();
                    // dont throw inside Dispose
                }
            }
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

            _streamHandle = Bass.BASS_StreamCreateFile(
                _streamPtr, 
                0L, 
                stream.Length, 
                BASSFlag.BASS_MUSIC_DECODE |
                BASSFlag.BASS_MUSIC_FLOAT | 
                BASSFlag.BASS_MUSIC_PRESCAN
            );
            if (_streamHandle == 0)
            {
                var bassErrorCode = Bass.BASS_ErrorGetCode();
                throw new ApplicationException("Failed to create audio stream. BASS error code: " + bassErrorCode);
            }
        }


        public long GetPositionAtTime(double seconds)
        {
            return Bass.BASS_ChannelSeconds2Bytes(_streamHandle, seconds);
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            // todo: read fft directly from bass

            // todo: perhaps use info data
            //var info = Bass.BASS_ChannelGetInfo(_streamHandle);
            //var buffer = new short[1024*1024];
            var tempBuffer = (offset == 0) ? (buffer) : (new byte[count]);
            var actualReadCount = Bass.BASS_ChannelGetData(_streamHandle, tempBuffer, count);
            if (actualReadCount != -1)
            {
                if (offset != 0)
                {
                    Array.Copy(tempBuffer, 0, buffer, offset, actualReadCount);
                }
                return actualReadCount;
            }

            var errorCode = Bass.BASS_ErrorGetCode();
            if (errorCode == BASSError.BASS_ERROR_ENDED)
                return 0;
                
            throw new AudioStreamException(errorCode);
            //var hasAnyValue = buffer.Count(x => x != 0);
            //Buffer.BlockCopy(
            //MathNet.Numerics.IntegralTransforms.Fourier.Forward(
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get
            {
                var value = Bass.BASS_ChannelGetLength(_streamHandle, BASSMode.BASS_POS_BYTES);
                // todo: error handling
                return value;
            }
        }

        public override long Position
        {
            get
            {
                // todo: error handling
                return Bass.BASS_ChannelGetPosition(_streamHandle, BASSMode.BASS_POS_BYTES);
            }
            set
            {
                var succeded = Bass.BASS_ChannelSetPosition(_streamHandle, value, BASSMode.BASS_POS_BYTES);
                if (!succeded)
                    throw new AudioStreamException(Bass.BASS_ErrorGetCode());
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
