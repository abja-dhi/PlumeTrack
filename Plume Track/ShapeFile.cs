using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plume_Track
{
    public class ShapeFile(string name, string path, string kind)
    {
        public string Name { get; set; } = name;
        public string Path { get; set; } = path;
        public string Kind { get; set; } = kind;
        // Polygon specific properties
        public string PolyEdgeColor { get; set; } = "#000000"; // default polygon edge color
        public string PolyLineWidth { get; set; } = "0.8"; // default polygon line width
        public string PolyFaceColor { get; set; } = "#000000"; // default polygon face color
        public string PolyAlpha { get; set; } = "1.0"; // default polygon transparency
        public string PolyLabelText { get; set; } = ""; // default label text
        public string PolyLabelFontSize { get; set; } = "8"; // default label font size
        public string PolyLabelColor { get; set; } = "#000000"; // default label color
        public string PolyLabelHA { get; set; } = "Left"; // default label horizontal alignment
        public string PolyLabelVA { get; set; } = "Center"; // default label vertical alignment
        public string PolyLabelOffsetPointsX { get; set; } = "0"; // default label offset in points (X)
        public string PolyLabelOffsetPointsY { get; set; } = "0"; // default label offset in points (Y)
        public string PolyLabelOffsetDataX { get; set; } = "0"; // default label offset in data units (X)
        public string PolyLabelOffsetDataY { get; set; } = "0"; // default label offset in data units (Y)

        // Line specific properties
        public string LineColor { get; set; } = "#000000"; // default line color
        public string LineLineWidth { get; set; } = "1.0"; // default line width
        public string LineAlpha { get; set; } = "1.0"; // default line transparency
        public string LineLabelText { get; set; } = ""; // default label text
        public string LineLabelFontSize { get; set; } = "8"; // default label font size
        public string LineLabelColor { get; set; } = "#000000"; // default label color
        public string LineLabelHA { get; set; } = "Left"; // default label horizontal alignment
        public string LineLabelVA { get; set; } = "Center"; // default label vertical alignment
        public string LineLabelOffsetPointsX { get; set; } = "0"; // default label offset in points (X)
        public string LineLabelOffsetPointsY { get; set; } = "0"; // default label offset in points (Y)
        public string LineLabelOffsetDataX { get; set; } = "0"; // default label offset in data units (X)
        public string LineLabelOffsetDataY { get; set; } = "0"; // default label offset in data units (Y)
        // Point specific properties 
        public string PointColor { get; set; } = "#000000"; // default point color
        public string PointMarker { get; set; } = "o"; // default point marker
        public string PointMarkerSize { get; set; } = "12"; // default point marker size
        public string PointAlpha { get; set; } = "1.0"; // default point transparency
        public string PointLabelText { get; set; } = ""; // default label text
        public string PointLabelFontSize { get; set; } = "8"; // default label font size
        public string PointLabelColor { get; set; } = "#000000"; // default label color
        public string PointLabelHA { get; set; } = "Left"; // default label horizontal alignment
        public string PointLabelVA { get; set; } = "Center"; // default label vertical alignment
        public string PointLabelOffsetPointsX { get; set; } = "0"; // default label offset in points (X)
        public string PointLabelOffsetPointsY { get; set; } = "0"; // default label offset in points (Y)
        public string PointLabelOffsetDataX { get; set; } = "0"; // default label offset in data units (X)
        public string PointLabelOffsetDataY { get; set; } = "0"; // default label offset in data units (Y)

        public override string ToString()
        {
            return Name; // fallback
        }
    }
}
