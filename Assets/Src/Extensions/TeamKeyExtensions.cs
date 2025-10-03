using System;
using Src.Data;

namespace Src.Extensions
{
    public static class TeamKeyExtensions
    {
        public static TeamKey OppositeTeam(this TeamKey teamKey)
        {
            switch (teamKey)
            {
                case TeamKey.Red:
                    return TeamKey.Blue;
                case TeamKey.Blue:
                    return TeamKey.Red;
                case TeamKey.Undefined:
                    return TeamKey.Undefined;
                default:
                    throw new ArgumentOutOfRangeException(nameof(teamKey), teamKey, null);
            }
        }
    }
}