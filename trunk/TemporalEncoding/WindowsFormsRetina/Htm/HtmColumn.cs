using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WindowsFormsRetina.Htm
{
    public class HtmColumn
    {
        #region Fields

        private readonly List<bool> _afterInhibationActivationHistory;
        private readonly List<bool> _beforeInhibationActivationHistory;
        private readonly int _historySize;

        #endregion

        #region Properties

        public int X;

        public int Y;

        public IEnumerable<HtmCell> Cells;

        public IEnumerable<HtmColumn> Neighbors;

        public IEnumerable<HtmForwardSynapse> PotentialSynapses;

        public double Boost;

        public double Overlap;

        public double MinimalDutyCycle;

        public double ActiveDutyCycle;

        public double OverlapDutyCycle;

        #endregion

        #region Methods

        public IEnumerable<HtmForwardSynapse> GetConnectedSynapses()
        {
            return PotentialSynapses.Where(synapse => synapse.Permanance > HtmParameters.ConnectedPermanence).ToList();
        }

        public void AddActivationToHistory(bool state)
        {
            _afterInhibationActivationHistory.Insert(0, state);
            if (_afterInhibationActivationHistory.Count > _historySize)
            {
                _afterInhibationActivationHistory.RemoveAt(_historySize);
            }
        }

        public void AddOverlapToHistory(bool state)
        {
            _beforeInhibationActivationHistory.Insert(0, state);
            if (_beforeInhibationActivationHistory.Count > _historySize)
            {
                _beforeInhibationActivationHistory.RemoveAt(_historySize);
            }
        }

        public void UpdateColumnBoost()
        {
            MinimalDutyCycle = 0.01 * (!Neighbors.Any() ? 1 : Neighbors.Max(n => n.ActiveDutyCycle));

            ActiveDutyCycle = (double)_afterInhibationActivationHistory.Count(state => state) / _afterInhibationActivationHistory.Count();

            if (ActiveDutyCycle > MinimalDutyCycle)
            {
                Boost = 1.0;
            }
            else
            {
                Boost += MinimalDutyCycle;
            }
        }

        public void UpdateSynapsePermanance(double connectedPermanance)
        {
            OverlapDutyCycle = (double)_beforeInhibationActivationHistory.Count(state => state) / _beforeInhibationActivationHistory.Count();

            if (OverlapDutyCycle < MinimalDutyCycle)
            {
                foreach (var synapse in PotentialSynapses)
                {
                    synapse.Permanance += connectedPermanance;
                }
            }
        }

        #endregion

        #region Instance

        public HtmColumn(int historySize = 1000)
        {
            _afterInhibationActivationHistory = new List<bool>();
            _beforeInhibationActivationHistory = new List<bool>();
            
            Cells = new BindingList<HtmCell>();
            PotentialSynapses = new List<HtmForwardSynapse>();
            
            _historySize = historySize;
            Boost = 1;
        }

        #endregion


        public override string ToString()
        {
            return X + "-" + Y + "-" + Overlap;
        }
    }
}
