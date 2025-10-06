using Src.Model;

namespace Src.Controllers.RolesBehaviourProcessors
{
    public interface IRoleBehaviourProcessor
    {
        void Process(IFootballerUnit footballer);
    }
}