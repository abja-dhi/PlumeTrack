using NetTopologySuite.IO;
using Plume_Track;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Plume_Track
{
    public partial class MapOptions : Form
    {
        bool isSaved;
        public _ClassConfigurationManager _project = new();
        public XmlElement? mapSettings;
        public XmlElement? mapSettings2D;
        public XmlElement? mapSettings3D;
        public XmlElement? mapShapefiles;
        public int currentShapefileIndex;
        public Dictionary<string, XmlElement> shapefileSettings = [];
        public List<TextBox> shpPathTextBoxes;
        public List<TextBox> shpTypeTextBoxes;
        public List<Panel> shpColorPanels;
        public List<Button> shpColorButtons;
        public List<TextBox> shpSizeTextBoxes;

        private readonly ColormapComboBox combo2Dcmap = new(_Globals.CMapsPath, "combo2Dcmap");
        private readonly ColormapComboBox combo3Dcmap = new(_Globals.CMapsPath, "combo3Dcmap");

        private readonly TableLayoutPanel tableShpPoint = new();
        private readonly Label lblShpPointPath = _Utils.CreateLabel("lblShpPointPath", "Path");
        private readonly TextBox txtShpPointPath = _Utils.CreateTextBox("txtShpPointPath", "", false);
        private readonly Label lblShpPointType = _Utils.CreateLabel("lblShpPointType", "Shapefile Type");
        private readonly TextBox txtShpPointType = _Utils.CreateTextBox("txtShpPointType", "", false);
        private readonly Label lblShpPointColor = _Utils.CreateLabel("lblShpPointColor", "Point Color");
        private readonly Panel pnlShpPointColor = _Utils.CreatePanel("pnlShpPointColor", Color.White);
        private readonly Button btnShpPointColor = _Utils.CreateButton("btnShpPointColor", "...");
        private readonly Label lblShpPointMarker = _Utils.CreateLabel("lblShpPointMarker", "Marker");
        private readonly ComboBox comboShpPointMarker = _Utils.CreateComboBox("comboShpPointMarker", ["o", "*", "^", "x"], 0);
        private readonly Label lblShpPointMarkerSize = _Utils.CreateLabel("lblShpPointMarkerSize", "Marker Size");
        private readonly NumericUpDown numShpPointMarkerSize = _Utils.CreateNumericUpDown("numShpPointMarkerSize", 1, 30, 12);
        private readonly Label lblShpPointMarkerAlpha = _Utils.CreateLabel("lblShpPointMarkerAlpha", "Transparency");
        private readonly NumericUpDown numShpPointMarkerAlpha = _Utils.CreateNumericUpDown("numShpPointMarkerAlpha", 0, 1, 1);
        private readonly Label lblShpPointLabelText = _Utils.CreateLabel("lblShpPointLabelText", "Label Text");
        private readonly TextBox txtShpPointLabelText = _Utils.CreateTextBox("txtShpPointLabelText", "");
        private readonly Label lblShpPointLabelFontSize = _Utils.CreateLabel("lblShpPointLabelFontSize", "Font Size");
        private readonly NumericUpDown numShpPointLabelFontSize = _Utils.CreateNumericUpDown("numShpPointLabelFontSize", 1, 30, 8);
        private readonly Label lblShpPointLabelColor = _Utils.CreateLabel("lblShpPointLabelColor", "Label Color");
        private readonly Panel pnlShpPointLabelColor = _Utils.CreatePanel("pnlShpPointLabelColor", Color.White);
        private readonly Button btnShpPointLabelColor = _Utils.CreateButton("btnShpPointLabelColor", "...");
        private readonly Label lblShpPointLabelHA = _Utils.CreateLabel("lblShpPointLabelHA", "Horizontal Alignment");
        private readonly ComboBox comboShpPointLabelHA = _Utils.CreateComboBox("comboShpPointLabelHA", ["Left", "Center", "Right"], 0);
        private readonly Label lblShpPointLabelVA = _Utils.CreateLabel("lblShpPointLabelVA", "Vertical Alignment");
        private readonly ComboBox comboShpPointLabelVA = _Utils.CreateComboBox("comboShpPointLabelVA", ["Top", "Center", "Bottom"], 1);
        private readonly Label lblShpPointLabelOffsetPointsX = _Utils.CreateLabel("lblShpPointLabelOffsetPointsX", "Offset Points X");
        private readonly TextBox txtShpPointLabelOffsetPointsX = _Utils.CreateTextBox("txtShpPointLabelOffsetPointsX", "0");
        private readonly Label lblShpPointLabelOffsetPointsY = _Utils.CreateLabel("lblShpPointLabelOffsetPointsY", "Offset Points Y");
        private readonly TextBox txtShpPointLabelOffsetPointsY = _Utils.CreateTextBox("txtShpPointLabelOffsetPointsY", "0");
        private readonly Label lblShpPointLabelOffsetDataX = _Utils.CreateLabel("lblShpPointLabelOffsetDataX", "Offset Data X");
        private readonly TextBox txtShpPointLabelOffsetDataX = _Utils.CreateTextBox("txtShpPointLabelOffsetDataX", "0");
        private readonly Label lblShpPointLabelOffsetDataY = _Utils.CreateLabel("lblShpPointLabelOffsetDataY", "Offset Data Y");
        private readonly TextBox txtShpPointLabelOffsetDataY = _Utils.CreateTextBox("txtShpPointLabelOffsetDataY", "0");

        private readonly TableLayoutPanel tableShpLine = new();
        private readonly Label lblShpLinePath = _Utils.CreateLabel("lblShpLinePath", "Path");
        private readonly TextBox txtShpLinePath = _Utils.CreateTextBox("txtShpLinePath", "");
        private readonly Label lblShpLineType = _Utils.CreateLabel("lblShpLineType", "Type");
        private readonly TextBox txtShpLineType = _Utils.CreateTextBox("txtShpLineType", "");
        private readonly Label lblShpLineColor = _Utils.CreateLabel("lblShpLineColor", "Color");
        private readonly Panel pnlShpLineColor = _Utils.CreatePanel("pnlShpLineColor", Color.White);
        private readonly Button btnShpLineColor = _Utils.CreateButton("btnShpLineColor", "...");
        private readonly Label lblShpLineLineWidth = _Utils.CreateLabel("lblShpLineLineWidth", "Line Width");
        private readonly TextBox txtShpLineLineWidth = _Utils.CreateTextBox("txtShpLineLineWidth", "");
        private readonly Label lblShpLineAlpha = _Utils.CreateLabel("lblShpLineAlpha", "Transparency");
        private readonly NumericUpDown numShpLineAlpha = _Utils.CreateNumericUpDown("numShpLineAlpha", 0, 1, 1);
        private readonly Label lblShpLineLabelText = _Utils.CreateLabel("lblShpLineLabelText", "Label Text");
        private readonly TextBox txtShpLineLabelText = _Utils.CreateTextBox("txtShpLineLabelText", "");
        private readonly Label lblShpLineLabelFontSize = _Utils.CreateLabel("lblShpLineLabelFontSize", "Font Size");
        private readonly NumericUpDown numShpLineLabelFontSize = _Utils.CreateNumericUpDown("numShpLineLabelFontSize", 0, 100, 12);
        private readonly Label lblShpLineLabelColor = _Utils.CreateLabel("lblShpLineLabelColor", "Label Color");
        private readonly Panel pnlShpLineLabelColor = _Utils.CreatePanel("pnlShpLineLabelColor", Color.White);
        private readonly Button btnShpLineLabelColor = _Utils.CreateButton("btnShpLineLabelColor", "...");
        private readonly Label lblShpLineLabelHA = _Utils.CreateLabel("lblShpLineLabelHA", "Horizontal Alignment");
        private readonly ComboBox comboShpLineLabelHA = _Utils.CreateComboBox("comboShpLineLabelHA", ["Left", "Center", "Right"], 0);
        private readonly Label lblShpLineLabelVA = _Utils.CreateLabel("lblShpLineLabelVA", "Vertical Alignment");
        private readonly ComboBox comboShpLineLabelVA = _Utils.CreateComboBox("comboShpLineLabelVA", ["Top", "Middle", "Bottom"], 0);
        private readonly Label lblShpLineLabelOffsetPointsX = _Utils.CreateLabel("lblShpLineLabelOffsetPointsX", "Offset Points X");
        private readonly TextBox txtShpLineLabelOffsetPointsX = _Utils.CreateTextBox("txtShpLineLabelOffsetPointsX", "0");
        private readonly Label lblShpLineLabelOffsetPointsY = _Utils.CreateLabel("lblShpLineLabelOffsetPointsY", "Offset Points Y");
        private readonly TextBox txtShpLineLabelOffsetPointsY = _Utils.CreateTextBox("txtShpLineLabelOffsetPointsY", "0");
        private readonly Label lblShpLineLabelOffsetDataX = _Utils.CreateLabel("lblShpLineLabelOffsetDataX", "Offset Data X");
        private readonly TextBox txtShpLineLabelOffsetDataX = _Utils.CreateTextBox("txtShpLineLabelOffsetDataX", "0");
        private readonly Label lblShpLineLabelOffsetDataY = _Utils.CreateLabel("lblShpLineLabelOffsetDataY", "Offset Data Y");
        private readonly TextBox txtShpLineLabelOffsetDataY = _Utils.CreateTextBox("txtShpLineLabelOffsetDataY", "0");

        private readonly TableLayoutPanel tableShpPoly = new();
        private readonly Label lblShpPolyPath = _Utils.CreateLabel("lblShpPolyPath", "Path");
        private readonly TextBox txtShpPolyPath = _Utils.CreateTextBox("txtShpPolyPath", "");
        private readonly Label lblShpPolyType = _Utils.CreateLabel("lblShpPolyType", "Shapefile Type");
        private readonly TextBox txtShpPolyType = _Utils.CreateTextBox("txtShpPolyType", "");
        private readonly Label lblShpPolyEdgeColor = _Utils.CreateLabel("lblShpPolyEdgeColor", "Edge Color");
        private readonly Panel pnlShpPolyEdgeColor = _Utils.CreatePanel("pnlShpPolyEdgeColor", Color.White);
        private readonly Button btnShpPolyEdgeColor = _Utils.CreateButton("btnShpPolyEdgeColor", "...");
        private readonly Label lblShpPolyLineWidth = _Utils.CreateLabel("lblShpPolyLineWidth", "Line Width");
        private readonly TextBox txtShpPolyLineWidth = _Utils.CreateTextBox("txtShpPolyLineWidth", "");
        private readonly Label lblShpPolyFaceColor = _Utils.CreateLabel("lblShpPolyFaceColor", "Face Color");
        private readonly Panel pnlShpPolyFaceColor = _Utils.CreatePanel("pnlShpPolyFaceColor", Color.White);
        private readonly Button btnShpPolyFaceColor = _Utils.CreateButton("btnShpPolyFaceColor", "...");
        private readonly Label lblShpPolyAlpha = _Utils.CreateLabel("lblShpPolyAlpha", "Alpha");
        private readonly NumericUpDown numShpPolyAlpha = _Utils.CreateNumericUpDown("numShpPolyAlpha", 0, 255, 255);
        private readonly Label lblShpPolyLabelText = _Utils.CreateLabel("lblShpPolyLabelText", "Label Text");
        private readonly TextBox txtShpPolyLabelText = _Utils.CreateTextBox("txtShpPolyLabelText", "");
        private readonly Label lblShpPolyLabelFontSize = _Utils.CreateLabel("lblShpPolyLabelFontSize", "Font Size");
        private readonly NumericUpDown numShpPolyLabelFontSize = _Utils.CreateNumericUpDown("numShpPolyLabelFontSize", 1, 30, 8);
        private readonly Label lblShpPolyLabelColor = _Utils.CreateLabel("lblShpPolyLabelColor", "Label Color");
        private readonly Panel pnlShpPolyLabelColor = _Utils.CreatePanel("pnlShpPolyLabelColor", Color.White);
        private readonly Button btnShpPolyLabelColor = _Utils.CreateButton("btnShpPolyLabelColor", "...");
        private readonly Label lblShpPolyLabelHA = _Utils.CreateLabel("lblShpPolyLabelHA", "Horizontal Alignment");
        private readonly ComboBox comboShpPolyLabelHA = _Utils.CreateComboBox("comboShpPolyLabelHA", ["Left", "Center", "Right"], 0);
        private readonly Label lblShpPolyLabelVA = _Utils.CreateLabel("lblShpPolyLabelVA", "Vertical Alignment");
        private readonly ComboBox comboShpPolyLabelVA = _Utils.CreateComboBox("comboShpPolyLabelVA", ["Top", "Center", "Bottom"], 1);
        private readonly Label lblShpPolyLabelOffsetPointsX = _Utils.CreateLabel("lblShpPolyLabelOffsetPointsX", "Offset Points X");
        private readonly TextBox txtShpPolyLabelOffsetPointsX = _Utils.CreateTextBox("txtShpPolyLabelOffsetPointsX", "0");
        private readonly Label lblShpPolyLabelOffsetPointsY = _Utils.CreateLabel("lblShpPolyLabelOffsetPointsY", "Offset Points Y");
        private readonly TextBox txtShpPolyLabelOffsetPointsY = _Utils.CreateTextBox("txtShpPolyLabelOffsetPointsY", "0");
        private readonly Label lblShpPolyLabelOffsetDataX = _Utils.CreateLabel("lblShpPolyLabelOffsetDataX", "Offset Data X");
        private readonly TextBox txtShpPolyLabelOffsetDataX = _Utils.CreateTextBox("txtShpPolyLabelOffsetDataX", "0");
        private readonly Label lblShpPolyLabelOffsetDataY = _Utils.CreateLabel("lblShpPolyLabelOffsetDataY", "Offset Data Y");
        private readonly TextBox txtShpPolyLabelOffsetDataY = _Utils.CreateTextBox("txtShpPolyLabelOffsetDataY", "0");

        private void InitializeShpTab()
        {
            InitializeShpProperties();
            BuildTableShpPoint();
            BuildTableShpLine();
            BuildTableShpPoly();
        }

        private void InitializeShpProperties()
        {
            txtShpPointPath.TextChanged += txtShp_TextChanged;
            txtShpPointType.TextChanged += txtShp_TextChanged;
            pnlShpPointColor.BackColorChanged += pnlBackColor_Changed;
            btnShpPointColor.Click += colorButton_Click;
            comboShpPointMarker.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            numShpPointMarkerSize.ValueChanged += numShp_ValueChanged;
            numShpPointMarkerAlpha.ValueChanged += numShp_ValueChanged;
            txtShpPointLabelText.TextChanged += txtShp_TextChanged;
            numShpPointLabelFontSize.ValueChanged += numShp_ValueChanged;
            pnlShpPointLabelColor.BackColorChanged += pnlBackColor_Changed;
            btnShpPointLabelColor.Click += colorButton_Click;
            comboShpPointLabelHA.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            comboShpPointLabelVA.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            txtShpPointLabelOffsetPointsX.TextChanged += txtShp_TextChanged;
            txtShpPointLabelOffsetPointsY.TextChanged += txtShp_TextChanged;
            txtShpPointLabelOffsetDataX.TextChanged += txtShp_TextChanged;
            txtShpPointLabelOffsetDataY.TextChanged += txtShp_TextChanged;

            txtShpLinePath.TextChanged += txtShp_TextChanged;
            txtShpLineType.TextChanged += txtShp_TextChanged;
            pnlShpLineColor.BackColorChanged += pnlBackColor_Changed;
            btnShpLineColor.Click += colorButton_Click;
            txtShpLineLineWidth.TextChanged += txtShp_TextChanged;
            numShpLineAlpha.ValueChanged += numShp_ValueChanged;
            txtShpLineLabelText.TextChanged += txtShp_TextChanged;
            numShpLineLabelFontSize.ValueChanged += numShp_ValueChanged;
            pnlShpLineLabelColor.BackColorChanged += pnlBackColor_Changed;
            btnShpLineLabelColor.Click += colorButton_Click;
            comboShpLineLabelHA.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            comboShpLineLabelVA.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            txtShpLineLabelOffsetPointsX.TextChanged += txtShp_TextChanged;
            txtShpLineLabelOffsetPointsY.TextChanged += txtShp_TextChanged;
            txtShpLineLabelOffsetDataX.TextChanged += txtShp_TextChanged;
            txtShpLineLabelOffsetDataY.TextChanged += txtShp_TextChanged;

            txtShpPolyPath.TextChanged += txtShp_TextChanged;
            txtShpPolyType.TextChanged += txtShp_TextChanged;
            pnlShpPolyEdgeColor.BackColorChanged += pnlBackColor_Changed;
            btnShpPolyEdgeColor.Click += colorButton_Click;
            txtShpPolyLineWidth.TextChanged += txtShp_TextChanged;
            pnlShpPolyFaceColor.BackColorChanged += pnlBackColor_Changed;
            btnShpPolyFaceColor.Click += colorButton_Click;
            numShpPolyAlpha.ValueChanged += numShp_ValueChanged;
            txtShpPolyLabelText.TextChanged += txtShp_TextChanged;
            numShpPolyLabelFontSize.ValueChanged += numShp_ValueChanged;
            pnlShpPolyLabelColor.BackColorChanged += pnlBackColor_Changed;
            btnShpPolyLabelColor.Click += colorButton_Click;
            comboShpPolyLabelHA.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            comboShpPolyLabelVA.SelectedIndexChanged += comboShp_SelectedIndexChanged;
            txtShpPolyLabelOffsetPointsX.TextChanged += txtShp_TextChanged;
            txtShpPolyLabelOffsetPointsY.TextChanged += txtShp_TextChanged;
            txtShpPolyLabelOffsetDataX.TextChanged += txtShp_TextChanged;
            txtShpPolyLabelOffsetDataY.TextChanged += txtShp_TextChanged;

        }

        private static void AddTo(TableLayoutPanel host, Control c, int col, int row)
        {
            if (c.Parent != null && c.Parent != host)
                c.Parent.Controls.Remove(c);
            host.Controls.Add(c, col, row);
        }
        private void BuildTableShpPoint()
        {
            int nrows = 16;
            tableShpPoint.ColumnCount = 3;
            tableShpPoint.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tableShpPoint.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableShpPoint.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableShpPoint.Dock = DockStyle.Fill;
            tableShpPoint.Location = new Point(0, 0);
            tableShpPoint.Name = "tableShpPoint";
            tableShpPoint.RowCount = nrows;
            for (int i = 0; i < (nrows - 1); i++)
                tableShpPoint.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableShpPoint.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableShpPoint.TabIndex = 0;
            AddTo(tableShpPoint, lblShpPointPath, 0, 0);
            AddTo(tableShpPoint, lblShpPointType, 0, 1);
            AddTo(tableShpPoint, txtShpPointPath, 1, 0);
            AddTo(tableShpPoint, txtShpPointType, 1, 1);
            AddTo(tableShpPoint, lblShpPointColor, 0, 2);
            AddTo(tableShpPoint, pnlShpPointColor, 1, 2);
            AddTo(tableShpPoint, btnShpPointColor, 2, 2);
            AddTo(tableShpPoint, lblShpPointMarker, 0, 3);
            AddTo(tableShpPoint, lblShpPointMarkerSize, 0, 4);
            AddTo(tableShpPoint, lblShpPointMarkerAlpha, 0, 5);
            AddTo(tableShpPoint, comboShpPointMarker, 1, 3);
            AddTo(tableShpPoint, numShpPointMarkerSize, 1, 4);
            AddTo(tableShpPoint, numShpPointMarkerAlpha, 1, 5);
            AddTo(tableShpPoint, lblShpPointLabelText, 0, 6);
            AddTo(tableShpPoint, txtShpPointLabelText, 1, 6);
            AddTo(tableShpPoint, lblShpPointLabelFontSize, 0, 7);
            AddTo(tableShpPoint, numShpPointLabelFontSize, 1, 7);
            AddTo(tableShpPoint, lblShpPointLabelColor, 0, 8);
            AddTo(tableShpPoint, pnlShpPointLabelColor, 1, 8);
            AddTo(tableShpPoint, btnShpPointLabelColor, 2, 8);
            AddTo(tableShpPoint, lblShpPointLabelHA, 0, 9);
            AddTo(tableShpPoint, comboShpPointLabelHA, 1, 9);
            AddTo(tableShpPoint, lblShpPointLabelVA, 0, 10);
            AddTo(tableShpPoint, comboShpPointLabelVA, 1, 10);
            AddTo(tableShpPoint, lblShpPointLabelOffsetPointsX, 0, 11);
            AddTo(tableShpPoint, txtShpPointLabelOffsetPointsX, 1, 11);
            AddTo(tableShpPoint, lblShpPointLabelOffsetPointsY, 0, 12);
            AddTo(tableShpPoint, txtShpPointLabelOffsetPointsY, 1, 12);
            AddTo(tableShpPoint, lblShpPointLabelOffsetDataX, 0, 13);
            AddTo(tableShpPoint, txtShpPointLabelOffsetDataX, 1, 13);
            AddTo(tableShpPoint, lblShpPointLabelOffsetDataY, 0, 14);
            AddTo(tableShpPoint, txtShpPointLabelOffsetDataY, 1, 14);

        }

        private void BuildTableShpLine()
        {
            int nrows = 15;
            tableShpLine.ColumnCount = 3;
            tableShpLine.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tableShpLine.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableShpLine.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableShpLine.Dock = DockStyle.Fill;
            tableShpLine.Location = new Point(0, 0);
            tableShpLine.Name = "tableShpLine";
            tableShpLine.RowCount = nrows;
            for (int i = 0; i < (nrows - 1); i++)
                tableShpLine.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableShpLine.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableShpLine.TabIndex = 0;
            AddTo(tableShpLine, lblShpLinePath, 0, 0);
            AddTo(tableShpLine, lblShpLineType, 0, 1);
            AddTo(tableShpLine, txtShpLinePath, 1, 0);
            AddTo(tableShpLine, txtShpLineType, 1, 1);
            AddTo(tableShpLine, lblShpLineColor, 0, 2);
            AddTo(tableShpLine, pnlShpLineColor, 1, 2);
            AddTo(tableShpLine, btnShpLineColor, 2, 2);
            AddTo(tableShpLine, lblShpLineLineWidth, 0, 3);
            AddTo(tableShpLine, txtShpLineLineWidth, 1, 3);
            AddTo(tableShpLine, lblShpLineAlpha, 0, 4);
            AddTo(tableShpLine, numShpLineAlpha, 1, 4);
            AddTo(tableShpLine, lblShpLineLabelText, 0, 5);
            AddTo(tableShpLine, txtShpLineLabelText, 1, 5);
            AddTo(tableShpLine, lblShpLineLabelFontSize, 0, 6);
            AddTo(tableShpLine, numShpLineLabelFontSize, 1, 6);
            AddTo(tableShpLine, lblShpLineLabelColor, 0, 7);
            AddTo(tableShpLine, pnlShpLineLabelColor, 1, 7);
            AddTo(tableShpLine, btnShpLineLabelColor, 2, 7);
            AddTo(tableShpLine, lblShpLineLabelHA, 0, 8);
            AddTo(tableShpLine, comboShpLineLabelHA, 1, 8);
            AddTo(tableShpLine, lblShpLineLabelVA, 0, 9);
            AddTo(tableShpLine, comboShpLineLabelVA, 1, 9);
            AddTo(tableShpLine, lblShpLineLabelOffsetPointsX, 0, 10);
            AddTo(tableShpLine, txtShpLineLabelOffsetPointsX, 1, 10);
            AddTo(tableShpLine, lblShpLineLabelOffsetPointsY, 0, 11);
            AddTo(tableShpLine, txtShpLineLabelOffsetPointsY, 1, 11);
            AddTo(tableShpLine, lblShpLineLabelOffsetDataX, 0, 12);
            AddTo(tableShpLine, txtShpLineLabelOffsetDataX, 1, 12);
            AddTo(tableShpLine, lblShpLineLabelOffsetDataY, 0, 13);
            AddTo(tableShpLine, txtShpLineLabelOffsetDataY, 1, 13);

        }

        private void BuildTableShpPoly()
        {
            int nrows = 16;
            tableShpPoly.ColumnCount = 3;
            tableShpPoly.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tableShpPoly.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableShpPoly.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableShpPoly.Dock = DockStyle.Fill;
            tableShpPoly.Location = new Point(0, 0);
            tableShpPoly.Name = "tableShpPoly";
            tableShpPoly.RowCount = nrows;
            for (int i = 0; i < (nrows - 1); i++)
                tableShpPoly.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableShpPoly.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableShpPoly.TabIndex = 0;
            AddTo(tableShpPoly, lblShpPolyPath, 0, 0);
            AddTo(tableShpPoly, lblShpPolyType, 0, 1);
            AddTo(tableShpPoly, txtShpPolyPath, 1, 0);
            AddTo(tableShpPoly, txtShpPolyType, 1, 1);
            AddTo(tableShpPoly, lblShpPolyEdgeColor, 0, 2);
            AddTo(tableShpPoly, pnlShpPolyEdgeColor, 1, 2);
            AddTo(tableShpPoly, btnShpPolyEdgeColor, 2, 2);
            AddTo(tableShpPoly, lblShpPolyLineWidth, 0, 3);
            AddTo(tableShpPoly, txtShpPolyLineWidth, 1, 3);
            AddTo(tableShpPoly, lblShpPolyFaceColor, 0, 4);
            AddTo(tableShpPoly, pnlShpPolyFaceColor, 1, 4);
            AddTo(tableShpPoly, btnShpPolyFaceColor, 2, 4);
            AddTo(tableShpPoly, lblShpPolyAlpha, 0, 5);
            AddTo(tableShpPoly, numShpPolyAlpha, 1, 5);
            AddTo(tableShpPoly, lblShpPolyLabelText, 0, 6);
            AddTo(tableShpPoly, txtShpPolyLabelText, 1, 6);
            AddTo(tableShpPoly, lblShpPolyLabelFontSize, 0, 7);
            AddTo(tableShpPoly, numShpPolyLabelFontSize, 1, 7);
            AddTo(tableShpPoly, lblShpPolyLabelColor, 0, 8);
            AddTo(tableShpPoly, pnlShpPolyLabelColor, 1, 8);
            AddTo(tableShpPoly, btnShpPolyLabelColor, 2, 8);
            AddTo(tableShpPoly, lblShpPolyLabelHA, 0, 9);
            AddTo(tableShpPoly, comboShpPolyLabelHA, 1, 9);
            AddTo(tableShpPoly, lblShpPolyLabelVA, 0, 10);
            AddTo(tableShpPoly, comboShpPolyLabelVA, 1, 10);
            AddTo(tableShpPoly, lblShpPolyLabelOffsetPointsX, 0, 11);
            AddTo(tableShpPoly, txtShpPolyLabelOffsetPointsX, 1, 11);
            AddTo(tableShpPoly, lblShpPolyLabelOffsetPointsY, 0, 12);
            AddTo(tableShpPoly, txtShpPolyLabelOffsetPointsY, 1, 12);
            AddTo(tableShpPoly, lblShpPolyLabelOffsetDataX, 0, 13);
            AddTo(tableShpPoly, txtShpPolyLabelOffsetDataX, 1, 13);
            AddTo(tableShpPoly, lblShpPolyLabelOffsetDataY, 0, 14);
            AddTo(tableShpPoly, txtShpPolyLabelOffsetDataY, 1, 14);

        }

        private static string GetShapeType(string filePath)
        {
            // Example using NetTopologySuite
            using var reader = new ShapefileDataReader(filePath, new NetTopologySuite.Geometries.GeometryFactory());
            var geomType = reader.ShapeHeader.ShapeType.ToString();
            if (geomType.Contains("Point")) return "Point";
            if (geomType.Contains("Polygon")) return "Polygon";
            if (geomType.Contains("Line")) return "Line";

            return "The shapefile type " + geomType + " is not supported.";
        }

        private static void selectComboByValue(ComboBox combo, string value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i].ToString() == value)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
            // If we reach here, the value was not found
            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0; // Select the first item as a fallback
            }
        }

        private static string getValueByIndex(ComboBox combo, int index)
        {
            if (index >= 0 && index < combo.Items.Count)
            {
                return combo.Items[index].ToString();
            }
            return string.Empty; // or throw an exception, or return null, based on your needs
        }

        private static void FillSurveyTree(TreeView tree)
        {
            tree.Nodes.Clear();
            foreach (XmlElement survey in _ClassConfigurationManager.GetObjects("Survey"))
            {
                string surveyName = survey.GetAttribute("name");
                TreeNode nodeElement = new(surveyName)
                {
                    Tag = survey
                };
                tree.Nodes.Add(nodeElement);
            }
        }

        private static void SetSurveyCheck(TreeView tree, XmlElement settingElement)
        {
            XmlElement? surveysElement = settingElement.SelectSingleNode("Surveys") as XmlElement;
            if (surveysElement == null)
            {
                MessageBox.Show("No Surveys element found in settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XmlNodeList? surveys = surveysElement.SelectNodes("Survey");
            if (surveys == null)
                return;
            List<string> surveyIds = [];
            foreach (XmlNode s in surveys)
            {
                if (s is XmlElement surveyElem)
                {
                    string ID = surveyElem?.InnerText ?? string.Empty;
                    surveyIds.Add(ID);
                }
            }
            foreach (TreeNode surveyNode in tree.Nodes)
            {
                XmlNode? node = surveyNode.Tag as XmlNode;
                if (node is XmlElement elem)
                {
                    string id = elem.GetAttribute("id");
                    if (surveyIds.Contains(id))
                    {
                        surveyNode.Checked = true;
                    }
                    else
                    {
                        surveyNode.Checked = false;
                    }
                }
            }
        }

        private static void populateTextBox(TextBox textBox, XmlElement? parent, string nodeName, string defaultValue)
        {
            if (parent == null)
            {
                textBox.Text = defaultValue;
                return;
            }
            XmlNode? node = parent.SelectSingleNode(nodeName);
            if (node != null)
            {
                if (!String.IsNullOrEmpty(node.InnerText))
                    textBox.Text = node.InnerText;
                else
                    textBox.Text = defaultValue;
            }
            else
                textBox.Text = defaultValue;
        }

        private static void populateComboBox(ComboBox comboBox, XmlElement? parent, string nodeName)
        {
            if (parent == null)
            {
                comboBox.SelectedIndex = 0;
                return;
            }
            XmlNode? node = parent.SelectSingleNode(nodeName);
            if (node != null)
            {
                if (!String.IsNullOrEmpty(node.InnerText))
                    selectComboByValue(comboBox, node.InnerText);
                else
                    comboBox.SelectedIndex = 0;
            }
            else
                comboBox.SelectedIndex = 0;
        }

        private static void populateCMapCombo(ColormapComboBox comboBox, XmlElement? parent, string nodeName)
        {
            string defaultName = "jet";

            if (parent != null)
            {
                XmlNode? node = parent.SelectSingleNode(nodeName);
                if (node != null && !string.IsNullOrEmpty(node.InnerText))
                {
                    defaultName = node.InnerText;
                }
            }
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is ColormapItem item && string.Equals(item.Name, defaultName, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }
        }

        private static void populatePanelColor(Panel panel, XmlElement? parent, string nodeName, string defaultColor)
        {
            if (parent == null)
            {
                panel.BackColor = ColorTranslator.FromHtml(defaultColor);
                return;
            }
            XmlNode? node = parent.SelectSingleNode(nodeName);
            if (node != null)
            {
                if (!String.IsNullOrEmpty(node.InnerText))
                    panel.BackColor = ColorTranslator.FromHtml(node.InnerText);
                else
                    panel.BackColor = ColorTranslator.FromHtml(defaultColor);
            }
            else
                panel.BackColor = ColorTranslator.FromHtml(defaultColor);
        }

        private static void populateNumericUpDown(NumericUpDown numericUpDown, XmlElement? parent, string nodeName, decimal defaultValue)
        {
            if (parent == null)
            {
                numericUpDown.Value = defaultValue;
                return;
            }
            XmlNode? node = parent.SelectSingleNode(nodeName);
            if (node != null)
            {
                if (decimal.TryParse(node.InnerText, out decimal value))
                    numericUpDown.Value = value;
                else
                    numericUpDown.Value = defaultValue;
            }
            else
                numericUpDown.Value = defaultValue;
        }

        private void populate2DMapSetting()
        {
            populatePanelColor(pnl2DBackColor, mapSettings2D, "BackgroundColor", "#000000");
            populateComboBox(combo2DFieldName, mapSettings2D, "FieldName");
            populateCMapCombo(combo2Dcmap, mapSettings2D, "Colormap");
            populateTextBox(txt2Dvmin, mapSettings2D, "vmin", "");
            populateTextBox(txt2Dvmax, mapSettings2D, "vmax", "");
            populateTextBox(txt2DPagDeg, mapSettings2D, "Padding", "0.1");
            populateNumericUpDown(num2DNGridLines, mapSettings2D, "NGridLines", 10);
            populateTextBox(txt2DGridOpacity, mapSettings2D, "GridOpacity", "0.35");
            populatePanelColor(pnl2DGridColor, mapSettings2D, "GridColor", "#333333");
            populateTextBox(txt2DGridWidth, mapSettings2D, "GridWidth", "1");
            populateNumericUpDown(num2DNAxisTicks, mapSettings2D, "NAxisTicks", 7);
            populateNumericUpDown(num2DTickFontSize, mapSettings2D, "TickFontSize", 10);
            populateNumericUpDown(num2DNTicksDecimals, mapSettings2D, "TickNDecimals", 4);
            populateNumericUpDown(num2DAxisLabelFontSize, mapSettings2D, "AxisLabelFontSize", 12);
            populatePanelColor(pnl2DAxisLabelColor, mapSettings2D, "AxisLabelColor", "#cccccc");
            populateNumericUpDown(num2DHoverFontSize, mapSettings2D, "HoverFontSize", 9);
            populateTextBox(txt2DTransectLineWidth, mapSettings2D, "TransectLineWidth", "3");
            populateComboBox(combo2DBins, mapSettings2D, "VerticalAggBinItem");
            if (combo2DBins.SelectedItem?.ToString() == "Bin")
            {
                num2DBins.Visible = true;
                txt2DDepth.Visible = false;
            }
            else
            {
                num2DBins.Visible = false;
                txt2DDepth.Visible = true;
            }
            XmlNode? verticalAggBinTargetNode = mapSettings2D.SelectSingleNode("VerticalAggBinTarget");
            if (verticalAggBinTargetNode != null)
            {
                string? target = verticalAggBinTargetNode.InnerText;
                if (target != null)
                {
                    if (target == "Mean")
                    {
                        num2DBins.Enabled = false;
                        check2DBins.Checked = true;
                    }
                    else
                    {
                        num2DBins.Enabled = true;
                        check2DBins.Checked = false;
                        if (int.TryParse(target, out int value))
                            num2DBins.Value = value;
                        else
                            num2DBins.Value = 1;
                    }
                }
                else
                {
                    num2DBins.Enabled = true;
                    check2DBins.Checked = false;
                    num2DBins.Value = 1;
                }
            }
            else
            {
                num2DBins.Enabled = true;
                check2DBins.Checked = false;
                num2DBins.Value = 1;
            }
            XmlNode? verticalAggBeamNode = mapSettings2D.SelectSingleNode("VerticalAggBeam");
            if (verticalAggBeamNode != null)
            {
                string? beam = verticalAggBeamNode.InnerText;
                if (beam != null)
                {
                    if (beam == "Mean")
                    {
                        num2DBeams.Enabled = false;
                        check2DBeams.Checked = true;
                    }
                    else
                    {
                        num2DBeams.Enabled = true;
                        check2DBeams.Checked = false;
                        if (int.TryParse(beam, out int value))
                            num2DBeams.Value = value;
                        else
                            num2DBeams.Value = 1;
                    }
                }
                else
                {
                    num2DBeams.Enabled = true;
                    check2DBeams.Checked = false;
                    num2DBeams.Value = 1;
                }
            }
            else
            {
                num2DBeams.Enabled = true;
                check2DBeams.Checked = false;
                num2DBeams.Value = 1;
            }
            FillSurveyTree(treeSurveys2D);
            SetSurveyCheck(treeSurveys2D, mapSettings2D);
        }

        private void populate3DMapSetting()
        {
            populatePanelColor(pnl3DBackColor, mapSettings3D, "BackgroundColor", "#000000");
            populateComboBox(combo3DFieldName, mapSettings3D, "FieldName");
            populateCMapCombo(combo3Dcmap, mapSettings3D, "Colormap");
            populateTextBox(txt3Dvmin, mapSettings3D, "vmin", "");
            populateTextBox(txt3Dvmax, mapSettings3D, "vmax", "");
            populateTextBox(txt3DPagDeg, mapSettings3D, "Padding", "0.1");
            populateNumericUpDown(num3DNGridLines, mapSettings3D, "NGridLines", 10);
            populateTextBox(txt3DGridOpacity, mapSettings3D, "GridOpacity", "0.35");
            populatePanelColor(pnl3DGridColor, mapSettings3D, "GridColor", "#333333");
            populateTextBox(txt3DGridWidth, mapSettings3D, "GridWidth", "1");
            populateNumericUpDown(num3DNAxisTicks, mapSettings3D, "NAxisTicks", 7);
            populateNumericUpDown(num3DTickFontSize, mapSettings3D, "TickFontSize", 10);
            populateNumericUpDown(num3DNTicksDecimals, mapSettings3D, "TickNDecimals", 4);
            populateNumericUpDown(num3DAxisLabelFontSize, mapSettings3D, "AxisLabelFontSize", 12);
            populatePanelColor(pnl3DAxisLabelColor, mapSettings3D, "AxisLabelColor", "#cccccc");
            populateNumericUpDown(num3DHoverFontSize, mapSettings3D, "HoverFontSize", 9);
            populateTextBox(txt3DZScale, mapSettings3D, "ZScale", "3.0");
            FillSurveyTree(treeSurveys3D);
            SetSurveyCheck(treeSurveys3D, mapSettings3D);
        }

        private void PopulateShapefileSetting()
        {
            treeShapefiles.Nodes.Clear();
            XmlNodeList? shapefiles = mapShapefiles.SelectNodes("Shapefile");
            if (shapefiles == null || shapefiles.Count == 0)
                return;
            foreach (XmlNode shpNode in shapefiles)
            {
                if (shpNode is not XmlElement shp)
                    continue;
                XmlNode? nameNode = shp.SelectSingleNode("Name");
                string name = nameNode != null ? nameNode.InnerText : "Unnamed";
                XmlNode? pathNode = shp.SelectSingleNode("Path");
                string path = pathNode != null ? pathNode.InnerText : "";
                XmlNode? kindNode = shp.SelectSingleNode("Kind");
                string kind = kindNode != null ? kindNode.InnerText : "Point";
                ShapeFile shapeFile = new(name: name, path: path, kind: kind);
                if (shapeFile.Kind == "Point")
                {
                    XmlNode? pointColorNode = shp.SelectSingleNode("PointColor");
                    shapeFile.PointColor = pointColorNode != null ? pointColorNode.InnerText : "#000000";
                    XmlNode? pointMarkerNode = shp.SelectSingleNode("PointMarker");
                    shapeFile.PointMarker = pointMarkerNode != null ? pointMarkerNode.InnerText : "o";
                    XmlNode? pointMarkerSizeNode = shp.SelectSingleNode("PointMarkerSize");
                    shapeFile.PointMarkerSize = pointMarkerSizeNode != null ? pointMarkerSizeNode.InnerText : "12";
                    XmlNode? pointAlphaNode = shp.SelectSingleNode("PointAlpha");
                    shapeFile.PointAlpha = pointAlphaNode != null ? pointAlphaNode.InnerText : "1.0";
                    XmlNode? labelTextNode = shp.SelectSingleNode("LabelText");
                    shapeFile.PointLabelText = labelTextNode != null ? labelTextNode.InnerText : "";
                    XmlNode? labelFontSizeNode = shp.SelectSingleNode("LabelFontSize");
                    shapeFile.PointLabelFontSize = labelFontSizeNode != null ? labelFontSizeNode.InnerText : "8";
                    XmlNode? labelColorNode = shp.SelectSingleNode("LabelColor");
                    shapeFile.PointLabelColor = labelColorNode != null ? labelColorNode.InnerText : "#000000";
                    XmlNode? labelHANode = shp.SelectSingleNode("LabelHA");
                    shapeFile.PointLabelHA = labelHANode != null ? labelHANode.InnerText : "Left";
                    XmlNode? labelVANode = shp.SelectSingleNode("LabelVA");
                    shapeFile.PointLabelVA = labelVANode != null ? labelVANode.InnerText : "Center";
                    XmlNode? labelOffsetPointsXNode = shp.SelectSingleNode("LabelOffsetPointsX");
                    shapeFile.PointLabelOffsetPointsX = labelOffsetPointsXNode != null ? labelOffsetPointsXNode.InnerText : "0";
                    XmlNode? labelOffsetPointsYNode = shp.SelectSingleNode("LabelOffsetPointsY");
                    shapeFile.PointLabelOffsetPointsY = labelOffsetPointsYNode != null ? labelOffsetPointsYNode.InnerText : "0";
                    XmlNode? labelOffsetDataXNode = shp.SelectSingleNode("LabelOffsetDataX");
                    shapeFile.PointLabelOffsetDataX = labelOffsetDataXNode != null ? labelOffsetDataXNode.InnerText : "0";
                    XmlNode? labelOffsetDataYNode = shp.SelectSingleNode("LabelOffsetDataY");
                    shapeFile.PointLabelOffsetDataY = labelOffsetDataYNode != null ? labelOffsetDataYNode.InnerText : "0";
                }
                else if (shapeFile.Kind == "Line")
                {
                    XmlNode? lineColorNode = shp.SelectSingleNode("LineColor");
                    shapeFile.LineColor = lineColorNode != null ? lineColorNode.InnerText : "#000000";
                    XmlNode? lineLineWidthNode = shp.SelectSingleNode("LineLineWidth");
                    shapeFile.LineLineWidth = lineLineWidthNode != null ? lineLineWidthNode.InnerText : "1";
                    XmlNode? lineAlphaNode = shp.SelectSingleNode("LineAlpha");
                    shapeFile.LineAlpha = lineAlphaNode != null ? lineAlphaNode.InnerText : "1.0";
                    XmlNode? labelTextNode = shp.SelectSingleNode("LabelText");
                    shapeFile.LineLabelText = labelTextNode != null ? labelTextNode.InnerText : "";
                    XmlNode? labelFontSizeNode = shp.SelectSingleNode("LabelFontSize");
                    shapeFile.LineLabelFontSize = labelFontSizeNode != null ? labelFontSizeNode.InnerText : "8";
                    XmlNode? labelColorNode = shp.SelectSingleNode("LabelColor");
                    shapeFile.LineLabelColor = labelColorNode != null ? labelColorNode.InnerText : "#000000";
                    XmlNode? labelHANode = shp.SelectSingleNode("LabelHA");
                    shapeFile.LineLabelHA = labelHANode != null ? labelHANode.InnerText : "Left";
                    XmlNode? labelVANode = shp.SelectSingleNode("LabelVA");
                    shapeFile.LineLabelVA = labelVANode != null ? labelVANode.InnerText : "Center";
                    XmlNode? labelOffsetPointsXNode = shp.SelectSingleNode("LabelOffsetPointsX");
                    shapeFile.LineLabelOffsetPointsX = labelOffsetPointsXNode != null ? labelOffsetPointsXNode.InnerText : "0";
                    XmlNode? labelOffsetPointsYNode = shp.SelectSingleNode("LabelOffsetPointsY");
                    shapeFile.LineLabelOffsetPointsY = labelOffsetPointsYNode != null ? labelOffsetPointsYNode.InnerText : "0";
                    XmlNode? labelOffsetDataXNode = shp.SelectSingleNode("LabelOffsetDataX");
                    shapeFile.LineLabelOffsetDataX = labelOffsetDataXNode != null ? labelOffsetDataXNode.InnerText : "0";
                    XmlNode? labelOffsetDataYNode = shp.SelectSingleNode("LabelOffsetDataY");
                    shapeFile.LineLabelOffsetDataY = labelOffsetDataYNode != null ? labelOffsetDataYNode.InnerText : "0";
                }
                else if (shapeFile.Kind == "Polygon")
                {
                    XmlNode? polyEdgeColorNode = shp.SelectSingleNode("PolyEdgeColor");
                    shapeFile.PolyEdgeColor = polyEdgeColorNode != null ? polyEdgeColorNode.InnerText : "#000000";
                    XmlNode? polyLineWidthNode = shp.SelectSingleNode("PolyLineWidth");
                    shapeFile.PolyLineWidth = polyLineWidthNode != null ? polyLineWidthNode.InnerText : "0.8";
                    XmlNode? polyFaceColorNode = shp.SelectSingleNode("PolyFaceColor");
                    shapeFile.PolyFaceColor = polyFaceColorNode != null ? polyFaceColorNode.InnerText : "#000000";
                    XmlNode? polyAlphaNode = shp.SelectSingleNode("PolyAlpha");
                    shapeFile.PolyAlpha = polyAlphaNode != null ? polyAlphaNode.InnerText : "1.0";
                    XmlNode? labelTextNode = shp.SelectSingleNode("LabelText");
                    shapeFile.PolyLabelText = labelTextNode != null ? labelTextNode.InnerText : "";
                    XmlNode? labelFontSizeNode = shp.SelectSingleNode("LabelFontSize");
                    shapeFile.PolyLabelFontSize = labelFontSizeNode != null ? labelFontSizeNode.InnerText : "8";
                    XmlNode? labelColorNode = shp.SelectSingleNode("LabelColor");
                    shapeFile.PolyLabelColor = labelColorNode != null ? labelColorNode.InnerText : "#000000";
                    XmlNode? labelHANode = shp.SelectSingleNode("LabelHA");
                    shapeFile.PolyLabelHA = labelHANode != null ? labelHANode.InnerText : "Left";
                    XmlNode? labelVANode = shp.SelectSingleNode("LabelVA");
                    shapeFile.PolyLabelVA = labelVANode != null ? labelVANode.InnerText : "Center";
                    XmlNode? labelOffsetPointsXNode = shp.SelectSingleNode("LabelOffsetPointsX");
                    shapeFile.PolyLabelOffsetPointsX = labelOffsetPointsXNode != null ? labelOffsetPointsXNode.InnerText : "0";
                    XmlNode? labelOffsetPointsYNode = shp.SelectSingleNode("LabelOffsetPointsY");
                    shapeFile.PolyLabelOffsetPointsY = labelOffsetPointsYNode != null ? labelOffsetPointsYNode.InnerText : "0";
                    XmlNode? labelOffsetDataXNode = shp.SelectSingleNode("LabelOffsetDataX");
                    shapeFile.PolyLabelOffsetDataX = labelOffsetDataXNode != null ? labelOffsetDataXNode.InnerText : "0";
                    XmlNode? labelOffsetDataYNode = shp.SelectSingleNode("LabelOffsetDataY");
                    shapeFile.PolyLabelOffsetDataY = labelOffsetDataYNode != null ? labelOffsetDataYNode.InnerText : "0";
                }

                bool isChecked = shp.GetAttribute("visible") == "true";
                AddShapeFileToTree(shapeFile, isChecked);
            }
        }

        private void PopulateFields()
        {
            populate2DMapSetting();
            populate3DMapSetting();
            PopulateShapefileSetting();
            isSaved = true; // Initially, fields are populated and considered saved
        }

        public MapOptions()
        {
            InitializeComponent();
            table2D.Controls.Add(combo2Dcmap, 1, 2);
            table3D.Controls.Add(combo3Dcmap, 1, 2);
            InitializeShpTab();
            mapSettings = _ClassConfigurationManager.GetObject("MapSettings", "2");
            if (mapSettings == null)
            {
                MessageBox.Show("MapSettings with ID=2 not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            mapSettings2D = mapSettings.SelectSingleNode("Map2D") as XmlElement;
            mapSettings3D = mapSettings.SelectSingleNode("Map3D") as XmlElement;
            mapShapefiles = mapSettings.SelectSingleNode("MapShapefiles") as XmlElement;
            treeSurveys2D.CheckBoxes = true;
            treeSurveys3D.CheckBoxes = true;
            treeShapefiles.CheckBoxes = true;
            PopulateFields();
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    text: "You have unsaved changes. Do you want to save before exiting?",
                    caption: "Confirm Exit",
                    buttons: MessageBoxButtons.YesNoCancel,
                    icon: MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveSettings();
                    this.Close();
                    return;

                }
                else if (result == DialogResult.Cancel)
                {
                    return; // User chose to cancel
                }
                else
                {
                    this.Close(); // User chose not to save, proceed with exit  
                }
            }
            else
            {
                this.Close(); // Close the form if there are no unsaved changes
            }
        }

        private void MapOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    text: "You have unsaved changes. Do you want to save before exiting?",
                    caption: "Confirm Exit",
                    buttons: MessageBoxButtons.YesNoCancel,
                    icon: MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveSettings();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return; // User chose to cancel
                }
            }
        }

        private void combo2DBins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo2DBins.SelectedIndex == 0)
            {
                num2DBins.Visible = true;
                txt2DDepth.Visible = false;
                num2DBins.Enabled = !check2DBins.Checked;
            }
            else
            {
                num2DBins.Visible = false;
                txt2DDepth.Visible = true;
                txt2DDepth.Enabled = !check2DBins.Checked;
            }
            isSaved = false;
        }

        private void check2DBins_CheckedChanged(object sender, EventArgs e)
        {
            if (check2DBins.Checked)
            {
                num2DBins.Enabled = false;
                txt2DDepth.Enabled = false;
            }
            else
            {
                num2DBins.Enabled = true;
                txt2DDepth.Enabled = true;
            }
            isSaved = false;
        }

        private void check2DBeams_CheckedChanged(object sender, EventArgs e)
        {
            if (check2DBins.Checked)
            {
                num2DBeams.Enabled = false;
            }
            else
            {
                num2DBeams.Enabled = true;
            }
            isSaved = false;
        }
        private static bool SetNode(XmlElement? parent, string nodeName, string value)
        {
            if (parent == null)
                return false;
            XmlNode? node = parent.SelectSingleNode(nodeName);
            if (node == null)
            {
                node = _Globals.Config.CreateElement(nodeName);
                parent.AppendChild(node);
            }
            node.InnerText = value;
            return true;
        }
        private int SaveSettings2D()
        {
            if (mapSettings2D == null) return 0;
            if (!SetNode(mapSettings2D, "BackgroundColor", ColorTranslator.ToHtml(pnl2DBackColor.BackColor))) return 0;
            if (!SetNode(mapSettings2D, "FieldName", getValueByIndex(combo2DFieldName, combo2DFieldName.SelectedIndex))) return 0;
            if (!SetNode(mapSettings2D, "Colormap", ((ColormapItem)combo2Dcmap.SelectedItem).Name)) return 0;
            if (!SetNode(mapSettings2D, "vmin", txt2Dvmin.Text)) return 0;
            if (!SetNode(mapSettings2D, "vmax", txt2Dvmax.Text)) return 0;
            if (!SetNode(mapSettings2D, "Padding", txt2DPagDeg.Text)) return 0;
            if (!SetNode(mapSettings2D, "NGridLines", num2DNGridLines.Value.ToString())) return 0;
            if (!SetNode(mapSettings2D, "GridOpacity", txt2DGridOpacity.Text)) return 0;
            if (!SetNode(mapSettings2D, "GridColor", ColorTranslator.ToHtml(pnl2DGridColor.BackColor))) return 0;
            if (!SetNode(mapSettings2D, "GridWidth", txt2DGridWidth.Text)) return 0;
            if (!SetNode(mapSettings2D, "NAxisTicks", num2DNAxisTicks.Value.ToString())) return 0;
            if (!SetNode(mapSettings2D, "TickFontSize", num2DTickFontSize.Value.ToString())) return 0;
            if (!SetNode(mapSettings2D, "TickNDecimals", num2DNTicksDecimals.Value.ToString())) return 0;
            if (!SetNode(mapSettings2D, "AxisLabelFontSize", num2DAxisLabelFontSize.Value.ToString())) return 0;
            if (!SetNode(mapSettings2D, "AxisLabelColor", ColorTranslator.ToHtml(pnl2DAxisLabelColor.BackColor))) return 0;
            if (!SetNode(mapSettings2D, "HoverFontSize", num2DHoverFontSize.Value.ToString())) return 0;
            if (!SetNode(mapSettings2D, "TransectLineWidth", txt2DTransectLineWidth.Text)) return 0;
            if (!SetNode(mapSettings2D, "VerticalAggBinItem", getValueByIndex(combo2DBins, combo2DBins.SelectedIndex))) return 0;
            if (!SetNode(mapSettings2D, "VerticalAggBinTarget",
                check2DBins.Checked ? "Mean" :
                combo2DBins.SelectedIndex == 0 ? num2DBins.Value.ToString() : txt2DDepth.Text)) return 0;
            if (!SetNode(mapSettings2D, "VerticalAggBeam",
                check2DBeams.Checked ? "Mean" : num2DBeams.Value.ToString())) return 0;

            // Surveys
            XmlNode? surveysNode = mapSettings2D.SelectSingleNode("Surveys");
            if (surveysNode != null)
            {
                XmlNodeList? surveys = surveysNode.SelectNodes("Survey");
                if (surveys != null && surveys.Count > 0)
                {
                    foreach (XmlNode s in surveys)
                    {
                        surveysNode.RemoveChild(s);
                    }
                }
            }
            else
            {
                surveysNode = _Globals.Config.CreateElement("Surveys");
                mapSettings2D.AppendChild(surveysNode);
            }
            foreach (TreeNode surveyNode in treeSurveys2D.Nodes)
            {
                if (surveyNode.Checked)
                {
                    if (surveyNode.Tag is XmlElement surveyElem)
                    {
                        string id = surveyElem.GetAttribute("id");
                        XmlElement newSurveyNode = _Globals.Config.CreateElement("Survey");
                        newSurveyNode.InnerText = id;
                        surveysNode.AppendChild(newSurveyNode);
                    }
                }
            }
            return 1;
        }

        private int SaveSettings3D()
        {
            if (mapSettings3D == null) return 0;
            if (!SetNode(mapSettings3D, "BackgroundColor", ColorTranslator.ToHtml(pnl3DBackColor.BackColor))) return 0;
            if (!SetNode(mapSettings3D, "FieldName", getValueByIndex(combo3DFieldName, combo3DFieldName.SelectedIndex))) return 0;
            if (!SetNode(mapSettings3D, "Colormap", ((ColormapItem)combo3Dcmap.SelectedItem).Name)) return 0;
            if (!SetNode(mapSettings3D, "vmin", txt3Dvmin.Text)) return 0;
            if (!SetNode(mapSettings3D, "vmax", txt3Dvmax.Text)) return 0;
            if (!SetNode(mapSettings3D, "Padding", txt3DPagDeg.Text)) return 0;
            if (!SetNode(mapSettings3D, "NGridLines", num3DNGridLines.Value.ToString())) return 0;
            if (!SetNode(mapSettings3D, "GridOpacity", txt3DGridOpacity.Text)) return 0;
            if (!SetNode(mapSettings3D, "GridColor", ColorTranslator.ToHtml(pnl3DGridColor.BackColor))) return 0;
            if (!SetNode(mapSettings3D, "GridWidth", txt3DGridWidth.Text)) return 0;
            if (!SetNode(mapSettings3D, "NAxisTicks", num3DNAxisTicks.Value.ToString())) return 0;
            if (!SetNode(mapSettings3D, "TickFontSize", num3DTickFontSize.Value.ToString())) return 0;
            if (!SetNode(mapSettings3D, "TickNDecimals", num3DNTicksDecimals.Value.ToString())) return 0;
            if (!SetNode(mapSettings3D, "AxisLabelFontSize", num3DAxisLabelFontSize.Value.ToString())) return 0;
            if (!SetNode(mapSettings3D, "AxisLabelColor", ColorTranslator.ToHtml(pnl3DAxisLabelColor.BackColor))) return 0;
            if (!SetNode(mapSettings3D, "HoverFontSize", num3DHoverFontSize.Value.ToString())) return 0;
            if (!SetNode(mapSettings3D, "ZScale", txt3DZScale.Text)) return 0;


            // Surveys
            XmlNode? surveysNode = mapSettings3D.SelectSingleNode("Surveys");
            if (surveysNode != null)
            {
                XmlNodeList? surveys = surveysNode.SelectNodes("Survey");
                if (surveys != null && surveys.Count > 0)
                {
                    foreach (XmlNode s in surveys)
                    {
                        surveysNode.RemoveChild(s);
                    }
                }
            }
            else
            {
                surveysNode = _Globals.Config.CreateElement("Surveys");
                mapSettings3D.AppendChild(surveysNode);
            }
            foreach (TreeNode surveyNode in treeSurveys3D.Nodes)
            {
                if (surveyNode.Checked)
                {
                    if (surveyNode.Tag is XmlElement surveyElem)
                    {
                        string id = surveyElem.GetAttribute("id");
                        XmlElement newSurveyNode = _Globals.Config.CreateElement("Survey");
                        newSurveyNode.InnerText = id;
                        surveysNode.AppendChild(newSurveyNode);
                    }
                }
            }
            return 1;
        }

        private void SaveSettingsShp()
        {
            if (mapShapefiles == null) return;
            // Shapefiles
            XmlNodeList? shapefilesNode = mapShapefiles.SelectNodes("Shapefile");
            if (shapefilesNode != null)
            {
                foreach (XmlNode shpNode in shapefilesNode)
                {
                    mapShapefiles.RemoveChild(shpNode);
                }
            }
            foreach (TreeNode shpNode in treeShapefiles.Nodes)
            {
                if (shpNode.Tag is ShapeFile shp)
                {
                    XmlElement newShpNode = _Globals.Config.CreateElement("Shapefile");
                    newShpNode.SetAttribute("visible", shpNode.Checked ? "true" : "false");

                    if (!SetNode(newShpNode, "Name", shp.Name)) continue;
                    if (!SetNode(newShpNode, "Path", shp.Path)) continue;
                    if (!SetNode(newShpNode, "Kind", shp.Kind)) continue;

                    if (shp.Kind == "Point")
                    {
                        if (!SetNode(newShpNode, "PointColor", shp.PointColor)) continue;
                        if (!SetNode(newShpNode, "PointMarker", shp.PointMarker)) continue;
                        if (!SetNode(newShpNode, "PointMarkerSize", shp.PointMarkerSize)) continue;
                        if (!SetNode(newShpNode, "PointAlpha", shp.PointAlpha)) continue;
                        if (!SetNode(newShpNode, "LabelText", shp.PointLabelText)) continue;
                        if (!SetNode(newShpNode, "LabelFontSize", shp.PointLabelFontSize)) continue;
                        if (!SetNode(newShpNode, "LabelColor", shp.PointLabelColor)) continue;
                        if (!SetNode(newShpNode, "LabelHA", shp.PointLabelHA)) continue;
                        if (!SetNode(newShpNode, "LabelVA", shp.PointLabelVA)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetPointsX", shp.PointLabelOffsetPointsX)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetPointsY", shp.PointLabelOffsetPointsY)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetDataX", shp.PointLabelOffsetDataX)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetDataY", shp.PointLabelOffsetDataY)) continue;
                    }
                    else if (shp.Kind == "Line")
                    {
                        if (!SetNode(newShpNode, "LineColor", shp.LineColor)) continue;
                        if (!SetNode(newShpNode, "LineLineWidth", shp.LineLineWidth)) continue;
                        if (!SetNode(newShpNode, "LineAlpha", shp.LineAlpha)) continue;
                        if (!SetNode(newShpNode, "LabelText", shp.LineLabelText)) continue;
                        if (!SetNode(newShpNode, "LabelFontSize", shp.LineLabelFontSize)) continue;
                        if (!SetNode(newShpNode, "LabelColor", shp.LineLabelColor)) continue;
                        if (!SetNode(newShpNode, "LabelHA", shp.LineLabelHA)) continue;
                        if (!SetNode(newShpNode, "LabelVA", shp.LineLabelVA)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetPointsX", shp.LineLabelOffsetPointsX)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetPointsY", shp.LineLabelOffsetPointsY)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetDataX", shp.LineLabelOffsetDataX)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetDataY", shp.LineLabelOffsetDataY)) continue;
                    }
                    else if (shp.Kind == "Polygon")
                    {
                        if (!SetNode(newShpNode, "PolyEdgeColor", shp.PolyEdgeColor)) continue;
                        if (!SetNode(newShpNode, "PolyLineWidth", shp.PolyLineWidth)) continue;
                        if (!SetNode(newShpNode, "PolyFaceColor", shp.PolyFaceColor)) continue;
                        if (!SetNode(newShpNode, "PolyAlpha", shp.PolyAlpha)) continue;
                        if (!SetNode(newShpNode, "LabelText", shp.PolyLabelText)) continue;
                        if (!SetNode(newShpNode, "LabelFontSize", shp.PolyLabelFontSize)) continue;
                        if (!SetNode(newShpNode, "LabelColor", shp.PolyLabelColor)) continue;
                        if (!SetNode(newShpNode, "LabelHA", shp.PolyLabelHA)) continue;
                        if (!SetNode(newShpNode, "LabelVA", shp.PolyLabelVA)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetPointsX", shp.PolyLabelOffsetPointsX)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetPointsY", shp.PolyLabelOffsetPointsY)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetDataX", shp.PolyLabelOffsetDataX)) continue;
                        if (!SetNode(newShpNode, "LabelOffsetDataY", shp.PolyLabelOffsetDataY)) continue;
                    }
                    mapShapefiles.AppendChild(newShpNode);
                }
            }
        }

        private void SaveSettings()
        {
            SaveSettings2D();
            SaveSettings3D();
            SaveSettingsShp();
            string name = _ClassConfigurationManager.GetSetting("Name");
            string mapViewer2DPath = Path.Combine(_Globals.dataPath, "PlumeTrack", $"MapViewer2D_{name}.html");
            string mapViewer3DPath = Path.Combine(_Globals.dataPath, "PlumeTrack", $"MapViewer3D_{name}.html");
            if (File.Exists(mapViewer2DPath))
                File.Delete(mapViewer2DPath);
            if (File.Exists(mapViewer3DPath))
                File.Delete(mapViewer3DPath);
            isSaved = true;
            _project.SaveConfig();
        }

        private int AddShapeFileTable(ShapeFile shp)
        {
            splitShp.Panel2.Controls.Clear();
            if (shp.Kind == "Point")
            {
                splitShp.Panel2.Controls.Add(tableShpPoint);
                txtShpPointPath.Text = shp.Path;
                txtShpPointType.Text = shp.Kind;
                pnlShpPointColor.BackColor = ColorTranslator.FromHtml(shp.PointColor);
                selectComboByValue(comboShpPointMarker, shp.PointMarker);
                numShpPointMarkerSize.Value = int.Parse(shp.PointMarkerSize);
                numShpPointMarkerAlpha.Value = decimal.Parse(shp.PointAlpha);
                txtShpPointLabelText.Text = shp.PointLabelText;
                numShpPointLabelFontSize.Value = int.Parse(shp.PointLabelFontSize);
                pnlShpPointLabelColor.BackColor = ColorTranslator.FromHtml(shp.PointLabelColor);
                selectComboByValue(comboShpPointLabelHA, shp.PointLabelHA);
                selectComboByValue(comboShpPointLabelVA, shp.PointLabelVA);
                txtShpPointLabelOffsetPointsX.Text = shp.PointLabelOffsetPointsX;
                txtShpPointLabelOffsetPointsY.Text = shp.PointLabelOffsetPointsY;
                txtShpPointLabelOffsetDataX.Text = shp.PointLabelOffsetDataX;
                txtShpPointLabelOffsetDataY.Text = shp.PointLabelOffsetDataY;
            }
            else if (shp.Kind == "Line")
            {
                splitShp.Panel2.Controls.Add(tableShpLine);
                txtShpLinePath.Text = shp.Path;
                txtShpLineType.Text = shp.Kind;
                pnlShpLineColor.BackColor = ColorTranslator.FromHtml(shp.LineColor);
                txtShpLineLineWidth.Text = shp.LineLineWidth;
                numShpLineAlpha.Value = decimal.Parse(shp.LineAlpha);
                txtShpLineLabelText.Text = shp.LineLabelText;
                numShpLineLabelFontSize.Value = int.Parse(shp.LineLabelFontSize);
                pnlShpLineLabelColor.BackColor = ColorTranslator.FromHtml(shp.LineLabelColor);
                selectComboByValue(comboShpLineLabelHA, shp.LineLabelHA);
                selectComboByValue(comboShpLineLabelVA, shp.LineLabelVA);
                txtShpLineLabelOffsetPointsX.Text = shp.LineLabelOffsetPointsX;
                txtShpLineLabelOffsetPointsY.Text = shp.LineLabelOffsetPointsY;
                txtShpLineLabelOffsetDataX.Text = shp.LineLabelOffsetDataX;
                txtShpLineLabelOffsetDataY.Text = shp.LineLabelOffsetDataY;
            }

            else if (shp.Kind == "Polygon")
            {
                splitShp.Panel2.Controls.Add(tableShpPoly);
                txtShpPolyPath.Text = shp.Path;
                txtShpPolyType.Text = shp.Kind;
                pnlShpPolyEdgeColor.BackColor = ColorTranslator.FromHtml(shp.PolyEdgeColor);
                txtShpPolyLineWidth.Text = shp.PolyLineWidth;
                pnlShpPolyFaceColor.BackColor = ColorTranslator.FromHtml(shp.PolyFaceColor);
                numShpPolyAlpha.Value = decimal.Parse(shp.PolyAlpha);
                txtShpPolyLabelText.Text = shp.PolyLabelText;
                numShpPolyLabelFontSize.Value = int.Parse(shp.PolyLabelFontSize);
                pnlShpPolyLabelColor.BackColor = ColorTranslator.FromHtml(shp.PolyLabelColor);
                selectComboByValue(comboShpPolyLabelHA, shp.PolyLabelHA);
                selectComboByValue(comboShpPolyLabelVA, shp.PolyLabelVA);
                txtShpPolyLabelOffsetPointsX.Text = shp.PolyLabelOffsetPointsX;
                txtShpPolyLabelOffsetPointsY.Text = shp.PolyLabelOffsetPointsY;
                txtShpPolyLabelOffsetDataX.Text = shp.PolyLabelOffsetDataX;
                txtShpPolyLabelOffsetDataY.Text = shp.PolyLabelOffsetDataY;
            }
            else
            {
                return 0;
            }
            return 1;
        }

        private void AddShapeFileToTree(ShapeFile shp, bool Checked)
        {
            TreeNode nodeElement = new(shp.Name)
            {
                Tag = shp
            };
            treeShapefiles.Nodes.Add(nodeElement);
            nodeElement.Checked = Checked;
            treeShapefiles.SelectedNode = nodeElement;
            treeShapefiles.Focus();
            TreeNodeMouseClickEventArgs args = new(nodeElement, MouseButtons.Left, 1, 0, 0);
            treeShapefiles_NodeMouseClick(treeShapefiles, args);
            AddShapeFileTable(shp);
        }

        private void btnShpAddShapefile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Title = "Select Shapefile",
                Filter = "Shapefiles (*.shp)|*.shp",
                Multiselect = false,
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = ofd.FileName;
                string selectedType = GetShapeType(selectedPath);
                string selectedName = Path.GetFileNameWithoutExtension(selectedPath);
                ShapeFile shp = new(name: selectedName, path: selectedPath, kind: selectedType);
                AddShapeFileToTree(shp, true);
            }
        }

        private void colorButton_Click(object? sender, EventArgs e)
        {
            if (sender == null) return;
            if (sender is not Button btn) return;
            Panel? pnl;
            if (btn.Name == "btn2DBackColor")
                pnl = pnl2DBackColor;
            else if (btn.Name == "btn2DGridColor")
                pnl = pnl2DGridColor;
            else if (btn.Name == "btn2DAxisLabelColor")
                pnl = pnl2DAxisLabelColor;
            else if (btn.Name == "btn3DBackColor")
                pnl = pnl3DBackColor;
            else if (btn.Name == "btn3DGridColor")
                pnl = pnl3DGridColor;
            else if (btn.Name == "btn3DAxisLabelColor")
                pnl = pnl3DAxisLabelColor;
            else if (btn.Name == "btnShpPointColor")
                pnl = pnlShpPointColor;
            else if (btn.Name == "btnShpPointLabelColor")
                pnl = pnlShpPointLabelColor;
            else if (btn.Name == "btnShpLineLabelColor")
                pnl = pnlShpLineLabelColor;
            else if (btn.Name == "btnShpPolyLabelColor")
                pnl = pnlShpPolyLabelColor;
            else if (btn.Name == "btnShpLineColor")
                pnl = pnlShpLineColor;
            else if (btn.Name == "btnShpPolyEdgeColor")
                pnl = pnlShpPolyEdgeColor;
            else if (btn.Name == "btnShpPolyFaceColor")
                pnl = pnlShpPolyFaceColor;
            else
                return;
            ColorDialog colorDialog = new()
            {
                AllowFullOpen = true,
                AnyColor = true,
                FullOpen = true,
                Color = pnl.BackColor,
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                pnl.BackColor = colorDialog.Color;
            }
        }

        private void pnlBackColor_Changed(object? sender, EventArgs e)
        {
            if (sender == null) return;
            if (sender is not Panel panel) return;
            if (panel.Name.Contains("Shp"))
            {
                if (treeShapefiles.SelectedNode?.Tag is not ShapeFile shp) return;
                if (panel.Name == "pnlShpPointColor")
                    shp.PointColor = ColorTranslator.ToHtml(panel.BackColor);
                else if (panel.Name == "pnlShpPointLabelColor")
                    shp.PointLabelColor = ColorTranslator.ToHtml(panel.BackColor);
                else if (panel.Name == "pnlShpLineLabelColor")
                    shp.LineLabelColor = ColorTranslator.ToHtml(panel.BackColor);
                else if (panel.Name == "pnlShpPolyLabelColor")
                    shp.PolyLabelColor = ColorTranslator.ToHtml(panel.BackColor);
                else if (panel.Name == "pnlShpLineColor")
                    shp.LineColor = ColorTranslator.ToHtml(panel.BackColor);
                else if (panel.Name == "pnlShpPolyEdgeColor")
                    shp.PolyEdgeColor = ColorTranslator.ToHtml(panel.BackColor);
                else if (panel.Name == "pnlShpPolyFaceColor")
                    shp.PolyFaceColor = ColorTranslator.ToHtml(panel.BackColor);
            }
            isSaved = false;
        }

        private void treeShapefiles_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is not ShapeFile shp) return;

            AddShapeFileTable(shp);
        }

        private void input_Changed(object? sender, EventArgs e)
        {
            isSaved = false;
        }

        private void txtShp_TextChanged(object? sender, EventArgs e)
        {
            if (sender == null) return;
            if (sender is not TextBox txt) return;
            if (treeShapefiles.SelectedNode?.Tag is not ShapeFile shp) return;
            if (txt.Name == "txtShpPointPath" || txt.Name == "txtShpLinePath" || txt.Name == "txtShpPolyPath")
                shp.Path = txt.Text;
            else if (txt.Name == "txtShpPointType" || txt.Name == "txtShpLineType" || txt.Name == "txtShpPolyType")
                shp.Kind = txt.Text;
            else if (txt.Name == "txtShpLineLineWidth")
                shp.LineLineWidth = txt.Text;
            else if (txt.Name == "txtShpPolyLineWidth")
                shp.PolyLineWidth = txt.Text;
            else if (txt.Name == "txtShpPointLabelText")
                shp.PointLabelText = txt.Text;
            else if (txt.Name == "txtShpLineLabelText")
                shp.LineLabelText = txt.Text;
            else if (txt.Name == "txtShpPolyLabelText")
                shp.PolyLabelText = txt.Text;
            else if (txt.Name == "txtShpPointLabelOffsetPointsX")
                shp.PointLabelOffsetPointsX = txt.Text;
            else if (txt.Name == "txtShpLineLabelOffsetPointsX")
                shp.LineLabelOffsetPointsX = txt.Text;
            else if (txt.Name == "txtShpPolyLabelOffsetPointsX")
                shp.PolyLabelOffsetPointsX = txt.Text;
            else if (txt.Name == "txtShpPointLabelOffsetPointsY")
                shp.PointLabelOffsetPointsY = txt.Text;
            else if (txt.Name == "txtShpLineLabelOffsetPointsY")
                shp.LineLabelOffsetPointsY = txt.Text;
            else if (txt.Name == "txtShpPolyLabelOffsetPointsY")
                shp.PolyLabelOffsetPointsY = txt.Text;
            else if (txt.Name == "txtShpPointLabelOffsetDataX")
                shp.PointLabelOffsetDataX = txt.Text;
            else if (txt.Name == "txtShpLineLabelOffsetDataX")
                shp.LineLabelOffsetDataX = txt.Text;
            else if (txt.Name == "txtShpPolyLabelOffsetDataX")
                shp.PolyLabelOffsetDataX = txt.Text;
            else if (txt.Name == "txtShpPointLabelOffsetDataY")
                shp.PointLabelOffsetDataY = txt.Text;
            else if (txt.Name == "txtShpLineLabelOffsetDataY")
                shp.LineLabelOffsetDataY = txt.Text;
            else if (txt.Name == "txtShpPolyLabelOffsetDataY")
                shp.PolyLabelOffsetDataY = txt.Text;
            isSaved = false;
        }

        private void comboShp_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (sender == null) return;
            if (sender is not ComboBox combo) return;
            if (treeShapefiles.SelectedNode?.Tag is not ShapeFile shp) return;
            if (combo.Name == "comboShpPointMarker")
                shp.PointMarker = combo.SelectedItem?.ToString() ?? "o";
            else if (combo.Name == "comboShpPointLabelHA")
                shp.PointLabelHA = combo.SelectedItem?.ToString() ?? "Left";
            else if (combo.Name == "comboShpLineLabelHA")
                shp.LineLabelHA = combo.SelectedItem?.ToString() ?? "Left";
            else if (combo.Name == "comboShpPolyLabelHA")
                shp.PolyLabelHA = combo.SelectedItem?.ToString() ?? "Left";
            else if (combo.Name == "comboShpPointLabelVA")
                shp.PointLabelVA = combo.SelectedItem?.ToString() ?? "Center";
            else if (combo.Name == "comboShpLineLabelVA")
                shp.LineLabelVA = combo.SelectedItem?.ToString() ?? "Center";
            else if (combo.Name == "comboShpPolyLabelVA")
                shp.PolyLabelVA = combo.SelectedItem?.ToString() ?? "Center";
            isSaved = false;
        }

        private void numShp_ValueChanged(object? sender, EventArgs e)
        {
            if (sender == null) return;
            if (sender is not NumericUpDown num) return;
            if (treeShapefiles.SelectedNode?.Tag is not ShapeFile shp) return;
            if (num.Name == "numShpPointMarkerSize")
                shp.PointMarkerSize = num.Value.ToString();
            else if (num.Name == "numShpPointMarkerAlpha")
                shp.PointAlpha = num.Value.ToString();
            else if (num.Name == "numShpLineAlpha")
                shp.LineAlpha = num.Value.ToString();
            else if (num.Name == "numShpPolyAlpha")
                shp.PolyAlpha = num.Value.ToString();
            else if (num.Name == "numShpPointLabelFontSize")
                shp.PointLabelFontSize = num.Value.ToString();
            else if (num.Name == "numShpLineLabelFontSize")
                shp.LineLabelFontSize = num.Value.ToString();
            else if (num.Name == "numShpPolyLabelFontSize")
                shp.PolyLabelFontSize = num.Value.ToString();
            isSaved = false;
        }

        private void treeShapefiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeShapefiles.SelectedNode != null)
            {
                if (treeShapefiles.SelectedNode.Tag is ShapeFile shp)
                {
                    AddShapeFileTable(shp);
                }
            }
            else
            {
                splitShp.Panel2.Controls.Clear();
            }
        }

    }
}
