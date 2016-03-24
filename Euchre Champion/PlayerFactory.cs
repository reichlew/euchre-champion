using System;
using System.Collections.Generic;

namespace EuchreChampion
{
    public class PlayerFactory
    {
        public List<Player> GetPlayers()
        {
            var players = new List<Player>();
            foreach (var position in Enum.GetValues(typeof(Position)))
            {
                players.Add(new Player((Position)position));
            }
            return players;
        }
    }
}