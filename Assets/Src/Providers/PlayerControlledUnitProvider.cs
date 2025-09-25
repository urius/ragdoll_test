using System;
using Src.Model;

namespace Src.Providers
{
    public class PlayerControlledUnitProvider
    {
        public event Action<IFootballerUnit> TargetUnitChanged; 
        public IFootballerUnit TargetUnit { get; private set; }

        public void SetTargetUnit(IFootballerUnit unit)
        {
            if (TargetUnit == unit) return;
            
            TargetUnit = unit;
            TargetUnitChanged?.Invoke(TargetUnit);
        }

    }
}