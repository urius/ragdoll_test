using Src.Model;
using Src.Providers;
using VContainer.Unity;

namespace Src.Controllers
{
    public class BallPositionController : IFixedTickable
    {
        private readonly IBallPositionProvider _ballPositionProvider;
        private readonly IFieldBorderPositionProvider _borderPositionProvider;
        private readonly IBallModel _ballModel;

        public BallPositionController(
            IBallPositionProvider ballPositionProvider,
            IFieldBorderPositionProvider borderPositionProvider,
            IBallModel ballModel)
        {
            _ballPositionProvider = ballPositionProvider;
            _borderPositionProvider = borderPositionProvider;
            _ballModel = ballModel;
        }

        public void FixedTick()
        {
            if (_ballPositionProvider.Position.y < 0)
            {
                var position = _borderPositionProvider.ClampByBorder(_ballPositionProvider.Position, 10);
                position.y = 10;
                
                _ballModel.SetPosition(position);
            }
        }
    }
}