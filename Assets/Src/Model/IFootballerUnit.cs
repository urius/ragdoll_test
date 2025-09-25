using Src.Data;

namespace Src.Model
{
    public interface IFootballerUnit
    {
        FootballerUnitData UnitData { get; }

        void SetupData(TeamKey team, int teamInnerIndex);
    }
}