using System;
using System.Collections.Generic;
using System.IO;

namespace Ignostic.Timing.Sync
{
    public partial class SyncTrack
    {
        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        private static readonly TrackEntryRowComparer _trackEntryRowComparer = new TrackEntryRowComparer();


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public string           Name    { get; private set; }
        public List<TrackEntry> Entries { get; private set; }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public SyncTrack(string name, List<TrackEntry> entries)
        {
            Name = name;
            Entries = entries;
        }

        
        public SyncTrack(string name)
            : this(name, new List<TrackEntry>())
        {
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public float GetValue(float rowIndex)
        {
            // found entry is garanteed to have .RowIndex >= floor(rowIndex)
            var entryIndex = FindClosestEntryIndex((int)Math.Floor(rowIndex));
            if (entryIndex < 0)
                entryIndex = ~entryIndex - 1;

            // before first entry
            if (entryIndex == -1)
                return 0F;

            // at or after last entry: return last value
            if (entryIndex >= Entries.Count - 1)
                return Entries[Entries.Count  -1].Value;

            // interpolation
            var entry0 = Entries[entryIndex];
            var entry1 = Entries[entryIndex + 1];
            var t = TLerp(entry0.RowIndex, entry1.RowIndex, rowIndex);
            switch (entry0.InterpolationType)
            {
                case InterpolationType.None:        return entry0.Value;
                case InterpolationType.Lerp:        return Lerp(entry0.Value, entry1.Value, t);
                case InterpolationType.Smoothstep:  return Smoothstep(entry0.Value, entry1.Value, t);
                case InterpolationType.Ramp:        return Ramp(entry0.Value, entry1.Value, t);
                default:
                    throw new SyncException(string.Format("Unknown interpolation type: {0}", entry0.InterpolationType));
            }
        }

        // returns the exact index if there is one.
        // otherwise returns the negative value of the closes larger index.
        private int FindClosestEntryIndex(int rowIndex)
        {
            var searchEntry = new TrackEntry { RowIndex = rowIndex };
            var entryIndex = Entries.BinarySearch(searchEntry, _trackEntryRowComparer);
            return entryIndex;
        }


        // https://en.wikipedia.org/wiki/Linear_interpolation
        private static float Lerp(float min, float max, float t)
        {
            return (1 - t) * min + t * max;
        }

        // https://en.wikipedia.org/wiki/Smoothstep
        private static float Smoothstep(float min, float max, float t)
        {
            return Lerp(min, max, t * t * (3 - 2 * t));
        }

        // 
        private static float Ramp(float min, float max, float t)
        {
            return Lerp(min, max, t * t);
        }

        //
        private static float TLerp(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        public TrackEntry this[int rowIndex]
        {
            set 
            {
                var entryIndex = FindClosestEntryIndex(rowIndex);
                if (entryIndex >= 0)
                {
                    if (value == null)
                        Entries.RemoveAt(entryIndex);
                    else
                        Entries[entryIndex] = value;
                }
                else if (value != null)
                {
                    Entries.Insert(~entryIndex, value);
                }
            }
        }


        /****************************************************************************************************
         * 
         ****************************************************************************************************/
        class TrackEntryRowComparer : IComparer<TrackEntry>
        {
            public int Compare(TrackEntry x, TrackEntry y)
            {
                return x.RowIndex.CompareTo(y.RowIndex);
            }
        }
    }
}
