using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plume_Track
{
    public class ComboBoxItem(string text, string id)
    {
        public string Text { get; set; } = text;
        public string ID { get; set; } = id;

        public override string ToString()
        {
            return Text; // what shows in the dropdown
        }
    }
}
