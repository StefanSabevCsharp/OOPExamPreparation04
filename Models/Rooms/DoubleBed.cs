using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models.Rooms
{
    public class DoubleBed : Room
    {
        private const int DefaultCapacity = 2;
        public DoubleBed()
            : base(DefaultCapacity)
        {
        }
    }
}
