using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Ignostic.Timing.Sync;
using Ignostic.Timing.Sync.Commands;

namespace Ignostic.Timing.Sync
{
    public class SyncManager : ICommandHandler
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private double              _dtime;
        private ISyncAdapter        _syncAdapter;
        private SyncFileAdapter     _fileAdapter;
        private int                 _rowIndex;
        

        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public TrackManager         TrackManager    { get; set; }
        public ITimerDevice         TimerDevice     { get; set; }
        public double               RowsPerSecond   { get; private set; }
        public dynamic              Data            { get; private set; }
        public int                  RowIndex        { get { return ConvertTimeToIntegerRow(_dtime); } }
        public bool                 IsRecording     { get; set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SyncManager()
        {
            IsRecording = true;
            Data = new SyncData(this);
            TrackManager = new TrackManager();
            _fileAdapter = new SyncFileAdapter(TrackManager);
        }

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public float ConvertTimeToRow(double dtime)
        {
            return (float)(dtime * RowsPerSecond);
        }


        public int ConvertTimeToIntegerRow(double dtime)
        {
            return (int)Math.Floor(dtime * RowsPerSecond + 0.5);
        }

        
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SyncManager Init(bool useTrackerMode, double bpm, int rowsPerBeat)
        {
            if (bpm < 1)
                throw new ArgumentException("bmp cannot be less than 1.0");
            if (rowsPerBeat == 0)
                throw new ArgumentException("rowsPerBeat cannot be 0");

            _syncAdapter = useTrackerMode
                ? (ISyncAdapter)new SyncTrackerAdapter(this) 
                : _fileAdapter;
            RowsPerSecond = bpm / 60.0 * rowsPerBeat;
            return this;
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public float GetValue(string trackName)
        {
            var track = TrackManager.GetTrack(trackName);
            if (track == null)
            {
                track = _syncAdapter.RequestTrack(trackName);
                TrackManager.AddTrack(track);
            }

            //
            var drow = _dtime * RowsPerSecond;
            var row = (float)(0.001 * Math.Floor(1000 * drow + 0.5));
            var value = track.GetValue(row);
            return value;
        }


        public void Update(double dtime)
        {
            _dtime = dtime;
            var rowIndex = ConvertTimeToIntegerRow(dtime);
            if (TimerDevice.IsPlaying && _rowIndex != rowIndex)
            {
                _rowIndex = rowIndex;
                _syncAdapter.SetRowIndex(rowIndex);
            }

            _syncAdapter.Update();
        }


        /****************************************************************************************************
         * ISyncCommandHandler implementation
         ****************************************************************************************************/
        void ICommandHandler.AddValue(int trackIndex, TrackEntry trackEntry)
        {
            TrackManager.AddValue(trackIndex, trackEntry);
        }


        void ICommandHandler.DeleteValue(int trackIndex, int rowIndex)
        {
            TrackManager.RemoveValue(trackIndex, rowIndex);
        }

        
        void ICommandHandler.SetRowIndex(int rowIndex)
        {
            _rowIndex = rowIndex;
            TimerDevice.Time = rowIndex / RowsPerSecond;
        }

        
        void ICommandHandler.SetPause(bool shouldStop)
        {
            if (shouldStop)
            {
                TimerDevice.StopPlaying();
            }
            else
            {
                TimerDevice.StartPlaying();
            }
        }

        
        void ICommandHandler.Export()
        {
            _fileAdapter.SaveAllTracks();
        }
    }
}
