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
    public class SyncTrackerAdapter : ISyncAdapter
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private const string _adapterHandshake = "hello, synctracker!";
        private const string _trackerHandshake = "hello, demo!";
        private const string _trackerHost = "localhost";
        private const int _trackerPort = 1338;
        private readonly TcpClient _tcpClient;
        private readonly CommandSerializer _serializer;
        private readonly CommandDispatcher _dispatcher;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SyncTrackerAdapter(ICommandHandler commandHandler)
        {
            _tcpClient = new TcpClient(_trackerHost, _trackerPort)
            {
                NoDelay = true
            };
            _serializer = new CommandSerializer(_tcpClient.GetStream());
            _dispatcher = new CommandDispatcher(commandHandler);
            Handshake();
        }


        public void Dispose()
        {
            _tcpClient.Close();
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void Update()
        {
            // handle incoming commands
            while (_tcpClient.Available > 0)
            {
                _dispatcher.DispatchCommand(_serializer.DeserializeCommand());
            }
        }


        public void SetRowIndex(int rowIndex)
        {
            // send new rowIndex
            _serializer.SerializeCommand(new SetRowIndexCommand(rowIndex));
            _serializer.Flush();
        }


        public SyncTrack RequestTrack(string trackName)
        {
            // send track request
            _serializer.SerializeCommand(new RequestTrackCommand(trackName));
            _serializer.Flush();
            
            // create new SyncTrack (which will be populated through Add Commands)
            return new SyncTrack(trackName);
        }


        private void Handshake()
        {
            var stream = _tcpClient.GetStream();
            using (var writer = new StreamWriter(stream, Encoding.ASCII, 8192, true))
            using (var reader = new StreamReader(stream, Encoding.ASCII, false, 8192, true))
            {
                // write adapter handshake
                writer.Write(_adapterHandshake);
                writer.Flush();

                // read tracker handshake
                var responseBuffer = new char[_trackerHandshake.Length];
                var responseLength = reader.Read(responseBuffer, 0, responseBuffer.Length);
                var responseString = new string(responseBuffer);

                // verify tracker handshake
                if (responseString != _trackerHandshake)
                {
                    throw new Exception(string.Format("Unexpected sync tracker response: {0}", responseString));
                }
            }
        }
    }
}
