using UnityEngine;

namespace Src.Providers
{
    public interface IDynamicPositionProvider
    {
        Vector3 Position { get; }
        Vector3 PositionProjected { get; }
    }
}