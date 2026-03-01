using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReFMGame.Network
{
    public class GameState
    {
        public int Time { get; set; }
        public int Power { get; set; }
        public DoorState Right {  get; set; }
        public DoorState Left { get; set; }
        public CameraState Camera { get; set; }
    }
    public class DoorState
    {
        public bool Blocked { get; set; }
        public bool Door { get; set; }
        public bool Light { get; set; }
    }
    public class CameraState
    {
        public bool Active { get; set; }
        public bool Garble { get; set; }
    }
}
