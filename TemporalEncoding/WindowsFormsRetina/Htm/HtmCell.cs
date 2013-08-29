﻿using System;
using System.Collections.Generic;

namespace WindowsFormsRetina.Htm
{
    public enum HtmTime
    {
        Before = 1,
        Now = 2
    }

    public class HtmCell
    {
        private readonly HtmCellState _oldState;
        private readonly HtmCellState _newState;
        
        public HtmCell()
        {
            _oldState = new HtmCellState();
            _newState = new HtmCellState();
        }


        public HtmCellState GetByTime(HtmTime time)
        {
            switch (time)
            {
                case HtmTime.Before:
                    return _oldState;
                case HtmTime.Now:
                    return _newState;
                default:
                    throw new ArgumentOutOfRangeException("time");
            }
        }
    }

    public class HtmCellState
    {
        #region Properties


        public bool ActiveState;

        public bool PredictiveState;

        public IEnumerable<HtmSegment> Segments;

        #endregion

        #region Instance

        public HtmCellState(IEnumerable<HtmSegment> dendriteSegments)
        {
            Segments = dendriteSegments;
        }

        public HtmCellState()
        {
            Segments = new List<HtmSegment>();
        }

        #endregion
 
    }


}
