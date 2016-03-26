using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EuchreChampion
{
    public static class PositionHelper
    {
        public static int Partner(int index)
        {
            switch (index)
            {
                case 0:
                    return 2;
                case 1:
                    return 3;
                case 2:
                    return 0;
                case 3:
                    return 1;
                default:
                    throw new Exception($"{index} is not a valid player index.");
            }
        }

        public static bool IsUserTeam(Position position)
        {
            return position == Position.North || position == Position.South;
        }
    }
}
