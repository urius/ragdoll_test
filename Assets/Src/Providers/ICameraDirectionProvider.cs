using UnityEngine;

namespace Src.Providers
{
    public interface ICameraDirectionProvider
    {
        Vector3 Forward { get; }
        Vector3 Right { get; }
    }
}