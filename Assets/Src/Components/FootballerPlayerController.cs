using UnityEngine;

namespace Src.Components
{
    public class FootballerPlayerController : MonoBehaviour
    {
        private FootballerUnit _presenter;

        private void Awake()
        {
            _presenter = GetComponent<FootballerUnit>();
        }

        private void Start()
        {
            //notify about this component for camera
        }
    }
}