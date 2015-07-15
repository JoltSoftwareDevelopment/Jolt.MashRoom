using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Ignostic.Timing.Sync.Commands;

namespace Ignostic.Timing.Sync
{
    public class CommandSerializer : IDisposable
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private readonly Encoding _encoding;
        private readonly Stream _stream;
        private BinaryReader _reader;
        private BinaryWriter _writer;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public CommandSerializer(Stream stream)
        {
            _encoding = Encoding.ASCII;
            _stream = stream;
        }


        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            if (_writer != null)
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }


        /****************************************************************************************************
         *
         ****************************************************************************************************/
        public void Flush()
        {
            Writer.Flush();
        }


        private BinaryReader Reader
        {
            get
            {
                if (_reader == null)
                    _reader = new BinaryReader(_stream, _encoding, true);
                return _reader;
            }
        }

        
        private BinaryWriter Writer
        {
            get
            {
                if (_writer == null)
                    _writer = new BinaryWriter(_stream, _encoding, true);
                return _writer;
            }
        }

        /****************************************************************************************************
         * Deserialization
         ****************************************************************************************************/
        public ISyncCommand DeserializeCommand()
        {
            var commandId = (CommandId)ReadByte();
            switch (commandId)
            {
                case CommandId.AddValue:    return new AddEntryCommand(ReadInt32(), DeserializeTrackEntry());
                case CommandId.DeleteValue: return new RemoveEntryCommand(ReadInt32(), ReadInt32());
                case CommandId.SetRowIndex: return new SetRowIndexCommand(ReadInt32());
                case CommandId.SetPause:    return new SetPauseCommand(ReadBool());
                case CommandId.Export:      return new ExportCommand();
                default:
                    throw new SyncException(string.Format("Unable to deserialize sync command: {0} ", commandId));
            }
        }

        
        private TrackEntry DeserializeTrackEntry()
        {
            return new TrackEntry
            {
                RowIndex          = ReadInt32(),
                Value             = ReadSingle(),
                InterpolationType = (InterpolationType)ReadByte(),
            };
        }

        
        public List<TrackEntry> DeserializeTrackEntries()
        {
            var entryCount = ReadInt32();
            var trackEntries = Enumerable
                .Range(0, entryCount)
                .Select(i => DeserializeTrackEntry())
                .ToList();
            return trackEntries;
        }


        /****************************************************************************************************
         * Serialization
         ****************************************************************************************************/
        public void SerializeCommand(ISyncCommand command)
        {
            switch (command.Id)
            {
                case CommandId.RequestTrack:
                {
                    Write((byte)command.Id);
                    Write((command as RequestTrackCommand).TrackName);
                    break;
                }
                case CommandId.SetRowIndex:
                {
                    Write((byte)command.Id);
                    Write((command as SetRowIndexCommand).RowIndex);
                    break;
                }
                default:
                    throw new SyncException(string.Format("Unable to serialize sync command: {0} ", command.Id));
            }
        }


        public void SerializeTrackEntries(List<TrackEntry> trackEntries)
        {
            Write(trackEntries.Count);
            foreach (var trackEntry in trackEntries)
            {
                SerializeTrackEntry(trackEntry);
            }
        }

        
        public void SerializeTrackEntry(TrackEntry trackEntry)
        {
            Write(trackEntry.RowIndex);
            Write(trackEntry.Value);
            Write((byte)trackEntry.InterpolationType);
        }


        /****************************************************************************************************
         * Big endian read/write
         ****************************************************************************************************/
        private bool ReadBool()
        {
            return Reader.ReadBoolean();
        }


        private int ReadByte()
        {
            return Reader.ReadByte();
        }


        private int ReadInt32()
        {
            return IPAddress.NetworkToHostOrder(Reader.ReadInt32());
        }


        private float ReadSingle()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(ReadInt32()), 0);
        }


        private void Write(bool value)
        {
            Writer.Write(value);
        }


        private void Write(byte value)
        {
            Writer.Write(value);
        }


        private void Write(int value)
        {
            Writer.Write(IPAddress.HostToNetworkOrder(value));
        }


        private void Write(float value)
        {
            Writer.Write(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(value), 0)));
        }

        
        private void Write(string value)
        {
            var bytes = _encoding.GetBytes(value);
            Write(bytes.Length);
            Writer.Write(bytes);
        }
    }
}
