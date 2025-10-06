using UnityEngine;

namespace Src.Providers
{
    public interface IBallPositionProvider : IDynamicPositionProvider
    {
        Vector3 LinearVelocity { get; }
    }
}