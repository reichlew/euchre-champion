using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EuchreChampion
{
    public class Hand
    {
        public bool UserCalledTrump { get; set; }
        public bool IsLoner { get; set; }        
        public Suit LeadSuit { get; set; }
        public Suit Trump { get; set; }
    }
}
