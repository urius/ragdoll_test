using System;
using System.Collections.Generic;
using Src.Model;

namespace Src.Providers
{
    public class GameUnitsProvider : IGameUnitsProvider, IGameUnitsHolder
    {
        public event Action<IFootballerUnit> UnitAdded;
        
        private readonly List<IFootballerUnit> _footballerUnits = new();
            
        public IEnumerable<IFootballerUnit> Footballers => _footballerUnits;
        
        public void AddFootballer(IFootballerUnit unit)
        {
            _footballerUnits.Add(unit);
            
            UnitAdded?.Invoke(unit);
        }
    }

    public interface IGameUnitsProvider
    {
        event Action<IFootballerUnit> UnitAdded;
            
        IEnumerable<IFootballerUnit> Footballers { get; }
    }

    public interface IGameUnitsHolder
    {
        void AddFootballer(IFootballerUnit unit);
    }
}