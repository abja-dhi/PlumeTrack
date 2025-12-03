using DHI.Generic.MikeZero;
using DHI.Generic.MikeZero.DFS;
using DHI.Generic.MikeZero.DFS.dfs123;
using DHI.Generic.MikeZero.DFS.dfsu;
using DHI.Generic.MikeZero.DFS.mesh;
using DHI.Math.Expression.Operations;
using Plume_Track;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Plume_Track
{
    public partial class ProjectPlot : Form
    {
        public XmlNodeList? hdModels;
        public XmlElement? hdModel;
        public XmlNodeList? mtModels;
        public XmlElement? mtModel;
        public XmlNodeList? adcps;
        public XmlElement? adcp;
        public _ClassConfigurationManager _project = new();

        private GroupBox grpRequired = _Utils.CreateGroupBox("grpRequired", "Required Inputs");
        private GroupBox grpMT = _Utils.CreateGroupBox("grpMT", "MT Inputs");
        private GroupBox grpHD = _Utils.CreateGroupBox("grpHD", "HD Inputs");
        private GroupBox grpADCP = _Utils.CreateGroupBox("grpADCP", "ADCP Inputs");
        private GroupBox grpLayout = _Utils.CreateGroupBox("grpLayout", "Layout");
        private GroupBox grpAnimation = _Utils.CreateGroupBox("grpAnimation", "Animation");

        private TableLayoutPanel tableRequired = _Utils.CreateTable("tableRequired", 3, [50F, 40F, 10F]);
        private TableLayoutPanel tableMT = _Utils.CreateTable("tableMT", 3, [50F, 40F, 10F]);
        private TableLayoutPanel tableHD = _Utils.CreateTable("tableHD", 3, [50F, 40F, 10F]);
        private TableLayoutPanel tableADCP = _Utils.CreateTable("tableADCP", 3, [50F, 40F, 10F]);
        private TableLayoutPanel tableLayout = _Utils.CreateTable("tableLayout", 3, [50F, 40F, 10F]);
        private TableLayoutPanel tableAnimation = _Utils.CreateTable("tableAnimation", 3, [50F, 40F, 10F]);

        // Required Inputs
        private Label lblHDModel = _Utils.CreateLabel("lblHDModel", "HD Model");
        private ComboBox comboHDModel = _Utils.CreateComboBox("comboHDModel", [], -1);
        private Label lblMTModel = _Utils.CreateLabel("lblMTModel", "MT Model");
        private ComboBox comboMTModel = _Utils.CreateComboBox("comboMTModel", [], -1);
        private Label lblADCP = _Utils.CreateLabel("lblADCP", "ADCP");
        private ComboBox comboADCP = _Utils.CreateComboBox("comboADCP", [], -1);
        // MT Inputs
        private Label lblSSCScale = _Utils.CreateLabel("lblSSCScale", "SSC Scale");
        private ComboBox comboSSCScale = _Utils.CreateComboBox("comboSSCScale", ["Normal", "Logarithmic"]);
        private Label lblSSCLevels = _Utils.CreateLabel("lblSSCLevels", "SSC Levels");
        private TextBox txtSSCLevels = _Utils.CreateTextBox("txtSSCLevels", "0.01,0.1,1.0,10.0");
        private Label lblSSCvmin = _Utils.CreateLabel("lblSSCvmin", "vmin");
        private TextBox txtSSCvmin = _Utils.CreateTextBox("txtSSCvmin", "");
        private Label lblSSCvmax = _Utils.CreateLabel("lblSSCvmax", "vmax");
        private TextBox txtSSCvmax = _Utils.CreateTextBox("txtSSCvmax", "");
        private Label lblSSCCmap = _Utils.CreateLabel("lblSSCCmap", "SSC Colormap");
        private ColormapComboBox comboSSCcmap = new(_Globals.CMapsPath, "combocmap")
        {
            SelectedColormapName = "turbo"
        };
        private Label lblSSCBottomThreshold = _Utils.CreateLabel("lblSSCBottomThreshold", "SSC Bottom Threshold");
        private TextBox txtSSCBottomThreshold = _Utils.CreateTextBox("txtSSCBottomThreshold", "");
        private Label lblSSCPixelSizeM = _Utils.CreateLabel("lblSSCPixelSizeM", "SSC Pixel Size (m)");
        private TextBox txtSSCPixelSizeM = _Utils.CreateTextBox("txtSSCPixelSizeM", "10");
        // HD Inputs
        private Label lblModelFieldPixelSizeM = _Utils.CreateLabel("lblModelFieldPixelSizeM", "Model Field Pixel Size (m)");
        private TextBox txtModelFieldPixelSizeM = _Utils.CreateTextBox("txtModelFieldPixelSizeM", "100");
        private Label lblModelFieldQuiverStrideN = _Utils.CreateLabel("lblModelFieldQuiverStrideN", "Model Field Quiver Stride N");
        private NumericUpDown numModelFieldQuiverStrideN = _Utils.CreateNumericUpDown("numModelFieldQuiverStrideN", 1, 20, 3);
        private Label lblModelQuiverMode = _Utils.CreateLabel("lblModelQuiverMode", "Model Quiver Mode");
        private ComboBox comboModelQuiverMode = _Utils.CreateComboBox("comboModelQuiverMode", ["Field", "Transect"]);
        private Label lblModelQuiverScale = _Utils.CreateLabel("lblModelQuiverScale", "Model Quiver Scale");
        private TextBox txtModelQuiverScale = _Utils.CreateTextBox("txtModelQuiverScale", "3");
        private Label lblModelQuiverWidth = _Utils.CreateLabel("lblModelQuiverWidth", "Model Quiver Width");
        private TextBox txtModelQuiverWidth = _Utils.CreateTextBox("txtModelQuiverWidth", "0.002");
        private Label lblModelQuiverHeadWidth = _Utils.CreateLabel("lblModelQuiverHeadWidth", "Model Quiver Head Width");
        private TextBox txtModelQuiverHeadWidth = _Utils.CreateTextBox("txtModelQuiverHeadWidth", "2");
        private Label lblModelQuiverHeadLength = _Utils.CreateLabel("lblModelQuiverHeadLength", "Model Quiver Head Length");
        private TextBox txtModelQuiverHeadLength = _Utils.CreateTextBox("txtModelQuiverHeadLength", "2.5");
        private Label lblModelQuiverColor = _Utils.CreateLabel("lblModelQuiverColor", "Model Quiver Color");
        private Panel pnlModelQuiverColor = _Utils.CreatePanel("pnlModelQuiverColor", Color.Blue);
        private Button btnModelQuiverColor = _Utils.CreateButton("btnModelQuiverColor", "...");
        private Label lblModelLevels = _Utils.CreateLabel("lblModelLevels", "Model Velocity Levels");
        private TextBox txtModelLevels = _Utils.CreateTextBox("txtModelLevels", "0.0,0.1,0.2,0.3,0.4,0.5");
        private Label lblModelvmin = _Utils.CreateLabel("lblModelvmin", "vmin");
        private TextBox txtModelvmin = _Utils.CreateTextBox("txtModelvmin", "");
        private Label lblModelvmax = _Utils.CreateLabel("lblModelvmax", "vmax");
        private TextBox txtModelvmax = _Utils.CreateTextBox("txtModelvmax", "");
        private Label lblModelCmap = _Utils.CreateLabel("lblModelCmap", "Model Colormap");
        private ColormapComboBox comboModelCmap = new(_Globals.CMapsPath, "combocmap")
        {
            SelectedColormapName = "turbo"
        };
        private Label lblModelCmapBottomThreshold = _Utils.CreateLabel("lblModelCmapBottomThreshold", "Model Colormap Bottom Threshold");
        private TextBox txtModelCmapBottomThreshold = _Utils.CreateTextBox("txtModelCmapBottomThreshold", "");
        // ADCP
        private Label lblADCPSeriesMode = _Utils.CreateLabel("lblADCPSeriesMode", "ADCP Series Mode");
        private ComboBox comboADCPSeriesMode = _Utils.CreateComboBox("comboADCPSeriesMode", ["Bin", "Depth"]);
        private Label lblADCPSeriesTarget = _Utils.CreateLabel("lblADCPSeriesTarget", "ADCP Series Target");
        private NumericUpDown numADCPSeriesTarget = _Utils.CreateNumericUpDown("numADCPSeriesTarget", 1, 100, 10);
        private TextBox txtADCPSeriesTarget = _Utils.CreateTextBox("txtADCPSeriesTarget", "10");
        private CheckBox checkADCPSeriesTarget = _Utils.CreateCheckBox("checkADCPSeriesTarget", "Mean", false);
        private Label lblADCPQuiverScale = _Utils.CreateLabel("lblADCPQuiverScale", "ADCP Quiver Scale");
        private TextBox txtADCPQuiverScale = _Utils.CreateTextBox("txtADCPQuiverScale", "3");
        private Label lblADCPQuiverWidth = _Utils.CreateLabel("lblADCPQuiverWidth", "ADCP Quiver Width");
        private TextBox txtADCPQuiverWidth = _Utils.CreateTextBox("txtADCPQuiverWidth", "0.002");
        private Label lblADCPQuiverHeadWidth = _Utils.CreateLabel("lblADCPQuiverHeadWidth", "ADCP Quiver Head Width");
        private TextBox txtADCPQuiverHeadWidth = _Utils.CreateTextBox("txtADCPQuiverHeadWidth", "2");
        private Label lblADCPQuiverHeadLength = _Utils.CreateLabel("lblADCPQuiverHeadLength", "ADCP Quiver Head Length");
        private TextBox txtADCPQuiverHeadLength = _Utils.CreateTextBox("txtADCPQuiverHeadLength", "2.5");
        private Label lblADCPQuiverColor = _Utils.CreateLabel("lblADCPQuiverColor", "ADCP Quiver Color");
        private Panel pnlADCPQuiverColor = _Utils.CreatePanel("pnlADCPQuiverColor", Color.Red);
        private Button btnADCPQuiverColor = _Utils.CreateButton("btnADCPQuiverColor", "...");
        private Label lblADCPTransectColor = _Utils.CreateLabel("lblADCPTransectColor", "ADCP Transect Color");
        private Panel pnlADCPTransectColor = _Utils.CreatePanel("pnlADCPTransectColor", Color.Green);
        private Button btnADCPTransectColor = _Utils.CreateButton("btnADCPTransectColor", "...");
        private Label lblADCPQuiverEveryN = _Utils.CreateLabel("lblADCPQuiverEveryN", "ADCP Quiver Every N");
        private NumericUpDown numADCPQuiverEveryN = _Utils.CreateNumericUpDown("numADCPQuiverEveryN", 1, 20, 1);
        private Label lblADCPTransectWidth = _Utils.CreateLabel("lblADCPTransectWidth", "ADCP Transect Line Width");
        private TextBox txtADCPTransectWidth = _Utils.CreateTextBox("txtADCPTransectWidth", "2");
        // Layout
        private Label lblLayoutCbarTickDecimals = _Utils.CreateLabel("lblLayoutCbarTickDecimals", "Color Bar Tick Decimals");
        private NumericUpDown numLayoutCbarTickDecimals = _Utils.CreateNumericUpDown("numLayoutCbarTickDecimals", 0, 5, 2);
        private Label lblLayoutAxisTickDecimals = _Utils.CreateLabel("lblLayoutAxisTickDecimals", "Axis Tick Decimals");
        private NumericUpDown numLayoutAxisTickDecimals = _Utils.CreateNumericUpDown("numLayoutAxisTickDecimals", 0, 5, 2);
        private Label lblLayoutPadM = _Utils.CreateLabel("lblLayoutPadM", "Padding (m)");
        private TextBox txtLayoutPadM = _Utils.CreateTextBox("txtLayoutPadM", "2000");
        private Label lblLayoutDistanceBinM = _Utils.CreateLabel("lblLayoutDistanceBinM", "Distance Bin (m)");
        private TextBox txtLayoutDistanceBinM = _Utils.CreateTextBox("txtLayoutDistanceBinM", "50");
        private Label lblLayoutBarWidthScale = _Utils.CreateLabel("lblLayoutBarWidthScale", "Bar Chart Width Scale");
        private TextBox txtLayoutBarWidthScale = _Utils.CreateTextBox("txtLayoutBarWidthScale", "0.15");
        // Animation
        private Label lblAnimationStartIndex = _Utils.CreateLabel("lblAnimationStartIndex", "First Timestep");
        private NumericUpDown numAnimationStartIndex = _Utils.CreateNumericUpDown("numAnimationStartIndex", 0, 10000, 0);
        private CheckBox checkAnimationUseStart = _Utils.CreateCheckBox("checkAnimationUseStart", "Use First Timestep", true);
        private Label lblAnimationEndIndex = _Utils.CreateLabel("lblAnimationEndIndex", "Last Timestep");
        private NumericUpDown numAnimationEndIndex = _Utils.CreateNumericUpDown("numAnimationEndIndex", 0, 10000, 10);
        private CheckBox checkAnimationUseEnd = _Utils.CreateCheckBox("checkAnimationUseEnd", "Use Last Timestep", true);
        private Label lblAnimationTimeStep = _Utils.CreateLabel("lblAnimationTimeStep", "Time Step");
        private NumericUpDown numAnimationTimeStep = _Utils.CreateNumericUpDown("numAnimationTimeStep", 1, 1000, 1);
        private Label lblAnimationInterval = _Utils.CreateLabel("lblAnimationInterval", "Interval (ms)");
        private NumericUpDown numAnimationInterval = _Utils.CreateNumericUpDown("numAnimationInterval", 100, 10000, 500);
        private Label lblAnimationBBox = _Utils.CreateLabel("lblAnimationBBox", "Bounding Box");
        private TextBox txtAnimationBBox = _Utils.CreateTextBox("txtAnimationBBox", "");
        private Button btnAnimationBBox = _Utils.CreateButton("btnAnimationBBox", "...");
        private Label lblAnimationOutputFile = _Utils.CreateLabel("lblAnimationOutputFile", "Output File");
        private TextBox txtAnimationOutputFile = _Utils.CreateTextBox("txtAnimationOutputFile", "");
        private Button btnAnimationOutputFile = _Utils.CreateButton("btnAnimationOutputFile", "...");

        private void PopulateComboBox(ComboBox comboBox, string itemName)
        {
            XmlNodeList? items = _ClassConfigurationManager.GetObjects(itemName);
            foreach (XmlNode node in items)
            {
                XmlElement element = (XmlElement)node;
                string name = element.GetAttribute("name");
                string id = element.GetAttribute("id");
                ComboBoxItem? item = new(name, id);
                comboBox.Items.Add(item);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void PropHDModelComparison()
        {

            tableProp.Controls.Clear();
            tableProp.RowStyles.Clear();
            PopulateComboBox(comboHDModel, "HDModel");
            PopulateComboBox(comboADCP, "VesselMountedADCP");

            IList<List<Control?>> controlsRequired = [
                [lblHDModel, comboHDModel],
                [lblADCP, comboADCP],
            ];
            _Utils.ConfigureTable(tableRequired, controlsRequired, [30F, 30F], false);
            grpRequired.Controls.Add(tableRequired);

            // ADCP Inputs
            IList<List<Control?>> controlsADCP = [
                [lblADCPSeriesMode, comboADCPSeriesMode],
                [lblADCPSeriesTarget, numADCPSeriesTarget, checkADCPSeriesTarget],
                [lblADCPQuiverScale, txtADCPQuiverScale],
                [lblADCPQuiverWidth, txtADCPQuiverWidth],
                [lblADCPQuiverHeadWidth, txtADCPQuiverHeadWidth],
                [lblADCPQuiverHeadLength, txtADCPQuiverHeadLength],
                [lblADCPQuiverColor, pnlADCPQuiverColor, btnADCPQuiverColor],
                [lblADCPTransectColor, pnlADCPTransectColor, btnADCPTransectColor],
            ];
            _Utils.ConfigureTable(tableADCP, controlsADCP, [30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F], false);
            tableADCP.Controls.Add(txtADCPSeriesTarget, 1, 1);
            grpADCP.Controls.Add(tableADCP);

            if (comboADCPSeriesMode.Items.Count > 0)
                comboADCPSeriesMode.SelectedIndex = 0;
            txtADCPSeriesTarget.Visible = false;
            checkADCPSeriesTarget.Checked = false;

            // HD Inputs
            IList<List<Control?>> controlsHD= [
                [lblModelFieldPixelSizeM, txtModelFieldPixelSizeM],
                [lblModelFieldQuiverStrideN, numModelFieldQuiverStrideN],
                [lblModelQuiverMode, comboModelQuiverMode],
                [lblModelQuiverScale, txtModelQuiverScale],
                [lblModelQuiverWidth, txtModelQuiverWidth],
                [lblModelQuiverHeadWidth, txtModelQuiverHeadWidth],
                [lblModelQuiverHeadLength, txtModelQuiverHeadLength],
                [lblModelQuiverColor, pnlModelQuiverColor, btnModelQuiverColor],
                [lblModelLevels, txtModelLevels],
                [lblModelvmin, txtModelvmin],
                [lblModelvmax, txtModelvmax],
                [lblModelCmap, comboModelCmap],
                [lblModelCmapBottomThreshold, txtModelCmapBottomThreshold],
            ];
            _Utils.ConfigureTable(tableHD, controlsHD, [30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F, 36F, 30F], false);
            grpHD.Controls.Add(tableHD);

            // Layout
            IList<List<Control?>> controlsLayout = [
                [lblLayoutCbarTickDecimals, numLayoutCbarTickDecimals],
                [lblLayoutAxisTickDecimals, numLayoutAxisTickDecimals],
                [lblLayoutPadM, txtLayoutPadM],
                [lblLayoutDistanceBinM, txtLayoutDistanceBinM],
                [lblLayoutBarWidthScale, txtLayoutBarWidthScale],
            ];
            _Utils.ConfigureTable(tableLayout, controlsLayout, [30F, 30F, 30F, 30F, 30F], false);
            grpLayout.Controls.Add(tableLayout);

            IList<List<Control?>> controls= [
                [grpRequired],
                [grpADCP],
                [grpHD],
                [grpLayout],
            ];
            _Utils.ConfigureTable(tableProp, controls, [tableRequired.RowCount * 30F, tableADCP.RowCount * 30F, tableHD.RowCount * 30F + 6F, tableLayout.RowCount * 30F], true);
            tableProp.Height = (int)(tableRequired.RowCount * 30F + tableADCP.RowCount * 30F + tableHD.RowCount * 30F + 6F + tableLayout.RowCount * 30F) + 40;
        }

        private void PropMTModelComparison()
        {
            tableProp.Controls.Clear();
            tableProp.RowStyles.Clear();
            PopulateComboBox(comboMTModel, "MTModel");
            PopulateComboBox(comboADCP, "VesselMountedADCP");
            
            // Required Inputs
            IList<List<Control?>> controlsRequired = [
                [lblMTModel, comboMTModel],
                [lblADCP, comboADCP],
            ];
            _Utils.ConfigureTable(tableRequired, controlsRequired, [30F, 30F], false);
            grpRequired.Controls.Add(tableRequired);

            // ADCP Inputs
            IList<List<Control?>> controlsADCP = [
                [lblADCPSeriesMode, comboADCPSeriesMode],
                [lblADCPSeriesTarget, numADCPSeriesTarget, checkADCPSeriesTarget],
                [lblADCPTransectColor, pnlADCPTransectColor, btnADCPTransectColor],
            ];
            _Utils.ConfigureTable(tableADCP, controlsADCP, [30F, 30F, 30F], false);
            tableADCP.Controls.Add(txtADCPSeriesTarget, 1, 1);
            grpADCP.Controls.Add(tableADCP);

            if (comboADCPSeriesMode.Items.Count > 0)
                comboADCPSeriesMode.SelectedIndex = 0;
            txtADCPSeriesTarget.Visible = false;
            checkADCPSeriesTarget.Checked = false;

            // MT Inputs
            IList<List<Control?>> controlsMT = [
                [lblSSCScale, comboSSCScale],
                [lblSSCLevels, txtSSCLevels],
                [lblSSCvmin, txtSSCvmin],
                [lblSSCvmax, txtSSCvmax],
                [lblSSCCmap, comboSSCcmap],
                [lblSSCBottomThreshold, txtSSCBottomThreshold],
                [lblSSCPixelSizeM, txtSSCPixelSizeM]
            ];
            _Utils.ConfigureTable(tableMT, controlsMT, [30F, 30F, 30F, 30F, 36F, 30F, 30F], false);
            grpMT.Controls.Add(tableMT);

            // Layout
            IList<List<Control?>> controlsLayout = [
                [lblLayoutCbarTickDecimals, numLayoutCbarTickDecimals],
                [lblLayoutAxisTickDecimals, numLayoutAxisTickDecimals],
                [lblLayoutPadM, txtLayoutPadM],
                [lblLayoutDistanceBinM, txtLayoutDistanceBinM],
                [lblLayoutBarWidthScale, txtLayoutBarWidthScale],
            ];
            _Utils.ConfigureTable(tableLayout, controlsLayout, [30F, 30F, 30F, 30F, 30F], false);
            grpLayout.Controls.Add(tableLayout);

            IList<List<Control?>> controls = [
                [grpRequired],
                [grpADCP],
                [grpMT],
                [grpLayout],
            ];
            _Utils.ConfigureTable(tableProp, controls, [tableRequired.RowCount * 30F, tableADCP.RowCount * 30F, tableMT.RowCount * 30F + 6F, tableLayout.RowCount * 30F], true);
            tableProp.Height = (int)(tableRequired.RowCount * 30F + tableADCP.RowCount * 30F + tableMT.RowCount * 30F + 6F + tableLayout.RowCount * 30F) + 40;
        }

        private void PropHDMTModelComparison()
        {
            tableProp.Controls.Clear();
            tableProp.RowStyles.Clear();
            PopulateComboBox(comboHDModel, "HDModel");
            PopulateComboBox(comboMTModel, "MTModel");
            PopulateComboBox(comboADCP, "VesselMountedADCP");

            // Required Inputs
            IList<List<Control?>> controlsRequired = [
                [lblHDModel, comboHDModel],
                [lblMTModel, comboMTModel],
                [lblADCP, comboADCP],
            ];
            _Utils.ConfigureTable(tableRequired, controlsRequired, [30F, 30F], false);
            grpRequired.Controls.Add(tableRequired);

            // ADCP Inputs
            IList<List<Control?>> controlsADCP = [
                [lblADCPSeriesMode, comboADCPSeriesMode],
                [lblADCPSeriesTarget, numADCPSeriesTarget, checkADCPSeriesTarget],
                [lblADCPQuiverScale, txtADCPQuiverScale],
                [lblADCPQuiverWidth, txtADCPQuiverWidth],
                [lblADCPQuiverHeadWidth, txtADCPQuiverHeadWidth],
                [lblADCPQuiverHeadLength, txtADCPQuiverHeadLength],
                [lblADCPQuiverEveryN, numADCPQuiverEveryN],
                [lblADCPTransectWidth, txtADCPTransectWidth]
            ];
            _Utils.ConfigureTable(tableADCP, controlsADCP, [30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F], false);
            tableADCP.Controls.Add(txtADCPSeriesTarget, 1, 1);
            grpADCP.Controls.Add(tableADCP);

            if (comboADCPSeriesMode.Items.Count > 0)
                comboADCPSeriesMode.SelectedIndex = 0;
            txtADCPSeriesTarget.Visible = false;
            checkADCPSeriesTarget.Checked = false;

            // HD Inputs
            IList<List<Control?>> controlsHD = [
                [lblModelFieldPixelSizeM, txtModelFieldPixelSizeM],
                [lblModelFieldQuiverStrideN, numModelFieldQuiverStrideN],
                [lblModelQuiverMode, comboModelQuiverMode],
                [lblModelQuiverScale, txtModelQuiverScale],
                [lblModelQuiverWidth, txtModelQuiverWidth],
                [lblModelQuiverHeadWidth, txtModelQuiverHeadWidth],
                [lblModelQuiverHeadLength, txtModelQuiverHeadLength],
                [lblModelQuiverColor, pnlModelQuiverColor, btnModelQuiverColor],
            ];
            _Utils.ConfigureTable(tableHD, controlsHD, [30F, 30F, 30F, 30F, 30F, 30F, 30F, 30F], false);
            grpHD.Controls.Add(tableHD);

            // MT Inputs
            IList<List<Control?>> controlsMT = [
                [lblSSCScale, comboSSCScale],
                [lblSSCLevels, txtSSCLevels],
                [lblSSCvmin, txtSSCvmin],
                [lblSSCvmax, txtSSCvmax],
                [lblSSCCmap, comboSSCcmap],
                [lblSSCBottomThreshold, txtSSCBottomThreshold],
                [lblSSCPixelSizeM, txtSSCPixelSizeM]
            ];
            _Utils.ConfigureTable(tableMT, controlsMT, [30F, 30F, 30F, 30F, 36F, 30F, 30F], false);
            grpMT.Controls.Add(tableMT);

            // Layout
            IList<List<Control?>> controlsLayout = [
                [lblLayoutCbarTickDecimals, numLayoutCbarTickDecimals],
                [lblLayoutAxisTickDecimals, numLayoutAxisTickDecimals],
                [lblLayoutPadM, txtLayoutPadM],
            ];
            _Utils.ConfigureTable(tableLayout, controlsLayout, [30F, 30F, 30F], false);
            grpLayout.Controls.Add(tableLayout);

            IList<List<Control?>> controls = [
                [grpRequired],
                [grpADCP],
                [grpMT],
                [grpLayout],
            ];
            _Utils.ConfigureTable(tableProp, controls, [tableRequired.RowCount * 30F, tableADCP.RowCount * 30F, tableHD.RowCount * 30F, tableMT.RowCount * 30F + 6F, tableLayout.RowCount * 30F], true);
            tableProp.Height = (int)(tableRequired.RowCount * 30F + tableADCP.RowCount * 30F + tableHD.RowCount * 30F + tableMT.RowCount * 30F + 6F + tableLayout.RowCount * 30F) + 40;
        }

        private void PropHDMTModelAnimation()
        {
            tableProp.Controls.Clear();
            tableProp.RowStyles.Clear();
            // Required Inputs
            XmlNodeList? hdModels = _ClassConfigurationManager.GetObjects("HDModel");
            foreach (XmlNode node in hdModels)
            {
                XmlElement element = (XmlElement)node;
                string name = element.GetAttribute("name");
                string id = element.GetAttribute("id");
                ComboBoxItem? item = new ComboBoxItem(name, id);
                comboHDModel.Items.Add(item);
            }
            if (comboHDModel.Items.Count > 0)
                comboHDModel.SelectedIndex = 0;
            XmlNodeList? mtModels = _ClassConfigurationManager.GetObjects("MTModel");
            foreach (XmlNode node in mtModels)
            {
                XmlElement element = (XmlElement)node;
                string name = element.GetAttribute("name");
                string id = element.GetAttribute("id");
                ComboBoxItem? item = new ComboBoxItem(name, id);
                comboMTModel.Items.Add(item);
            }
            if (comboMTModel.Items.Count > 0)
                comboMTModel.SelectedIndex = 0;
            tableRequired.Controls.Clear();
            tableRequired.RowStyles.Clear();
            tableRequired.RowCount = 3;
            tableRequired.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableRequired.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableRequired.Controls.Add(lblHDModel, 0, 0);
            tableRequired.Controls.Add(comboHDModel, 1, 0);
            tableRequired.Controls.Add(lblMTModel, 0, 1);
            tableRequired.Controls.Add(comboMTModel, 1, 1);
            grpRequired.Controls.Add(tableRequired);

            // HD Inputs
            IList<List<Control?>> controlsHD = [
                [lblModelFieldPixelSizeM, txtModelFieldPixelSizeM],
                [lblModelFieldQuiverStrideN, numModelFieldQuiverStrideN],
                [lblModelQuiverScale, txtModelQuiverScale],
                [lblModelQuiverWidth, txtModelQuiverWidth],
                [lblModelQuiverHeadWidth, txtModelQuiverHeadWidth],
                [lblModelQuiverHeadLength, txtModelQuiverHeadLength],
                [lblModelQuiverColor, pnlModelQuiverColor, btnModelQuiverColor],
            ];
            _Utils.ConfigureTable(tableHD, controlsHD, [30F, 30F, 30F, 30F, 30F, 30F, 30F], false);
            grpHD.Controls.Add(tableHD);

            // MT Inputs
            IList<List<Control?>> controlsMT = [
                [lblSSCScale, comboSSCScale],
                [lblSSCLevels, txtSSCLevels],
                [lblSSCvmin, txtSSCvmin],
                [lblSSCvmax, txtSSCvmax],
                [lblSSCCmap, comboSSCcmap],
                [lblSSCBottomThreshold, txtSSCBottomThreshold],
                [lblSSCPixelSizeM, txtSSCPixelSizeM]
            ];
            _Utils.ConfigureTable(tableMT, controlsMT, [30F, 30F, 30F, 30F, 36F, 30F, 30F], false);
            grpMT.Controls.Add(tableMT);

            // Layout
            IList<List<Control?>> controlsLayout = [
                [lblLayoutCbarTickDecimals, numLayoutCbarTickDecimals],
                [lblLayoutAxisTickDecimals, numLayoutAxisTickDecimals],
            ];
            _Utils.ConfigureTable(tableLayout, controlsLayout, [30F, 30F], false);
            grpLayout.Controls.Add(tableLayout);

            // Animation
            IList<List<Control?>> controlsAnimation = [
                [lblAnimationStartIndex, numAnimationStartIndex, checkAnimationUseStart],
                [lblAnimationEndIndex, numAnimationEndIndex, checkAnimationUseEnd],
                [lblAnimationTimeStep, numAnimationTimeStep],
                [lblAnimationInterval, numAnimationInterval],
                [lblAnimationBBox, txtAnimationBBox, btnAnimationBBox],
                [lblAnimationOutputFile, txtAnimationOutputFile, btnAnimationOutputFile]
            ];
            _Utils.ConfigureTable(tableAnimation, controlsAnimation, [30F, 30F, 30F, 30F, 30F, 30F], false);
            grpAnimation.Controls.Add(tableAnimation);

            // Final Layout
            IList<List<Control?>> controls = [
                [grpRequired],
                [grpHD],
                [grpMT],
                [grpLayout],
                [grpAnimation]
            ];
            _Utils.ConfigureTable(tableProp, controls, [tableRequired.RowCount * 30F, tableHD.RowCount * 30F, tableMT.RowCount * 30F + 6F, tableLayout.RowCount * 30F, tableAnimation.RowCount * 30F,], true);
            tableProp.Height = (int)(tableRequired.RowCount * 30F + tableHD.RowCount * 30F + tableMT.RowCount * 30F + 6F + tableLayout.RowCount * 30F + tableAnimation.RowCount * 30F) + 40;
        }

        private void InitializeWidgets()
        {
            comboHDModel.DisplayMember = "name";
            comboHDModel.ValueMember = "id";
            comboMTModel.DisplayMember = "name";
            comboMTModel.ValueMember = "id";
            comboADCP.DisplayMember = "name";
            comboADCP.ValueMember = "id";
            if (comboPlotType.Items.Count > 0)
                comboPlotType.SelectedIndex = 0;
            checkADCPSeriesTarget.CheckedChanged += checkADCPSeriesTarget_CheckedChanged;
            if (comboSSCScale.Items.Count > 0)
                comboSSCScale.SelectedIndex = 1;
            // Set comboSSCcmap to "turbo" if available
            for (int i = 0; i < comboSSCcmap.Items.Count; i++)
            {
                if (comboSSCcmap.Items[i] is ColormapItem item && item.Name.Equals("turbo", StringComparison.OrdinalIgnoreCase))
                {
                    comboSSCcmap.SelectedIndex = i;
                    break;
                }
            }
            // Set comboModelCmap to "turbo" if available
            for (int i = 0; i < comboModelCmap.Items.Count; i++)
            {
                if (comboModelCmap.Items[i] is ColormapItem item && item.Name.Equals("turbo", StringComparison.OrdinalIgnoreCase))
                {
                    comboModelCmap.SelectedIndex = i;
                    break;
                }
            }
            if (comboModelQuiverMode.Items.Count > 0)
                comboModelQuiverMode.SelectedIndex = 0;
            btnModelQuiverColor.Click += btnChangeColor_Click;
            btnADCPQuiverColor.Click += btnChangeColor_Click;
            btnADCPTransectColor.Click += btnChangeColor_Click;
            checkAnimationUseStart.CheckedChanged += checkAnimationUseStart_CheckedChanged;
            checkAnimationUseEnd.CheckedChanged += checkAnimationUseEnd_CheckedChanged;
            btnAnimationBBox.Click += btnAnimatiobBBox_Click;
            btnAnimationOutputFile.Click += btnAnimationOutputFile_Click;
            numAnimationStartIndex.Enabled = false;
            numAnimationEndIndex.Enabled = false;
        }

        public ProjectPlot()
        {
            InitializeComponent();
            InitializeWidgets();
        }

        private bool ValidInputs(string type)
        {
            if (type == "HD")
            {

                return true;
            }
            else if (type == "MT")
            {
                return true;
            }
            else if (type == "HDMT")
            {
                return true;
            }
            else if (type == "Animation")
            {
                return true;
            }
            return true;
        }

        private void btnPlot_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> inputs = null;
            if (comboPlotType.SelectedItem.ToString() == "HD Comparison")
            {
                if (ValidInputs(type: "HD"))
                {
                    var selectedHDModel = comboHDModel.SelectedItem as ComboBoxItem;
                    var selectedADCP = comboADCP.SelectedItem as ComboBoxItem;
                    string adcpSeriesTarget;
                    if (comboADCPSeriesMode.SelectedItem.ToString().Equals("Bin", StringComparison.OrdinalIgnoreCase))
                        adcpSeriesTarget = numADCPSeriesTarget.Value.ToString();
                    else
                        adcpSeriesTarget = txtADCPSeriesTarget.Text;
                    inputs = new Dictionary<string, string>
                    {
                        { "Task", "HDComparison" },
                        { "Project", _Globals.Config.OuterXml.ToString() },
                        { "ModelID", selectedHDModel.ID},
                        { "ADCPID", selectedADCP.ID},
                        { "ADCPSeriesMode", comboADCPSeriesMode.SelectedItem.ToString() },
                        { "ADCPSeriesTarget", adcpSeriesTarget },
                        { "UseMean", checkADCPSeriesTarget.Checked ? "Yes":"No" },
                        { "ADCPTransectColor", ColorTranslator.ToHtml(pnlADCPTransectColor.BackColor) },
                        { "ADCPQuiverScale", txtADCPQuiverScale.Text },
                        { "ADCPQuiverWidth", txtADCPQuiverWidth.Text },
                        { "ADCPQuiverHeadWidth", txtADCPQuiverHeadWidth.Text },
                        { "ADCPQuiverHeadLength", txtADCPQuiverHeadLength.Text },
                        { "ADCPQuiverColor", ColorTranslator.ToHtml(pnlADCPQuiverColor.BackColor) },
                        { "ModelFieldPixelSizeM", txtModelFieldPixelSizeM.Text },
                        { "ModelFieldQuiverStrideN", numModelFieldQuiverStrideN.Value.ToString() },
                        { "ModelQuiverScale", txtModelQuiverScale.Text },
                        { "ModelQuiverWidth", txtModelQuiverWidth.Text },
                        { "ModelQuiverHeadWidth", txtModelQuiverHeadWidth.Text },
                        { "ModelQuiverHeadLength", txtModelQuiverHeadLength.Text },
                        { "ModelQuiverColor", ColorTranslator.ToHtml(pnlModelQuiverColor.BackColor) },
                        { "ModelQuiverMode", comboModelQuiverMode.SelectedItem.ToString() },
                        { "ModelLevels", txtModelLevels.Text },
                        { "Modelvmin", txtModelvmin.Text },
                        { "Modelvmax", txtModelvmax.Text },
                        { "ModelCmapName", comboModelCmap.SelectedItem.ToString() },
                        { "ModelBottomThreshold", txtModelCmapBottomThreshold.Text },
                        { "LayoutCbarTickDecimals", numLayoutCbarTickDecimals.Value.ToString() },
                        { "LayoutAxisTickDecimals", numLayoutAxisTickDecimals.Value.ToString() },
                        { "LayoutPadM", txtLayoutPadM.Text },
                        { "LayoutDistanceBinM", txtLayoutDistanceBinM.Text },
                        { "LayoutBarWidthScale", txtLayoutBarWidthScale.Text }
                    };
                }
                else
                {
                    MessageBox.Show("Worng Input! Please check the inputs and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (comboPlotType.SelectedItem.ToString() == "MT Comparison")
            {
                if (ValidInputs(type: "MT"))
                {
                    var selectedMTModel = comboMTModel.SelectedItem as ComboBoxItem;
                    var selectedADCP = comboADCP.SelectedItem as ComboBoxItem;
                    string adcpSeriesTarget;
                    if (comboADCPSeriesMode.SelectedItem.ToString().Equals("Bin", StringComparison.OrdinalIgnoreCase))
                        adcpSeriesTarget = numADCPSeriesTarget.Value.ToString();
                    else
                        adcpSeriesTarget = txtADCPSeriesTarget.Text;
                    inputs = new Dictionary<string, string>
                    {
                        { "Task", "MTComparison" },
                        { "Project", _Globals.Config.OuterXml.ToString() },
                        { "ModelID", selectedMTModel.ID },
                        { "ADCPID", selectedADCP.ID },
                        { "ADCPSeriesMode", comboADCPSeriesMode.SelectedItem.ToString() },
                        { "ADCPSeriesTarget", adcpSeriesTarget },
                        { "UseMean", checkADCPSeriesTarget.Checked ? "Yes":"No" },
                        { "ADCPTransectColor", ColorTranslator.ToHtml(pnlADCPTransectColor.BackColor) },
                        { "SSCScale", comboSSCScale.SelectedItem.ToString() },
                        { "SSCLevels", txtSSCLevels.Text },
                        { "SSCvmin", txtSSCvmin.Text },
                        { "SSCvmax", txtSSCvmax.Text },
                        { "SSCCmapName", comboSSCcmap.SelectedItem.ToString() },
                        { "SSCBottomThreshold", txtSSCBottomThreshold.Text },
                        { "SSCPixelSizeM", txtSSCPixelSizeM.Text },
                        { "LayoutCbarTickDecimals", numLayoutCbarTickDecimals.Value.ToString() },
                        { "LayoutAxisTickDecimals", numLayoutAxisTickDecimals.Value.ToString() },
                        { "LayoutPadM", txtLayoutPadM.Text },
                        { "LayoutDistanceBinM", txtLayoutDistanceBinM.Text },
                        { "LayoutBarWidthScale", txtLayoutBarWidthScale.Text }
                    };
                }
                else
                {
                    MessageBox.Show("Worng Input! Please check the inputs and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else if (comboPlotType.SelectedItem.ToString() == "MT and HD Comparison")
            {
                if (ValidInputs(type: "HDMT"))
                {
                    var selectedMTModel = comboMTModel.SelectedItem as ComboBoxItem;
                    var selectedHDModel = comboHDModel.SelectedItem as ComboBoxItem;
                    var selectedADCP = comboADCP.SelectedItem as ComboBoxItem;
                    string adcpSeriesTarget;
                    if (comboADCPSeriesMode.SelectedItem.ToString().Equals("Bin", StringComparison.OrdinalIgnoreCase))
                        adcpSeriesTarget = numADCPSeriesTarget.Value.ToString();
                    else
                        adcpSeriesTarget = txtADCPSeriesTarget.Text;
                    inputs = new Dictionary<string, string>
                    {
                        { "Task", "HDMTComparison" },
                        { "Project", _Globals.Config.OuterXml.ToString() },
                        { "MTModelID", selectedMTModel.ID },
                        { "HDModelID", selectedHDModel.ID },
                        { "ADCPID", selectedADCP.ID },
                        { "ADCPTransectLineWidth", txtADCPTransectWidth.Text },
                        { "ADCPSeriesMode", comboADCPSeriesMode.SelectedItem.ToString() },
                        { "ADCPSeriesTarget", adcpSeriesTarget },
                        { "UseMean", checkADCPSeriesTarget.Checked ? "Yes":"No" },
                        { "ADCPQuiverEveryN", numADCPQuiverEveryN.Value.ToString() },
                        { "ADCPQuiverWidth", txtADCPQuiverWidth.Text },
                        { "ADCPQuiverHeadWidth", txtADCPQuiverHeadWidth.Text },
                        { "ADCPQuiverHeadLength", txtADCPQuiverHeadLength.Text },
                        { "ADCPQuiverScale", txtADCPQuiverScale.Text },
                        { "SSCScale", comboSSCScale.SelectedItem.ToString() },
                        { "SSCLevels", txtSSCLevels.Text },
                        { "SSCvmin", txtSSCvmin.Text },
                        { "SSCvmax", txtSSCvmax.Text },
                        { "SSCCmapName", comboSSCcmap.SelectedItem.ToString() },
                        { "SSCBottomThreshold", txtSSCBottomThreshold.Text },
                        { "SSCPixelSizeM", txtSSCPixelSizeM.Text },
                        { "ModelFieldPixelSizeM", txtModelFieldPixelSizeM.Text },
                        { "ModelFieldQuiverStrideN", numModelFieldQuiverStrideN.Value.ToString() },
                        { "ModelQuiverScale", txtModelQuiverScale.Text },
                        { "ModelQuiverWidth", txtModelQuiverWidth.Text },
                        { "ModelQuiverHeadWidth", txtModelQuiverHeadWidth.Text },
                        { "ModelQuiverHeadLength", txtModelQuiverHeadLength.Text },
                        { "ModelQuiverColor", ColorTranslator.ToHtml(pnlModelQuiverColor.BackColor) },
                        { "ModelQuiverMode", comboModelQuiverMode.SelectedItem.ToString() },
                        { "LayoutCbarTickDecimals", numLayoutCbarTickDecimals.Value.ToString() },
                        { "LayoutAxisTickDecimals", numLayoutAxisTickDecimals.Value.ToString() },
                        { "LayoutPadM", txtLayoutPadM.Text },
                    };
                }
                else
                {
                    MessageBox.Show("Worng Input! Please check the inputs and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (comboPlotType.SelectedItem.ToString() == "MT and HD Comparison Animation")
            {
                if (ValidInputs(type: "Animation"))
                {
                    var selectedMTModel = comboMTModel.SelectedItem as ComboBoxItem;
                    var selectedHDModel = comboHDModel.SelectedItem as ComboBoxItem;
                    string layoutBBox = txtAnimationBBox.Text;
                    if (!File.Exists(layoutBBox))
                    {
                        MessageBox.Show("Bounding Box Shapefile does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    inputs = new Dictionary<string, string>
                    {
                        { "Task", "HDMTAnimation" },
                        { "Project", _Globals.Config.OuterXml.ToString() },
                        { "MTModelID", selectedMTModel.ID },
                        { "HDModelID", selectedHDModel.ID },
                        { "SSCScale", comboSSCScale.SelectedItem.ToString() },
                        { "SSCLevels", txtSSCLevels.Text },
                        { "SSCvmin", txtSSCvmin.Text },
                        { "SSCvmax", txtSSCvmax.Text },
                        { "SSCCmapName", comboSSCcmap.SelectedItem.ToString() },
                        { "SSCBottomThreshold", txtSSCBottomThreshold.Text },
                        { "SSCPixelSizeM", txtSSCPixelSizeM.Text },
                        { "ModelFieldPixelSizeM", txtModelFieldPixelSizeM.Text },
                        { "ModelFieldQuiverStrideN", numModelFieldQuiverStrideN.Value.ToString() },
                        { "ModelQuiverScale", txtModelQuiverScale.Text },
                        { "ModelQuiverWidth", txtModelQuiverWidth.Text },
                        { "ModelQuiverHeadWidth", txtModelQuiverHeadWidth.Text },
                        { "ModelQuiverHeadLength", txtModelQuiverHeadLength.Text },
                        { "ModelQuiverColor", ColorTranslator.ToHtml(pnlModelQuiverColor.BackColor) },
                        { "AnimationStartIndex", checkAnimationUseStart.Checked ? "Start" : numAnimationStartIndex.Value.ToString() },
                        { "AnimationEndIndex", checkAnimationUseEnd.Checked ? "End" : numAnimationEndIndex.Value.ToString() },
                        { "AnimationTimeStep", numAnimationTimeStep.Value.ToString() },
                        { "AnimationInterval", numAnimationInterval.Value.ToString() },
                        { "AnimationOutputFile", txtAnimationOutputFile.Text },
                        { "LayoutCbarTickDecimals", numLayoutCbarTickDecimals.Value.ToString() },
                        { "LayoutAxisTickDecimals", numLayoutAxisTickDecimals.Value.ToString() },
                        { "LayoutBBox", layoutBBox}

                    };
                }

                else
                {
                    return;
                }
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
            comboADCP.Items.Clear();
            comboHDModel.Items.Clear();
            comboMTModel.Items.Clear();
            if (comboPlotType.SelectedItem.ToString() == "HD Comparison")
            {
                PropHDModelComparison();
            }
            else if (comboPlotType.SelectedItem.ToString() == "MT Comparison")
            {
                PropMTModelComparison();
            }
            else if (comboPlotType.SelectedItem.ToString() == "MT and HD Comparison")
            {
                PropHDMTModelComparison();
            }
            else if (comboPlotType.SelectedItem.ToString() == "MT and HD Comparison Animation")
            {
                PropHDMTModelAnimation();
            }
        }

        private void checkADCPSeriesTarget_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkADCPSeriesTarget.Checked)
            {
                numADCPSeriesTarget.Enabled = false;
                txtADCPSeriesTarget.Enabled = false;
            }
            else
            {
                numADCPSeriesTarget.Enabled = true;
                txtADCPSeriesTarget.Enabled = true;
            }
        }

        private void checkAnimationUseStart_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkAnimationUseStart.Checked)
            {
                numAnimationStartIndex.Enabled = false;
            }
            else
            {
                numAnimationStartIndex.Enabled = true;
            }
        }

        private void checkAnimationUseEnd_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkAnimationUseEnd.Checked)
            {
                numAnimationEndIndex.Enabled = false;
            }
            else
            {
                numAnimationEndIndex.Enabled = true;
            }
        }

        private void btnAnimationBBox_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Animation Bounding Box Shapefile";
                ofd.Filter = "Polygon Shapefile (*.shp)|*.shp";
                ofd.DefaultExt = "shp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtAnimationBBox.Text = ofd.FileName;
                }
            }
        }

        private void btnAnimationOutputFile_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Select Animation Output File";
                sfd.Filter = "MP4 files (*.mp4)|*.mp4|gif files (*.gif)|*.gif";
                sfd.DefaultExt = "gif";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    txtAnimationOutputFile.Text = sfd.FileName;
                }
            }
        }

        private void btnAnimatiobBBox_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Animation Bounding Box Shapefile";
                ofd.Filter = "Polygon Shapefile (*.shp)|*.shp";
                ofd.DefaultExt = "shp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtAnimationBBox.Text = ofd.FileName;
                }
            }
        }

        private void btnChangeColor_Click(object? sender, EventArgs e)
        {
            Button btn = sender as Button;
            Panel pnl = null;
            if (btn.Name == "btnModelQuiverColor")
                pnl = pnlModelQuiverColor;
            else if (btn.Name == "btnADCPQuiverColor")
                pnl = pnlADCPQuiverColor;
            else if (btn.Name == "btnADCPTransectColor")
                pnl = pnlADCPTransectColor;
            if (pnl == null)
                return;
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = pnl.BackColor;
                cd.FullOpen = true;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    pnl.BackColor = cd.Color;
                }
            }
        }
    }
}
