using DHI.Generic.MikeZero;
using DHI.Generic.MikeZero.DFS;
using DHI.Generic.MikeZero.DFS.dfs123;
using DHI.Generic.MikeZero.DFS.dfsu;
using DHI.Generic.MikeZero.DFS.mesh;
using Plume_Track;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;

namespace Plume_Track
{
    public partial class ModelHDPlot : Form
    {
        public XmlElement? model;
        public _ClassConfigurationManager _project = new();

        private Label lblTitle;
        private TextBox txtTitle;

        private Label lblFieldName;
        private ComboBox comboFieldName;

        private Label lblyAxisMode;
        private ComboBox comboyAxisMode;

        private Label lblcmap;
        private ComboBox combocmap;

        private Label lblvmin;
        private TextBox txtvmin;

        private Label lblvmax;
        private TextBox txtvmax;

        private Label lblMask;
        private ComboBox comboMask;

        private Label lblBinSelection;
        private NumericUpDown numericNBins;
        private CheckBox checkUseMean;

        private Label lblScale;
        private TextBox txtScale;

        private Label lblLineWidth;
        private TextBox txtLineWidth;

        private Label lblLineAlpha;
        private TextBox txtLineAlpha;

        private Label lblHistBins;
        private NumericUpDown numericHistBins;

        private Label lblBeamSelection;
        private NumericUpDown numericNBeams;

        private void PropMeshPlot()
        {
            tableProp.Controls.Clear();

            tableProp.RowStyles.Clear();
            tableProp.RowCount = 2;
            tableProp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableProp.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tableProp.Controls.Add(lblTitle, 0, 0);
            tableProp.Controls.Add(txtTitle, 1, 0);
        }

