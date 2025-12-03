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
    public partial class SSCModelPlot : Form
    {
        public XmlElement? sscmodel;
        public XmlElement? survey;
        public string? id;
        public string? type;
        public _ClassConfigurationManager _project = new();

        private readonly Label lblTitle = _Utils.CreateLabel("lblTitle", "Title");
        private readonly TextBox txtTitle = _Utils.CreateTextBox("txtTitle");
        private readonly Label lblBeamSelection = _Utils.CreateLabel("lblBeamSelection", "Beam Selection");
        private readonly NumericUpDown numericNBeams = _Utils.CreateNumericUpDown("numericNBeams", 1, 4, 1, false);
        private readonly CheckBox checkUseMean = _Utils.CreateCheckBox("checkUseMean", "Use Mean", true);
        private readonly Label lblFieldName = _Utils.CreateLabel("lblFieldName", "Field Name");
        private readonly ComboBox comboFieldName = _Utils.CreateComboBox("comboFieldName", ["Echo Intensity", "Correlation Magnitude", "Percent Good", "Absolute Backscatter", "Alpha s", "Alpha w", "Signal to Noise Ratio", "SSC"]);
        private readonly Label lblyAxisMode = _Utils.CreateLabel("lblyAxisMode", "y Axis Mode");
        private readonly ComboBox comboyAxisMode = _Utils.CreateComboBox("comboyAxisMode", ["Bin", "Depth"]);
        private readonly Label lblcmap = _Utils.CreateLabel("lblcmap", "Colormap");
        private readonly ColormapComboBox combocmap = new(_Globals.CMapsPath, "combocmap")
        {
            SelectedColormapName = "turbo"
        };
        private readonly Label lblvmin = _Utils.CreateLabel("lblvmin", "Colomrap Minimum");
        private readonly TextBox txtvmin = _Utils.CreateTextBox("txtvmin");
        private readonly Label lblvmax = _Utils.CreateLabel("lblvmax", "Colormap Maximum");
        private readonly TextBox txtvmax = _Utils.CreateTextBox("txtvmax");
        private readonly Label lblMask = _Utils.CreateLabel("lblMask", "Apply Masking?");
        private readonly ComboBox comboMask = _Utils.CreateComboBox("comboMask", ["Yes", "No"]);

        private void InitializeWidgets()
        {
            checkUseMean.CheckedChanged += CheckUseMean_CheckedChanged;
        }

        private void PropRegressionPlot()
        {
            IList<List<Control?>> controls = [[lblTitle, txtTitle]];
            _Utils.ConfigureTable(tableProp, controls, [30F], true);
            txtTitle.TabIndex = 0;
        }

        private void PropTransectPlot()
        {
            IList<List<Control?>> controls = [
                [lblBeamSelection, numericNBeams, checkUseMean],
                [lblFieldName, comboFieldName],
                [lblyAxisMode, comboyAxisMode],
                [lblcmap, combocmap],
                [lblvmin, txtvmin],
                [lblvmax, txtvmax],
                [lblTitle, txtTitle],
                [lblMask, comboMask]
            ];
            _Utils.ConfigureTable(tableProp, controls, [30F, 30F, 30F, 36F, 30F, 30F, 30F, 30F], true);
            numericNBeams.TabIndex = 0;
            checkUseMean.TabIndex = 1;
            comboFieldName.TabIndex = 2;
            comboyAxisMode.TabIndex = 3;
            combocmap.TabIndex = 4;
            txtvmin.TabIndex = 5;
            txtvmax.TabIndex = 6;
            txtTitle.TabIndex = 7;
            comboMask.TabIndex = 8;
        }

        public SSCModelPlot(string? _id, string? _type)
        {
            InitializeComponent();
            InitializeWidgets();
            comboPlotType.SelectedIndex = 0;
            id = _id;
            type = _type;
            sscmodel = _ClassConfigurationManager.GetObject(type: type, id: id);
            comboPlotType.Items.Clear();
            if (type == "NTU2SSC")
            {
                comboPlotType.Items.Add("Regression Plot");
                comboPlotType.SelectedIndex = 0;
            }
            else
            {
                comboPlotType.Items.Add("Regression Plot");
                comboPlotType.Items.Add("Transect Plot");
                comboPlotType.SelectedIndex = 0;
            }
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> inputs;
            
            if (comboPlotType.SelectedItem?.ToString() == "Regression Plot")
            {
                string task;
                if (type == "BKS2SSC")
                    task = "PlotBKS2SSCRegression";
                else if (type == "NTU2SSC")
                    task = "PlotNTU2SSCRegression";
                else
                    task = "PlotBKS2NTURegression";
                inputs = new Dictionary<string, string>
                {
                    { "Task", task },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "SSCModelID", id },
                    { "Title", txtTitle.Text },
                };
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Transect Plot")
            {
                string task;
                if (type == "BKS2SSC")
                    task = "PlotBKS2SSCTransect";
                else
                    task = "PlotBKS2NTUTransect";
                inputs = new Dictionary<string, string>
                {
                    { "Task", task },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                    { "SSCModelID", id },
                    { "BeamSelection", numericNBeams.Value.ToString()},
                    { "UseMean", checkUseMean.Checked ? "Yes" : "No"},
                    { "FieldName", comboFieldName.SelectedItem.ToString()},
                    { "yAxisMode", comboyAxisMode.SelectedItem.ToString()},
                    { "Colormap", combocmap.SelectedItem.ToString()},
                    { "vmin", txtvmin.Text},
                    { "vmax", txtvmax.Text},
                    { "Title", txtTitle.Text},
                    { "Mask", comboMask.SelectedItem.ToString()}
                };
            }
            else
            {
                return;
            }

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
            if (comboPlotType.SelectedItem?.ToString() == "Regression Plot")
            {
                PropRegressionPlot();
            }
            else if (comboPlotType.SelectedItem?.ToString() == "Transect Plot")
            {
                PropTransectPlot();
            }
        }

        private void CheckUseMean_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkUseMean.Checked)
            {
                numericNBeams.Enabled = false;
            }
            else
            {
                numericNBeams.Enabled = true;
            }
        }
    }
}
