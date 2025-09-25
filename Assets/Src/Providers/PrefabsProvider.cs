using Src.Components;
using UnityEngine;

namespace Src.Providers
{
    [CreateAssetMenu(fileName = "PrefabsProvider", menuName = "ScriptableObjects/PrefabsProvider")]
    public class PrefabsProvider : ScriptableObject, IPrefabsProvider
    {
        [SerializeField] private FootballerUnit _footballerPrefab;

        public FootballerUnit FootballerUnit => _footballerPrefab;
    }
}