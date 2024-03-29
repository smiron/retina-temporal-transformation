﻿namespace WindowsFormsRetina.Htm
{
    public class HtmLateralSynapse
    {
        #region Fields

        private readonly double _connectedPermanence;

        #endregion

        #region Properties

        public HtmCell InputCell;

        public double Permanance;
        

        #endregion

        #region Methods

        public bool IsConnected()
        {
            return Permanance > _connectedPermanence;
        }

        #endregion

        #region Instance

        public HtmLateralSynapse(double connectedPermanence)
        {
            _connectedPermanence = connectedPermanence;
        }

        #endregion
    }
}