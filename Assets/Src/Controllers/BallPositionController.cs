using Src.Model;
using Src.Providers;
using VContainer.Unity;

namespace Src.Controllers
{
    public class BallPositionController : IFixedTickable
    {
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IFieldBoundsPositionProvider _boundsPositionProvider;
        private readonly IBallModel _ballModel;

        public BallPositionController(
            IBallPositionProvider ballPositionProvider,
            IFieldBoundsPositionProvider boundsPositionProvider,
            IBallModel ballModel)
        {
            _ballPositionProvider = ballPositionProvider;
            _boundsPositionProvider = boundsPositionProvider;
            _ballModel = ballModel;
        }

        public void FixedTick()
        {
            if (_ballPositionProvider.Position.y < 0)
            {
                var position = _boundsPositionProvider.ClampByBorder(_ballPositionProvider.Position, 10);
                position.y = 10;
                
                _ballModel.SetPosition(position);
            }
        }
    }
}