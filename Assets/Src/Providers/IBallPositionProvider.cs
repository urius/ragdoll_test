using UnityEngine;

namespace Src.Providers
{
    public interface IBallPositionProvider
    {
        Vector3 Position { get; }
    }
}