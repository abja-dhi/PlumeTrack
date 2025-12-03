using DHI.Mike1D.ResultDataAccess;
using DHI.Mike1D.ResultDataAccess.Epanet;
using Microsoft.VisualBasic;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using Python.Runtime;
using System.Security.Principal;
using System.Xml;
using System.Xml.Linq;


namespace Plume_Track
{
    public partial class __PlumeTrack : Form
    {
        public bool isSaved; // Track if project has been saved
        public _ClassConfigurationManager _project = new();

        private static void AddChildNodes(XmlNode xmlNode, TreeNode treeNode)
        {
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element || child.Name == "Settings" || child.Name == "MapSettings")
                    continue; // Skip non-element nodes and the Settings node

                XmlAttribute? typeAttr = child.Attributes?["type"];
                XmlAttribute? nameAttr = child.Attributes?["name"];
                if (typeAttr != null && nameAttr != null)
                {
                    string name = nameAttr.Value ?? child.Name;
                    TreeNode childNode = new(name)
                    {
                        Tag = child
                    };
                    treeNode.Nodes.Add(childNode);
                    AddChildNodes(child, childNode); // Recursively add child nodes
                }
            }
        }

        private void FillTree()
        {
            treeProject.Nodes.Clear();
            XmlNode? root = _Globals.Config.DocumentElement;
            if (root != null)
            {
                string name = _ClassConfigurationManager.GetSetting(settingName: "Name");
                TreeNode rootNode = new(name)
                {
                    Tag = root
                };
                treeProject.Nodes.Add(rootNode);
                AddChildNodes(root, rootNode); // Recursively add child nodes
                treeProject.ExpandAll(); // Expand all nodes for better visibility
            }
            else
            {
                MessageBox.Show("Error: Project configuration is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeProject()
        {
            _project.InitializeProject();
            isSaved = true;
            FillTree(); // Populate the tree view with project structure
        }

        public __PlumeTrack(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
            {
                string projectFilePath = args[0];
                if (File.Exists(projectFilePath) && Path.GetExtension(projectFilePath).Equals(".mtproj", StringComparison.OrdinalIgnoreCase))
                {
                    _project.LoadConfig(projectFilePath);
                    isSaved = true; // Mark the project as saved after loading
                    FillTree(); // Populate the tree view with the loaded project structure
                }
                else
                {
                    MessageBox.Show("The specified project file does not exist or is not a valid .mtproj file. A new project will be created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    InitializeProject();
                }
            }
            else
                InitializeProject();

            this.KeyPreview = true; // Enable form to capture key events
            this.KeyDown += __PlumeTrack_KeyDown; // Attach key down event handler

            this.Load += async (s, e) =>
            {
                try
                {
                    bool isElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent())
                        .IsInRole(WindowsBuiltInRole.Administrator);
                    string webView2UserData = Path.Combine(
                        _Globals.dataPath, "PlumeTrack", isElevated ? "WebView2UserData_elevated" : "WebView2UserData");

                    Directory.CreateDirectory(webView2UserData);

                    // ---- Create environment & initialize WebView2 ----
                    var env = await CoreWebView2Environment.CreateAsync(
                        browserExecutableFolder: null,   // use Evergreen runtime on machine
                        userDataFolder: webView2UserData,
                        options: null);

                    await webView.EnsureCoreWebView2Async(env);

                    // ---- Navigate to your local HTML ----
                    // Build an absolute file path and convert to a file:// URI
                    string htmlPath = Path.Combine(_Globals.dataPath, "PlumeTrack", "load_data_message.html");
                    htmlPath = Path.GetFullPath(htmlPath);

                    if (!File.Exists(htmlPath))
                    {
                        // Optional: show a friendly message if the file is missing
                        MessageBox.Show($"Missing file:\n{htmlPath}", "Startup error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                    }

                    var fileUri = new Uri(htmlPath); // converts C:\...\file.html -> file:///C:/.../file.html
                                                     // Either:
                    webView.Source = fileUri;
                    // Or:
                    // webView.CoreWebView2.Navigate(fileUri.AbsoluteUri);
                }
                catch (UnauthorizedAccessException ex)
                {
                    // Most common cause: user-data folder not writable or reused across elevation levels
                    MessageBox.Show(
                        "WebView2 could not access its user data folder.\n\n" +
                        "Try these steps:\n" +
                        " • Run without Administrator if you previously ran as admin\n" +
                        " • Delete %APPDATA%\\PlumeTrack\\WebView2UserData* and retry\n" +
                        " • Ensure antivirus or policies are not locking the folder\n\n" +
                        ex.Message,
                        "Access denied (WebView2)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Startup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            };
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (!_Globals.isSaved || !isSaved) // If project has changed
            {
                // Show a message box to ask the user if they want to save changes
                DialogResult result = MessageBox.Show(
                "Do you want to save the current project before creating a new project?",
                "Unsaved Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveProject(); // Save the project
                    if (status == 1)
                    {
                        InitializeProject(); // Initialize a new project if save was successful
                        _Globals.isSaved = false;
                    }
                    else
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // User chose to cancel, do not create a new project
                }
                else if (result == DialogResult.No)
                {
                    InitializeProject(); // Initialize a new project without saving
                    _Globals.isSaved = false;
                }
            }
            else
            {
                InitializeProject(); // Initialize a new project if no unsaved changes
                _Globals.isSaved = false;
            }

        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (!_Globals.isSaved || !isSaved) // If project has changed
            {
                // Show a message box to ask the user if they want to save changes
                DialogResult result = MessageBox.Show(
                text: "Do you want to save the current project before loading a new one?",
                caption: "Unsaved changes",
                buttons: MessageBoxButtons.YesNoCancel,
                icon: MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveProject(); // Save the current project
                    if (status == 0)
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // User chose to cancel, do not load a new project
                }
            }
            using OpenFileDialog ofd = new()
            {
                Filter = "MT Project Files (*.mtproj)|*.mtproj",
                Title = "Load Project",
                InitialDirectory = _ClassConfigurationManager.GetSetting(settingName: "Directory")
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                _project.LoadConfig(filePath);
                isSaved = true; // Mark the project as saved after loading
                FillTree(); // Repopulate the tree view with the loaded project structure
                _Globals.isSaved = true;
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveProject();
            FillTree(); // Refresh the tree view after saving
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            _Globals.isSaved = false; // Force Save As dialog
            SaveProject();
            FillTree(); // Refresh the tree view after saving
        }

        private void menuProperties_Click(object sender, EventArgs e)
        {
            PropertiesPage propertiesPage = new();
            propertiesPage.ShowDialog();
        }

        private void menuMapOptions_Click(object sender, EventArgs e)
        {
            MapOptions mapOptions = new();
            mapOptions.ShowDialog();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            //if (!_Globals.isSaved || !isSaved)
            //{
            //    DialogResult result = MessageBox.Show(
            //    text: "Do you want to save the current project before exiting?",
            //    caption: "Unsaved changes",
            //    buttons: MessageBoxButtons.YesNoCancel,
            //    icon: MessageBoxIcon.Warning);
            //    if (result == DialogResult.Yes)
            //    {
            //        int status = SaveProject(); // Save the current project
            //        if (status == 1)
            //        {
            //            Application.Exit();
            //            return;
            //        }
            //        else
            //            return;
            //    }
            //    else if (result == DialogResult.Cancel)
            //    {
            //        return; // User chose to cancel, do not exit
            //    }
            //    else if (result == DialogResult.No)
            //    {
            //        Application.Exit(); // Exit without saving
            //    }
            //}
            Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_Globals.isSaved || !isSaved)
            {
                DialogResult result = MessageBox.Show(
                text: "Do you want to save the current project before exiting?",
                caption: "Unsaved changes",
                buttons: MessageBoxButtons.YesNoCancel,
                icon: MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveProject(); // Save the current project
                    if (status == 0)
                    {
                        e.Cancel = true; // Cancel closing if save failed
                        return;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // User chose to cancel, do not close the form
                    return;
                }
            }
            try
            {
                PythonEngine.Shutdown(); // Shutdown Python engine
            }
            catch { }

            //Application.Exit(); // Exit the application
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            FillTree(); // Refresh the tree view when the form is activated
            mapView_CheckedChanged(sender, e); // Refresh the map view
        }

        private void menuAddSurvey_Click(object sender, EventArgs e)
        {
            Survey addSurvey = new(null);
            addSurvey.ShowDialog();
            FillTree(); // Refresh the tree view after adding a survey
        }

        private void menuAddHDModel_Click(object sender, EventArgs e)
        {
            ModelHD addHDModel = new(null);
            addHDModel.ShowDialog();
            FillTree(); // Refresh the tree view after adding a model
        }

        private void menuAddMTModel_Click(object sender, EventArgs e)
        {
            ModelMT addMTModel = new(null);
            addMTModel.ShowDialog();
            FillTree(); // Refresh the tree view after adding a model
        }

        private void menuAddSSCModel_Click(object sender, EventArgs e)
        {
            SSCModel addSSCModel = new(null);
            addSSCModel.ShowDialog();
            FillTree(); // Refresh the tree view after adding a model
        }

        private void menuAboutUs_Click(object sender, EventArgs e)
        {
            _AboutUs aboutUs = new();
            aboutUs.ShowDialog();
        }

        private void treeProject_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeProject.SelectedNode = e.Node;
                // Check type of associated XML node
                if (e.Node.Tag is XmlNode xmlNode)
                {
                    string? type = xmlNode.Attributes?["type"]?.Value ?? string.Empty;
                    itemDelete.Visible = type != "Project"; // Disable delete for the root project node

                    if (type == "NTU2SSC" || type == "BKS2SSC" || type == "BKS2NTU")
                    {
                        XmlNode? modeNode = xmlNode.SelectSingleNode("Mode");
                        string mode = modeNode?.InnerText ?? string.Empty;

                        itemPlot.Visible = mode != "Manual";

                    }
                }
                cmenuNode.Show(treeProject, e.Location);
            }
        }

        private void treeProject_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode? selectedNode = treeProject.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is XmlNode xmlNode)
            {
                string type = xmlNode.Attributes?["type"]?.Value ?? string.Empty;

                switch (type)
                {
                    case "Project":
                        PropertiesPage propertiesPage = new();
                        propertiesPage.ShowDialog();
                        break;
                    case "Survey":
                        Survey editSurvey = new(xmlNode);
                        editSurvey.ShowDialog();
                        break;
                    case "HDModel":
                        ModelHD editHDModel = new(xmlNode);
                        editHDModel.ShowDialog();
                        break;
                    case "MTDModel":
                        ModelMT editMTModel = new(xmlNode);
                        editMTModel.ShowDialog();
                        break;
                    case "VesselMountedADCP":
                        if (xmlNode.ParentNode is not XmlElement survey)
                        {
                            MessageBox.Show("Error: Unable to find parent survey for the selected Vessel Mounted ADCP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _SurveyManager surveyManager = new()
                        {
                            Name = survey.GetAttribute("name"),
                            survey = survey
                        };
                        VesselMountedADCP editVesselMountedADCP = new(surveyManager, xmlNode);
                        editVesselMountedADCP.ShowDialog();
                        break;
                    case "WaterSample":
                        if (xmlNode.ParentNode is not XmlElement survey2)
                        {
                            MessageBox.Show("Error: Unable to find parent survey for the selected Water Sample.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _SurveyManager surveyManager2 = new()
                        {
                            Name = survey2.GetAttribute("name"),
                            survey = survey2
                        };

                        WaterSample editWaterSample = new(surveyManager2, xmlNode);
                        editWaterSample.ShowDialog();
                        break;
                    case "OBSVerticalProfile":
                        if (xmlNode.ParentNode is not XmlElement survey3)
                        {
                            MessageBox.Show("Error: Unable to find parent survey for the selected OBS Vertical Profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _SurveyManager surveyManager3 = new()
                        {
                            Name = survey3.GetAttribute("name"),
                            survey = survey3
                        };
                        OBSVerticalProfile editOBSVerticalProfile = new(surveyManager3, xmlNode);
                        editOBSVerticalProfile.ShowDialog();
                        break;
                    case "NTU2SSC":
                    case "BKS2SSC":
                    case "BKS2NTU":
                        SSCModel editSSCModel = new(xmlNode);
                        editSSCModel.ShowDialog();
                        break;
                }
            }
            FillTree(); // Refresh the tree view after double-clicking an item
        }

        private void itemOpen_Click(object sender, EventArgs e)
        {
            TreeNode? selectedNode = treeProject.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is XmlNode xmlNode)
            {
                string type = xmlNode.Attributes?["type"]?.Value ?? string.Empty;

                switch (type)
                {
                    case "Project":
                        PropertiesPage propertiesPage = new();
                        propertiesPage.ShowDialog();
                        break;
                    case "Survey":
                        Survey editSurvey = new(xmlNode);
                        editSurvey.ShowDialog();
                        break;
                    case "HDModel":
                        ModelHD editHDModel = new(xmlNode);
                        editHDModel.ShowDialog();
                        break;
                    case "MTDModel":
                        ModelMT editMTModel = new(xmlNode);
                        editMTModel.ShowDialog();
                        break;
                    case "VesselMountedADCP":
                        if (xmlNode.ParentNode is not XmlElement survey)
                        {
                            MessageBox.Show("Error: Unable to find parent survey for the selected Vessel Mounted ADCP.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _SurveyManager surveyManager = new()
                        {
                            Name = survey.GetAttribute("name"),
                            survey = survey
                        };
                        VesselMountedADCP editVesselMountedADCP = new(surveyManager, xmlNode);
                        editVesselMountedADCP.ShowDialog();
                        break;
                    case "WaterSample":
                        if (xmlNode.ParentNode is not XmlElement survey2)
                        {
                            MessageBox.Show("Error: Unable to find parent survey for the selected Water Sample.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _SurveyManager surveyManager2 = new()
                        {
                            Name = survey2.GetAttribute("name"),
                            survey = survey2
                        };

                        WaterSample editWaterSample = new(surveyManager2, xmlNode);
                        editWaterSample.ShowDialog();
                        break;
                    case "OBSVerticalProfile":
                        if (xmlNode.ParentNode is not XmlElement survey3)
                        {
                            MessageBox.Show("Error: Unable to find parent survey for the selected OBS Vertical Profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        _SurveyManager surveyManager3 = new()
                        {
                            Name = survey3.GetAttribute("name"),
                            survey = survey3
                        };
                        OBSVerticalProfile editOBSVerticalProfile = new(surveyManager3, xmlNode);
                        editOBSVerticalProfile.ShowDialog();
                        break;
                    case "NTU2SSC":
                    case "BKS2SSC":
                    case "BKS2NTU":
                        SSCModel editSSCModel = new(xmlNode);
                        editSSCModel.ShowDialog();
                        break;
                }
            }
            FillTree(); // Refresh the tree view after opening an item
        }

        private void itemPlot_Click(object sender, EventArgs e)
        {
            TreeNode? selectedNode = treeProject.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is XmlNode xmlNode)
            {
                string type = xmlNode.Attributes?["type"]?.Value ?? string.Empty;
                string name = xmlNode.Attributes?["name"]?.Value ?? xmlNode.Name;
                string id = xmlNode.Attributes?["id"]?.Value ?? string.Empty;


                switch (type)
                {
                    case "Project":
                        ProjectPlot projectPlot = new();
                        projectPlot.Show();
                        break;
                    case "Survey":
                        MessageBox.Show($"Plotting survey: {name}", "Open Survey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Implement survey opening logic here
                        break;
                    case "HDModel":
                        ModelHDPlot modelHDPlot = new(id);
                        modelHDPlot.Show();
                        break;
                    case "MTModel":
                        ModelMTPlot modelMTPlot = new(id);
                        modelMTPlot.Show();
                        break;
                    case "VesselMountedADCP":
                        VesselMountedADCPPlot vesselMountedADCPPlot = new(id);
                        vesselMountedADCPPlot.Show();
                        break;
                    case "WaterSample":
                        MessageBox.Show($"Plotting water sample: {name}", "Open Water Sample", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Implement water sample opening logic here
                        break;
                    case "OBSVerticalProfile":
                        MessageBox.Show($"Plotting OBS vertical profile: {name}", "Open OBS Vertical Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Implement OBS vertical profile opening logic here
                        break;
                    case "NTU2SSC":
                        SSCModelPlot sscModelPlot = new(id, "NTU2SSC");
                        sscModelPlot.Show();
                        break;
                    case "BKS2SSC":
                        SSCModelPlot bks2sscModelPlot = new(id, "BKS2SSC");
                        bks2sscModelPlot.Show();
                        break;
                    case "BKS2NTU":
                        SSCModelPlot sSCModelPlot = new(id, "BKS2NTU");
                        sSCModelPlot.Show();
                        break;
                }
            }
        }

        private void itemDelete_Click(object sender, EventArgs e)
        {
            TreeNode? selectedNode = treeProject.SelectedNode;
            if (selectedNode != null && selectedNode.Tag is XmlNode xmlNode)
            {
                string type = xmlNode.Attributes?["type"]?.Value ?? string.Empty;
                string name = xmlNode.Attributes?["name"]?.Value ?? xmlNode.Name;
                string id = xmlNode.Attributes?["id"]?.Value ?? string.Empty;

                switch (type)
                {
                    case "Survey":
                        DialogResult resultSurvey = MessageBox.Show($"Are you sure you want to delete the survey: {name}?", "Delete Survey", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultSurvey == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "Survey", id: id);
                        }
                        break;
                    case "Model":
                        DialogResult resultMode = MessageBox.Show($"Are you sure you want to delete the model: {name}?", "Delete Model", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultMode == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "Model", id: id);
                        }
                        break;
                    case "VesselMountedADCP":
                        DialogResult resultVesselMountedADCP = MessageBox.Show($"Are you sure you want to delete the vessel-mounted ADCP: {name}?", "Delete Vessel Mounted ADCP", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultVesselMountedADCP == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "VesselMountedADCP", id: id);
                        }
                        break;
                    case "WaterSample":
                        DialogResult resultWaterSample = MessageBox.Show($"Are you sure you want to delete the water sample: {name}?", "Delete Water Sample", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultWaterSample == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "WaterSample", id: id);
                        }
                        break;
                    case "OBSVerticalProfile":
                        DialogResult resultOBSVerticalProfile = MessageBox.Show($"Are you sure you want to delete the OBS vertical profile: {name}?", "Delete OBS Vertical Profile", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultOBSVerticalProfile == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "OBSVerticalProfile", id: id);
                        }
                        break;
                    case "NTU2SSC":
                        DialogResult resultNTU2SSCModel = MessageBox.Show($"Are you sure you want to delete the NTU to SSC model: {name}?", "Delete NTU to SSC Model", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultNTU2SSCModel == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "NTU2SSC", id: id);
                        }
                        break;
                    case "BKS2SSC":
                        DialogResult resultBKS2SSCModel = MessageBox.Show($"Are you sure you want to delete the Absolute Backscatter to SSC model: {name}?", "Delete Absolute Backscatter to SSC Model", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultBKS2SSCModel == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "BKS2SSC", id: id);
                        }
                        break;
                    case "BKS2NTU":
                        DialogResult resultBKS2NTUModel = MessageBox.Show($"Are you sure you want to delete the Absolute Backscatter to NTU model: {name}?", "Delete Absolute Backscatter to NTU Model", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (resultBKS2NTUModel == DialogResult.Yes)
                        {
                            _ClassConfigurationManager.DeleteNode(type: "BKS2NTU", id: id);
                        }
                        break;
                }
            }
            isSaved = false; // Mark the project as unsaved after deletion
            FillTree();
        }

        private int SaveProject()
        {
            if (!_Globals.isSaved)
            {
                string directory = _ClassConfigurationManager.GetSetting(settingName: "Directory");
                if (directory == Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                    directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                SaveFileDialog sfd = new()
                {
                    Filter = "MT Project Files (*.mtproj)|*.mtproj",
                    Title = "Save Project",
                    InitialDirectory = directory,
                    FileName = _ClassConfigurationManager.GetSetting(settingName: "Name") + ".mtproj",
                    OverwritePrompt = true,
                };
                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sfd.FileName);
                    string? selectedDirectory = Path.GetDirectoryName(sfd.FileName);
                    _ClassConfigurationManager.SetSetting(settingName: "Name", value: fileNameWithoutExtension);
                    if (selectedDirectory != null)
                        _ClassConfigurationManager.SetSetting(settingName: "Directory", value: selectedDirectory);
                    else
                        _ClassConfigurationManager.SetSetting(settingName: "Directory", value: Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    _project.SaveConfig(); // Save the project configuration with the new name and directory
                    _Globals.isSaved = true;
                    return 1;
                }
                else if (result == DialogResult.Cancel)
                {
                    return 0; // Indicate cancellation
                }

            }
            else
            {
                _project.SaveConfig(); // Save the project configuration
                isSaved = true;
                return 1; // Indicate success    
            }
            return 1;
        }

        private void __PlumeTrack_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S) // Ctrl + S
            {
                e.SuppressKeyPress = true; // Prevent default behavior
                if (sender != null)
                    menuSave_Click(sender, e); // Save the project
            }
            else if (e.Control && e.KeyCode == Keys.N) // Ctrl + N
            {
                e.SuppressKeyPress = true; // Prevent default behavior
                if (sender != null)
                    menuNew_Click(sender, e); // Create a new project
            }
            else if (e.Control && e.KeyCode == Keys.O) // Ctrl + O
            {
                e.SuppressKeyPress = true; // Prevent default behavior
                if (sender != null)
                    menuOpen_Click(sender, e); // Open an existing project
            }
        }

        private void mapView_CheckedChanged(object sender, EventArgs e)
        {
            string name = _ClassConfigurationManager.GetSetting("Name");
            string mapViewer2DPath = Path.Combine(_Globals.dataPath, "PlumeTrack", $"MapViewer2D_{name}.html");
            string mapViewer3DPath = Path.Combine(_Globals.dataPath, "PlumeTrack", $"MapViewer3D_{name}.html");
            if (map2D.Checked)
            {
                if (File.Exists(mapViewer2DPath))
                {
                    webView.Source = new Uri(mapViewer2DPath);
                    return;
                }
                Dictionary<string, string> inputs2D = new()
                {
                    { "Task", "MapViewer2D" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                };
                string xmlInput2D = _Tools.GenerateInput(inputs2D);
                XmlDocument result2D = _Tools.CallPython(xmlInput2D);
                Dictionary<string, string> outputs2D = _Tools.ParseOutput(result2D);
                if (outputs2D.TryGetValue("Error", out string? value))
                {
                    MessageBox.Show(value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                webView.Source = new Uri(outputs2D["Result"]);
            }
            else if (map3D.Checked)
            {
                if (File.Exists(mapViewer3DPath))
                {
                    webView.Source = new Uri(mapViewer3DPath);
                    return;
                }
                Dictionary<string, string> inputs3D = new()
                {
                    { "Task", "MapViewer3D" },
                    { "Project", _Globals.Config.OuterXml.ToString() },
                };
                string xmlInput3D = _Tools.GenerateInput(inputs3D);
                XmlDocument result3D = _Tools.CallPython(xmlInput3D);
                Dictionary<string, string> outputs3D = _Tools.ParseOutput(result3D);
                if (outputs3D.TryGetValue("Error", out string? value))
                {
                    MessageBox.Show(value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                webView.Source = new Uri(outputs3D["Result"]);
            }
        }

        private static string GetTypeAttr(TreeNode n) =>
            (n?.Tag as XmlElement)?.GetAttribute("type") ?? string.Empty;

        private static string GetIDAttr(TreeNode n) =>
            (n?.Tag as XmlElement)?.GetAttribute("id") ?? string.Empty;

        private static bool IsSource(TreeNode n)
        {
            return GetTypeAttr(n) switch
            {
                "BKS2SSC" or "BKS2NTU" or "NTU2SSC" => true,
                _ => false,
            };
        }

        private static bool CanDrop(TreeNode source, TreeNode target)
        {
            var s = GetTypeAttr(source);
            var t = GetTypeAttr(target);

            // NTU2SSC only OBSVerticalProfile
            if (s == "NTU2SSC") return t == "OBSVerticalProfile";

            // BKS2SSC or BKS2NTU only VesselMountedADCP
            if (s == "BKS2SSC" || s == "BKS2NTU") return t == "VesselMountedADCP";

            return false;
        }

        private static XmlElement? GetOrCreateChild(XmlElement parent)
        {
            string type = parent.GetAttribute("type");
            if (type == "VesselMountedADCP")
            {
                XmlNode? pd0 = parent.SelectSingleNode("Pd0");
                if (pd0 == null)
                {
                    MessageBox.Show("Error: 'Pd0' node is missing in the Vessel Mounted ADCP model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                XmlNode? sscModelNode = pd0.SelectSingleNode("SSCModelID");
                if (sscModelNode == null)
                {
                    XmlElement newSscModelNode = _Globals.Config.CreateElement("SSCModelID");
                    pd0.AppendChild(newSscModelNode);
                    return newSscModelNode;
                }
                return sscModelNode as XmlElement;
            }
            else if (type == "OBSVerticalProfile")
            {
                XmlNode? fileInfo = parent.SelectSingleNode("FileInfo");
                if (fileInfo == null)
                {
                    MessageBox.Show("Error: 'FileInfo' node is missing in the OBS Vertical Profile model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                XmlNode? sscModelNode = fileInfo.SelectSingleNode("SSCModelID");
                if (sscModelNode == null)
                {
                    XmlElement newSscModelNode = _Globals.Config.CreateElement("SSCModelID");
                    fileInfo.AppendChild(newSscModelNode);
                    return newSscModelNode;
                }
                return sscModelNode as XmlElement;
            }
            else
                return null;
        }


        private void treeProject_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node && IsSource(node))
                DoDragDrop(e.Item, DragDropEffects.Link);
        }

        private void treeProject_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            e.Effect = e.Data.GetDataPresent(typeof(TreeNode)) ? DragDropEffects.Link : DragDropEffects.None;
        }

        private void treeProject_DragOver(object sender, DragEventArgs e)
        {
            var tv = (TreeView)sender;
            var pt = tv.PointToClient(new Point(e.X, e.Y));
            var target = tv.GetNodeAt(pt);
            if (e.Data == null) return;

            if (e.Data.GetData(typeof(TreeNode)) is TreeNode source && target != null && IsSource(source) && CanDrop(source, target))
                e.Effect = DragDropEffects.Link;
            else
                e.Effect = DragDropEffects.None;

            tv.SelectedNode = target; // optional visual cue
        }

        private void treeProject_DragDrop(object sender, DragEventArgs e)
        {
            var tv = (TreeView)sender;
            var pt = tv.PointToClient(new Point(e.X, e.Y));
            var targetNode = tv.GetNodeAt(pt);
            if (e.Data == null) return;
            if (e.Data.GetData(typeof(TreeNode)) is not TreeNode sourceNode || targetNode == null) return;
            if (!IsSource(sourceNode) || !CanDrop(sourceNode, targetNode)) return;

            if (targetNode.Tag is not XmlElement targetXml) return;

            // ensure SSCModelID exists on target and update its InnerText
            var sscNode = GetOrCreateChild(targetXml);
            if (sscNode == null) return;
            sscNode.InnerText = GetIDAttr(sourceNode);

            tv.SelectedNode = targetNode;
            tv.Focus();
            isSaved = false; // mark project as unsaved

        }
    }
}