        private static Label CreateLabel(string name, string text)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Name = name,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }
        private static TextBox CreateTextBox(string name, int tabIndex, string text = "")
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Name = name,
                Text = text,
                TabIndex = tabIndex
            };
        }
        private static ComboBox CreateComboBox(string name, string[] items, int tabIndex, int selectedIndex = 0)
        {
            var comboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Name = name,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = tabIndex
            };
            comboBox.Items.AddRange(items);
            comboBox.SelectedIndex = selectedIndex;
            return comboBox;
        }
        private static NumericUpDown CreateNumericUpDown(string name, int tabIndex, decimal min, decimal max, decimal value, bool enabled = true)
        {
            return new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Name = name,
                TabIndex = tabIndex,
                Minimum = min,
                Maximum = max,
                Value = value,
                Enabled = enabled
            };
        }
        private static CheckBox CreateCheckBox(string name, int tabIndex, string text, bool isChecked)
        {
            return new CheckBox
            {
                Dock = DockStyle.Fill,
                Name = name,
                TabIndex = tabIndex,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Checked = isChecked
            };
        }
        private void InitializeWidgets()
        {
            lblTitle = CreateLabel("lblTitle", "Title");
            txtTitle = CreateTextBox("txtTitle", 0);
            lblFieldName = CreateLabel("lblFieldName", "Field Name");
            string[] fieldNames = { "Echo Intensity", "Correlation Magnitude", "Percent Good", "Absolute Backscatter", "Alpha s", "Alpha w", "Signal to Noise Ratio", "SSC" };
            comboFieldName = CreateComboBox("comboFieldName", fieldNames, 1, 0);
            lblyAxisMode = CreateLabel("lblyAxisMode", "y Axis Mode");
            string[] yAxisModes = { "Bin", "Depth" };
            comboyAxisMode = CreateComboBox("comboyAxisMode", yAxisModes, 8, 0);
            lblcmap = CreateLabel("lblcmap", "Colormap");
            // Placeholder for combocmap initialization
            lblvmin = CreateLabel("lblvmin", "Colormap Minimum");
            txtvmin = CreateTextBox("txtvmin", 11);
            lblvmax = CreateLabel("lblvmax", "Colormap Maximum");
            txtvmax = CreateTextBox("txtvmax", 12);
            lblMask = CreateLabel("lblMask", "Apply Masking?");
            string[] maskOptions = { "Yes", "No" };
            comboMask = CreateComboBox("comboMask", maskOptions, 10, 0);
            lblBinSelection = CreateLabel("lblBinSelection", "Bin Selection");
            numericNBins = CreateNumericUpDown("numericNBins", 13, 1, 1000, 1, false);
            checkUseMean = CreateCheckBox("checkUseMean", 15, "Use Mean", true);
            lblScale = CreateLabel("lblScale", "Vector Scale");
            txtScale = CreateTextBox("txtScale", 16, "0.005");
            lblLineWidth = CreateLabel("lblLineWidth", "Line Width");
            txtLineWidth = CreateTextBox("txtLineWidth", 17, "2.5");
            lblLineAlpha = CreateLabel("lblLineAlpha", "Line Alpha");
            txtLineAlpha = CreateTextBox("txtLineAlpha", 18, "0.9");
            lblHistBins = CreateLabel("lblHistBins", "Number of Histogram Bins");
            numericHistBins = CreateNumericUpDown("numericHistBins", 19, 1, 50, 20);
            lblBeamSelection = CreateLabel("lblBeamSelection", "Beam Selection");
            numericNBeams = CreateNumericUpDown("numericNBeams", 13, 1, 4, 1, false);
            // 
            // combocmap
            // 
            combocmap = new ComboBox();
            combocmap.Dock = DockStyle.Fill;
            combocmap.DropDownStyle = ComboBoxStyle.DropDownList;
            combocmap.FormattingEnabled = true;
            combocmap.Name = "combocmap";
            combocmap.TabIndex = 9;
            combocmap.DrawMode = DrawMode.OwnerDrawFixed;
            combocmap.ItemHeight = 24; // more room for image
            combocmap.Items.Add(new ColormapItem("autumn", Image.FromFile(Path.Combine(_Globals.CMapsPath, "autumn.png"))));
            combocmap.Items.Add(new ColormapItem("cividis", Image.FromFile(Path.Combine(_Globals.CMapsPath, "cividis.png"))));
            combocmap.Items.Add(new ColormapItem("cool", Image.FromFile(Path.Combine(_Globals.CMapsPath, "cool.png"))));
            combocmap.Items.Add(new ColormapItem("hot", Image.FromFile(Path.Combine(_Globals.CMapsPath, "hot.png"))));
            combocmap.Items.Add(new ColormapItem("inferno", Image.FromFile(Path.Combine(_Globals.CMapsPath, "inferno.png"))));
            combocmap.Items.Add(new ColormapItem("jet", Image.FromFile(Path.Combine(_Globals.CMapsPath, "jet.png"))));
            combocmap.Items.Add(new ColormapItem("magma", Image.FromFile(Path.Combine(_Globals.CMapsPath, "magma.png"))));
            combocmap.Items.Add(new ColormapItem("plasma", Image.FromFile(Path.Combine(_Globals.CMapsPath, "plasma.png"))));
            combocmap.Items.Add(new ColormapItem("spring", Image.FromFile(Path.Combine(_Globals.CMapsPath, "spring.png"))));
            combocmap.Items.Add(new ColormapItem("summer", Image.FromFile(Path.Combine(_Globals.CMapsPath, "summer.png"))));
            combocmap.Items.Add(new ColormapItem("turbo", Image.FromFile(Path.Combine(_Globals.CMapsPath, "turbo.png"))));
            combocmap.Items.Add(new ColormapItem("viridis", Image.FromFile(Path.Combine(_Globals.CMapsPath, "viridis.png"))));
            combocmap.Items.Add(new ColormapItem("winter", Image.FromFile(Path.Combine(_Globals.CMapsPath, "winter.png"))));
            combocmap.SelectedIndex = 0;
            combocmap.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;

                e.DrawBackground();
                var item = (ColormapItem)combocmap.Items[e.Index];

                int textColumnWidth = 100;  // fixed width reserved for text

                // Draw text (always in the same aligned column)
                using (Brush brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(
                        item.Name,
                        e.Font,
                        brush,
                        e.Bounds.Left + 2,
                        e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2
                    );
                }

                // Draw preview aligned to the same start point
                if (item.Preview != null)
                {
                    int imageX = e.Bounds.Left + textColumnWidth;
                    int imageWidth = e.Bounds.Right - imageX - 2;
                    int imageHeight = e.Bounds.Height - 4;

                    var imageRect = new Rectangle(imageX, e.Bounds.Top + 2, imageWidth, imageHeight);
                    e.Graphics.DrawImage(item.Preview, imageRect);
                }

                e.DrawFocusRectangle();
            };

        }

        public ModelHDPlot(string? id)
        {
            InitializeComponent();
            InitializeWidgets();
            comboPlotType.SelectedIndex = 0;
            model = _ClassConfigurationManager.GetObject(type: "HDModel", id: id);
            string path = model?.SelectSingleNode("Path")?.InnerText;
            if (!File.Exists(path))
            {
                MessageBox.Show(text: $"The specified model file does not exist: {path}", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                this.Close(); // Close the form if the file does not exist
            }
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            string path = model?.SelectSingleNode("Path")?.InnerText;
            Dictionary<string, string> inputs = null;
            if (comboPlotType.SelectedItem.ToString() == "Mesh Plot")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotModelMesh" },
                    { "Path", path },
                    { "Title", txtTitle.Text },

                };
            }
            //else if (comboPlotType.SelectedItem.ToString() == "Four Beam Flood Plot")
            //{
            //    inputs = new Dictionary<string, string>
            //    {
            //        { "Task", "PlotFourBeamFlood" },
            //        { "EPSG", _project.GetSetting("EPSG") },
            //        { "Water", waterProp.OuterXml.ToString() },
            //        { "Sediment", sedimentProp.OuterXml.ToString() },
            //        { "Instrument", adcp.OuterXml.ToString() },
            //        { "FieldName", comboFieldName.SelectedItem.ToString()},
            //        { "yAxisMode", comboyAxisMode.SelectedItem.ToString()},
            //        { "Colormap", combocmap.SelectedItem.ToString()},
            //        { "vmin", txtvmin.Text},
            //        { "vmax", txtvmax.Text},
            //        { "Title", txtTitle.Text},
            //        { "Mask", comboMask.SelectedItem.ToString()}
            //    };
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Single Beam Flood Plot")
            //{
            //    inputs = new Dictionary<string, string>
            //    {
            //        { "Task", "PlotSingleBeamFlood" },
            //        { "EPSG", _project.GetSetting("EPSG") },
            //        { "Water", waterProp.OuterXml.ToString() },
            //        { "Sediment", sedimentProp.OuterXml.ToString() },
            //        { "Instrument", adcp.OuterXml.ToString() },
            //        { "BeamSelection", numericNBeams.Value.ToString()},
            //        { "UseMean", checkUseMean.Checked ? "Yes" : "No"},
            //        { "FieldName", comboFieldName.SelectedItem.ToString()},
            //        { "yAxisMode", comboyAxisMode.SelectedItem.ToString()},
            //        { "Colormap", combocmap.SelectedItem.ToString()},
            //        { "vmin", txtvmin.Text},
            //        { "vmax", txtvmax.Text},
            //        { "Title", txtTitle.Text},
            //        { "Mask", comboMask.SelectedItem.ToString()}
            //    };
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Plot Transect Velocities")
            //{
            //    inputs = new Dictionary<string, string>
            //    {
            //        { "Task", "PlotTransectVelocities" },
            //        { "EPSG", _project.GetSetting("EPSG") },
            //        { "Water", waterProp.OuterXml.ToString() },
            //        { "Sediment", sedimentProp.OuterXml.ToString() },
            //        { "Instrument", adcp.OuterXml.ToString() },
            //        { "BinSelection", numericNBins.Value.ToString()},
            //        { "UseMean", checkUseMean.Checked ? "Yes" : "No"},
            //        { "VectorScale", txtScale.Text},
            //        { "Colormap", combocmap.SelectedItem.ToString()},
            //        { "vmin", txtvmin.Text},
            //        { "vmax", txtvmax.Text},
            //        { "Title", txtTitle.Text},
            //        { "LineWidth", txtLineWidth.Text},
            //        { "LineAlpha", txtLineAlpha.Text},
            //        { "HistBins", numericHistBins.Value.ToString()}
            //    };
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Beam Geometry Animation")
            //{
            //    inputs = new Dictionary<string, string>
            //    {
            //        { "Task", "PlotBeamGeometryAnimation" },
            //        { "EPSG", _project.GetSetting("EPSG") },
            //        { "Water", waterProp.OuterXml.ToString() },
            //        { "Sediment", sedimentProp.OuterXml.ToString() },
            //        { "Instrument", adcp.OuterXml.ToString() }
            //    };
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Transect Animation")
            //{
            //    inputs = new Dictionary<string, string>
            //    {
            //        { "Task", "PlotTransectAnimation" },
            //        { "EPSG", _project.GetSetting("EPSG") },
            //        { "Water", waterProp.OuterXml.ToString() },
            //        { "Sediment", sedimentProp.OuterXml.ToString() },
            //        { "Instrument", adcp.OuterXml.ToString() },
            //        { "Colormap", combocmap.SelectedItem.ToString()},
            //        { "vmin", txtvmin.Text},
            //        { "vmax", txtvmax.Text}
            //    };
            //}
            //else
            //{
            //    return;
            //}

            string xmlInput = _Tools.GenerateInput(inputs);
            XmlDocument result = _Tools.CallPython(xmlInput);
            Dictionary<string, string> outputs = _Tools.ParseOutput(result);
            if (outputs.ContainsKey("Error"))
            {
                MessageBox.Show(outputs["Error"], "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void comboPlotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPlotType.SelectedItem.ToString() == "Mesh Plot")
            {
                PropMeshPlot();
            }
            //else if (comboPlotType.SelectedItem.ToString() == "Four Beam Flood Plot")
            //{
            //    PropFourBeamFloodPlot();
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Single Beam Flood Plot")
            //{
            //    PropSingleBeamFloodPlot();
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Plot Transect Velocities")
            //{
            //    PlotTransectVelocities();
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Beam Geometry Animation")
            //{
            //    tableProp.Controls.Clear();
            //}
            //else if (comboPlotType.SelectedItem.ToString() == "Transect Animation")
            //{
            //    PlotTransectAnimation();
            //}
        }

        private void CheckUseMean_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkUseMean.Checked)
            {
                numericNBins.Enabled = false;
                numericNBeams.Enabled = false;
            }
            else
            {
                numericNBins.Enabled = true;
                numericNBeams.Enabled = true;
            }
        }
    }
}
