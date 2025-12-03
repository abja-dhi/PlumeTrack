using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plume_Track
{
    public class ColormapItem(string name, Image preview)
    {
        public string Name { get; set; } = name;
        public Image Preview { get; set; } = preview;

        public override string ToString()
        {
            return Name; // fallback
        }
    }

}
