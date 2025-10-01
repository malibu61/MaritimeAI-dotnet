using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaritimeAI.DtoLAyer.Dtos
{
    public class ShipsDto
    {
        public int Type { get; set; }
        public int Unknown1 { get; set; }
        public long MMSI { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double Course { get; set; }
    }
}
