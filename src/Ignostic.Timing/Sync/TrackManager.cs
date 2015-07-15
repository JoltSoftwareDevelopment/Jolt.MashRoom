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
    public class TrackManager
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public List<SyncTrack> Tracks { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public TrackManager()
        {
            Tracks = new List<SyncTrack>();
        }


        public SyncTrack GetTrack(string trackName)
        {
            return Tracks.FirstOrDefault(t => t.Name == trackName);
        }


        public void AddTrack(SyncTrack track)
        {
            Tracks.Add(track);
        }

        
        public void AddValue(int trackIndex, TrackEntry trackEntry)
        {
            Tracks[trackIndex][trackEntry.RowIndex] = trackEntry;
        }


        public void RemoveValue(int trackIndex, int rowIndex)
        {
            Tracks[trackIndex][rowIndex] = null;
        }
    }
}
