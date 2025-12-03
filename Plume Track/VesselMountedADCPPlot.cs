using NetTopologySuite.Geometries;
using Plume_Track;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Plume_Track
{
    public partial class VesselMountedADCPPlot : Form
    {
        public XmlElement? adcp;
        public XmlElement? survey;
        public string id = null!;
        public string type = null!;
        public _ClassConfigurationManager _project = new();
        
        private readonly Label lblTitle = _Utils.CreateLabel("lblTitle", "Title");
        private readonly TextBox txtTitle = _Utils.CreateTextBox("txtTitle");
        private readonly Label lblFieldName = _Utils.CreateLabel("lblFieldName", "Field Name");
        private readonly ComboBox comboFieldName = _Utils.CreateComboBox("comboFieldName", ["Echo Intensity", "Correlation Magnitude", "Percent Good", "Absolute Backscatter", "Signal to Noise Ratio", "SSC"]);
        private readonly Label lblxAxisMode = _Utils.CreateLabel("lblxAxisMode", "x Axis Mode");
        private readonly ComboBox comboxAxisMode = _Utils.CreateComboBox("comboxAxisMode", ["Ensemble", "Time"]);
        private readonly Label lblyAxisMode = _Utils.CreateLabel("lblyAxisMode", "y Axis Mode");
        private readonly ComboBox comboyAxisMode = _Utils.CreateComboBox("comboyAxisMode", ["Bin", "Depth"]);
        private readonly Label lblcmap = _Utils.CreateLabel("lblcmap", "Colormap");
        private readonly ColormapComboBox combocmap = new (_Globals.CMapsPath, "combocmap")
        {
            SelectedColormapName = "turbo"
        };
        private readonly Label lblvmin = _Utils.CreateLabel("lblvmin", "Colomrap Minimum");
        private readonly TextBox txtvmin = _Utils.CreateTextBox("txtvmin");
        private readonly Label lblvmax = _Utils.CreateLabel("lblvmax", "Colormap Maximum");
        private readonly TextBox txtvmax = _Utils.CreateTextBox("txtvmax");
        private readonly Label lblMask = _Utils.CreateLabel("lblMask", "Apply Masking?");
        private readonly ComboBox comboMask = _Utils.CreateComboBox("comboMask", ["Yes", "No"]);
        private readonly Label lblBinSelection = _Utils.CreateLabel("lblBinSelection", "Bin Selection");
        private readonly NumericUpDown numericNBins = _Utils.CreateNumericUpDown("numericNBins", 1, 1000, 1, false);
        private readonly CheckBox checkUseMean = _Utils.CreateCheckBox("checkUseMean", "Use Mean", true);
        private readonly Label lblScale = _Utils.CreateLabel("lblScale", "Vector Scale");
        private readonly TextBox txtScale = _Utils.CreateTextBox("txtScale", "0.005");
        private readonly Label lblLineWidth = _Utils.CreateLabel("lblLineWidth", "Line Width");
        private readonly TextBox txtLineWidth = _Utils.CreateTextBox("txtLineWidth", "2.5");
        private readonly Label lblLineAlpha = _Utils.CreateLabel("lblLineAlpha", "Line Alpha");
        private readonly TextBox txtLineAlpha = _Utils.CreateTextBox("txtLineAlpha", "0.9");
        private readonly Label lblHistBins = _Utils.CreateLabel("lblHistBins", "Number of Histogram Bins");
        private readonly NumericUpDown numericHistBins = _Utils.CreateNumericUpDown("numericHistBins", 1, 50, 20);
        private readonly Label lblBeamSelection = _Utils.CreateLabel("lblBeamSelection", "Beam Selection");
        private readonly NumericUpDown numericNBeams = _Utils.CreateNumericUpDown("numericNBeams", 1, 4, 1, false);
        private readonly Label lblVelFieldName = _Utils.CreateLabel("lblVelFieldName", "Field Name");
        private readonly ComboBox comboVelFieldName = _Utils.CreateComboBox("comboVelFieldName", ["Speed", "Direction", "Error Velocity"]);
        private readonly Label lblCoord = _Utils.CreateLabel("lblCoord", "Coordinate System");
        private readonly ComboBox comboCoord = _Utils.CreateComboBox("comboCoord", ["Instrument", "Ship", "Earth"]);
        private readonly Label lblAnimationOutputFile = _Utils.CreateLabel("lblAnimationOutputFile", "Animation Output File");
        private readonly TextBox txtAnimationOutputFile = _Utils.CreateTextBox("txtAnimationOutputFile");
        private readonly Button btnAnimationOutputFile = _Utils.CreateButton("btnAnimationOutputFile", "...");

        private void InitializeWidgets()
        {
            btnAnimationOutputFile.Click += BtnAnimationOutputFile_Click;
            checkUseMean.CheckedChanged += CheckUseMean_CheckedChanged;
        }

        private void PropPlatformOrientation()
        {
            IList<List<Control?>> controls = [
                [lblTitle, txtTitle]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F], true);
            txtTitle.TabIndex = 0;
            
        }

        private void PropFourBeamFloodPlot()
        {
            IList<List<Control?>> controls = [
                [lblFieldName, comboFieldName],
                [lblxAxisMode, comboxAxisMode],
                [lblyAxisMode, comboyAxisMode],
                [lblcmap, combocmap],
                [lblvmin, txtvmin],
                [lblvmax, txtvmax],
                [lblTitle, txtTitle],
                [lblMask, comboMask]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F, 30F, 30F, 36F, 30F, 30F, 30F, 30F], true);
            comboFieldName.TabIndex = 0;
            comboxAxisMode.TabIndex = 1;
            comboyAxisMode.TabIndex = 2;
            combocmap.TabIndex = 3;
            txtvmin.TabIndex = 4;
            txtvmax.TabIndex = 5;
            txtTitle.TabIndex = 6;
            comboMask.TabIndex = 7;
        }

        private void PropVelocityFloodPlot()
        {
            IList<List<Control?>> controls = [
                [lblVelFieldName, comboVelFieldName],
                [lblCoord, comboCoord],
                [lblxAxisMode, comboxAxisMode],
                [lblyAxisMode, comboyAxisMode],
                [lblcmap, combocmap],
                [lblvmin, txtvmin],
                [lblvmax, txtvmax],
                [lblTitle, txtTitle],
                [lblMask, comboMask]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F, 30F, 30F, 30F, 36F, 30F, 30F, 30F, 30F], true);
            comboVelFieldName.TabIndex = 0;
            comboCoord.TabIndex = 1;
            comboxAxisMode.TabIndex = 2;
            comboyAxisMode.TabIndex = 3;
            combocmap.TabIndex = 4;
            txtvmin.TabIndex = 5;
            txtvmax.TabIndex = 6;
            txtTitle.TabIndex = 7;
            comboMask.TabIndex = 8;
        }

        private void PropSingleBeamFloodPlot()
        {
            IList<List<Control?>> controls = [
                [lblBeamSelection, numericNBeams, checkUseMean],
                [lblFieldName, comboFieldName],
                [lblxAxisMode, comboxAxisMode],
                [lblyAxisMode, comboyAxisMode],
                [lblcmap, combocmap],
                [lblvmin, txtvmin],
                [lblvmax, txtvmax],
                [lblTitle, txtTitle],
                [lblMask, comboMask]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F, 30F, 30F, 30F, 36F, 30F, 30F, 30F, 30F], true);
            numericNBeams.TabIndex = 0;
            checkUseMean.TabIndex = 1;
            comboFieldName.TabIndex = 2;
            comboxAxisMode.TabIndex = 3;
            comboyAxisMode.TabIndex = 4;
            combocmap.TabIndex = 5;
            txtvmin.TabIndex = 6;
            txtvmax.TabIndex = 7;
            txtTitle.TabIndex = 8;
            comboMask.TabIndex = 9;
        }

        private void PlotTransectVelocities()
        {
            IList<List<Control?>> controls = [
                [lblBinSelection, numericNBins, checkUseMean],
                [lblScale, txtScale],
                [lblcmap, combocmap],
                [lblvmin, txtvmin],
                [lblvmax, txtvmax],
                [lblTitle, txtTitle],
                [lblLineWidth, txtLineWidth],
                [lblLineAlpha, txtLineAlpha],
                [lblHistBins, numericHistBins],
                [lblMask, comboMask]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F, 30F, 36F, 30F, 30F, 30F, 30F, 30F, 30F, 30F], true);
            numericNBins.TabIndex = 0;
            checkUseMean.TabIndex = 1;
            txtScale.TabIndex = 2;
            combocmap.TabIndex = 3;
            txtvmin.TabIndex = 4;
            txtvmax.TabIndex = 5;
            txtTitle.TabIndex = 6;
            txtLineWidth.TabIndex = 7;
            txtLineAlpha.TabIndex = 8;
            numericHistBins.TabIndex = 9;
            comboMask.TabIndex = 10;
        }

        private void PlotBeamGeometryAnimation()
        {
            IList<List<Control?>> controls = [
                [lblAnimationOutputFile, txtAnimationOutputFile, btnAnimationOutputFile]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F], true);
        }

        private void PlotTransectAnimation()
        {
            IList<List<Control?>> controls = [
                [lblcmap, combocmap],
                [lblvmin, txtvmin],
                [lblvmax, txtvmax],
                [lblAnimationOutputFile, txtAnimationOutputFile, btnAnimationOutputFile],
            ];
            _Utils.ConfigureTable(tableProp, controls, [36F, 30F, 30F, 30F], true);
            combocmap.TabIndex = 0;
            txtvmin.TabIndex = 1;
            txtvmax.TabIndex = 2;
            txtAnimationOutputFile.TabIndex = 3;
            btnAnimationOutputFile.TabIndex = 4;
        }

        public VesselMountedADCPPlot(string _id)
        {
            InitializeComponent();
            InitializeWidgets();
            comboPlotType.SelectedIndex = 0;
            id = _id;
            type = "VesselMountedADCP";
            adcp = _ClassConfigurationManager.GetObject(type: "VesselMountedADCP", id: id);
            XmlElement? pd0 = adcp?.SelectSingleNode("Pd0") as XmlElement;

            if (pd0?.SelectSingleNode("SSCModelID") is not XmlElement sscModelID || string.IsNullOrEmpty(sscModelID.InnerText))
            {
                comboFieldName.Items.Remove("SSC");
            }

        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> inputs;

            if (comboPlotType.SelectedItem?.ToString() == "Platform Orientation")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotPlatformOrientation" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty },
                    { "Type", type ?? string.Empty},
                    { "Title", txtTitle?.Text ?? string.Empty},
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Four Beam Flood Plot")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotFourBeamFlood" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty },
                    { "Type", type ?? string.Empty },
                    { "FieldName", comboFieldName?.SelectedItem?.ToString() ?? string.Empty},
                    { "xAxisMode", comboxAxisMode?.SelectedItem?.ToString() ?? string.Empty},
                    { "yAxisMode", comboyAxisMode?.SelectedItem?.ToString() ?? string.Empty},
                    { "Colormap", combocmap?.SelectedItem?.ToString() ?? string.Empty},
                    { "vmin", txtvmin?.Text ?? string.Empty},
                    { "vmax", txtvmax?.Text ?? string.Empty},
                    { "Title", txtTitle?.Text ?? string.Empty},
                    { "Mask", comboMask?.SelectedItem?.ToString() ?? string.Empty}
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Single Beam Flood Plot")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotSingleBeamFlood" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty },
                    { "Type", type ?? string.Empty},
                    { "BeamSelection", numericNBeams?.Value.ToString() ?? string.Empty},
                    { "UseMean", checkUseMean.Checked ? "Yes" : "No"},
                    { "FieldName", comboFieldName?.SelectedItem?.ToString() ?? string.Empty},
                    { "xAxisMode", comboxAxisMode?.SelectedItem?.ToString() ?? string.Empty},
                    { "yAxisMode", comboyAxisMode?.SelectedItem?.ToString() ?? string.Empty},
                    { "Colormap", combocmap?.SelectedItem?.ToString() ?? string.Empty},
                    { "vmin", txtvmin?.Text ?? string.Empty},
                    { "vmax", txtvmax?.Text ?? string.Empty},
                    { "Title", txtTitle?.Text ?? string.Empty},
                    { "Mask", comboMask?.SelectedItem?.ToString() ?? string.Empty}
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Velocity Flood Plot")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotVelocityFlood" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty },
                    { "Type", type ?? string.Empty },
                    { "FieldName", comboVelFieldName?.SelectedItem?.ToString() ?? string.Empty},
                    { "Coord", comboCoord?.SelectedItem?.ToString() ?? string.Empty},
                    { "xAxisMode", comboxAxisMode?.SelectedItem?.ToString() ?? string.Empty},
                    { "yAxisMode", comboyAxisMode?.SelectedItem?.ToString() ?? string.Empty},
                    { "Colormap", combocmap?.SelectedItem?.ToString() ?? string.Empty},
                    { "vmin", txtvmin?.Text ?? string.Empty},
                    { "vmax", txtvmax?.Text ?? string.Empty},
                    { "Title", txtTitle?.Text ?? string.Empty},
                    { "Mask", comboMask?.SelectedItem?.ToString() ?? string.Empty}
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Plot Transect Velocities")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotTransectVelocities" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty},
                    { "Type", type ?? string.Empty },
                    { "BinSelection", numericNBins?.Value.ToString() ?? string.Empty},
                    { "UseMean", checkUseMean.Checked ? "Yes" : "No"},
                    { "VectorScale", txtScale?.Text ?? string.Empty},
                    { "Colormap", combocmap?.SelectedItem?.ToString() ?? string.Empty},
                    { "vmin", txtvmin?.Text ?? string.Empty},
                    { "vmax", txtvmax?.Text ?? string.Empty},
                    { "Title", txtTitle?.Text ?? string.Empty},
                    { "LineWidth", txtLineWidth?.Text ?? string.Empty},
                    { "LineAlpha", txtLineAlpha?.Text ?? string.Empty},
                    { "HistBins", numericHistBins?.Value.ToString() ?? string.Empty}
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Beam Geometry Animation")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotBeamGeometryAnimation" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty },
                    { "Type", type ?? string.Empty },
                    { "AnimationOutputFile", txtAnimationOutputFile?.Text ?? string.Empty}
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Transect Animation")
            {
                inputs = new Dictionary<string, string>
                {
                    { "Task", "PlotTransectAnimation" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "InstrumentID", id ?? string.Empty },
                    { "Type", type ?? string.Empty },
                    { "Colormap", combocmap?.SelectedItem?.ToString() ?? string.Empty},
                    { "vmin", txtvmin?.Text ?? string.Empty},
                    { "vmax", txtvmax?.Text ?? string.Empty},
                    { "AnimationOutputFile", txtAnimationOutputFile?.Text ?? string.Empty}
                };
            }
            else
            {
                return;
            }

            string xmlInput = _Tools.GenerateInput(inputs);
            XmlDocument result = _Tools.CallPython(xmlInput);
            Dictionary<string, string> outputs = _Tools.ParseOutput(result);
            if (outputs.TryGetValue("Error", out string? value))
            {
                MessageBox.Show(value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void comboPlotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboPlotType.SelectedItem?.ToString() == "Platform Orientation")
            {
                PropPlatformOrientation();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Four Beam Flood Plot")
            {
                PropFourBeamFloodPlot();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Single Beam Flood Plot")
            {
                PropSingleBeamFloodPlot();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Velocity Flood Plot")
            {
                PropVelocityFloodPlot();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Plot Transect Velocities")
            {
                PlotTransectVelocities();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Beam Geometry Animation")
            {
                PlotBeamGeometryAnimation();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Transect Animation")
            {
                PlotTransectAnimation();
            }
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

        private void BtnAnimationOutputFile_Click(object? sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "gif files (*.fig)|*.gif",
                DefaultExt = "gif",
                Title = "Select Animation Output File"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtAnimationOutputFile.Text = saveFileDialog.FileName;
            }
        }
    }
}
