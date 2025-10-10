using Src.Providers;
using UnityEngine;

namespace Src.Model
{
    public interface IBallModel : IBallPositionProvider
    {
        void SetPosition(Vector3 position);
    }
}