using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Ignostic.Timing.Sync
{
    public class SyncFileAdapter : ISyncAdapter
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private readonly string _rootPath;
        private readonly TrackManager _trackManager;


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SyncFileAdapter(TrackManager trackManager)
        {
            _rootPath = Path.Combine("resources", "sync");
            _trackManager = trackManager;
        }


        public void Dispose()
        {
        }

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public void Update()
        {
        }


        public void SetRowIndex(int rowIndex)
        {
        }


        public SyncTrack RequestTrack(string trackName)
        {
            return LoadTrack(trackName);
        }


        public SyncTrack LoadTrack(string trackName)
        {
            var path = GetTrackPath(trackName);
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var serializer = new CommandSerializer(stream))
            {
                var entries = serializer.DeserializeTrackEntries();
                var track = new SyncTrack(trackName, entries);
                return track;
            }
        }


        public void SaveTrack(SyncTrack track)
        {
            var path = GetTrackPath(track.Name);
            using (var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var serializer = new CommandSerializer(stream))
            {
                serializer.SerializeTrackEntries(track.Entries);
            }
        }

        
        public void SaveAllTracks()
        {
            foreach (var track in _trackManager.Tracks)
            {
                SaveTrack(track);
            }
        }

        
        private string GetTrackPath(string trackName)
        {
            var fileName = string.Format("sync_{0}.track", trackName);
            var filePath = Path.Combine(_rootPath, fileName);
            return filePath;
        }
    }
}
