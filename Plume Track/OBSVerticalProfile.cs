using Plume_Track;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar;

namespace Plume_Track
{

    public partial class OBSVerticalProfile : Form
    {

        public _SurveyManager? surveyManager;
        public XmlDocument? project;
        public int id;
        XmlElement? obsElement;
        public bool isSaved;
        public int mode; // 0 = new, 1 = edit
        int _headerLine;
        string? _delimiter;
        public _ClassConfigurationManager _project = new();

        private void InitializeOBS()
        {
            if (surveyManager == null || surveyManager.survey == null)
            {
                MessageBox.Show("Survey manager is not initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            txtName.Text = "New OBS Vertical Profile";
            txtFilePath.Text = string.Empty;
            checkMaskingDateTime.Checked = false;
            checkMaskingDepth.Checked = false;
            checkMaskingNTU.Checked = false;
            txtMaskingStartDateTime.Text = string.Empty;
            txtMaskingEndDateTime.Text = string.Empty;
            txtMaskingMinDepth.Text = string.Empty;
            txtMaskingMaxDepth.Text = string.Empty;
            txtMaskingMinNTU.Text = string.Empty;
            txtMaskingMaxNTU.Text = string.Empty;
            comboDate.Items.Clear();
            comboTime.Items.Clear();
            comboDepth.Items.Clear();
            comboNTU.Items.Clear();
            comboSSCModel.Items.Clear();
            comboDate.Text = string.Empty;
            comboTime.Text = string.Empty;
            comboDepth.Text = string.Empty;
            comboNTU.Text = string.Empty;
            comboSSCModel.Text = string.Empty;
            lblSSCModel.Enabled = false;
            comboSSCModel.Enabled = false;
            tableColumnInfo.Enabled = false;
            tableMasking.Enabled = false;
            // Create the instrument element
            isSaved = true;
        }

        private void PopulateOBS()
        {
            if (obsElement == null)
            {
                MessageBox.Show("OBS Vertical Profile element is not initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            txtName.Text = obsElement.GetAttribute("name") ?? string.Empty;
            XmlNode? fileInfoNode = obsElement.SelectSingleNode("FileInfo");
            txtFilePath.Text = fileInfoNode?.SelectSingleNode("Path")?.InnerText ?? string.Empty;
            XmlNode? columns = fileInfoNode?.SelectSingleNode("Columns");
            if (columns == null)
            {
                MessageBox.Show("No column information found in the OBS Vertical Profile configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            foreach (XmlNode columnNode in columns.ChildNodes)
            {
                if (columnNode.NodeType == XmlNodeType.Element)
                {
                    string columnName = columnNode.InnerText.Trim();
                    comboDate.Items.Add(columnName);
                    comboTime.Items.Add(columnName);
                    comboDepth.Items.Add(columnName);
                    comboNTU.Items.Add(columnName);
                }
            }
            XmlNode? headerNode = fileInfoNode?.SelectSingleNode("Header");
            _headerLine = int.TryParse(headerNode?.InnerText, out int header) ? header : 0;
            XmlNode? sepNode = fileInfoNode?.SelectSingleNode("Sep");
            _delimiter = sepNode?.InnerText ?? ",";
            static void SelectComboItem(ComboBox combo, string value)
            {
                int index = combo.Items.IndexOf(value.Trim());
                if (index >= 0)
                    combo.SelectedIndex = index;
            }
            SelectComboItem(comboDate, fileInfoNode?.SelectSingleNode("DateColumn")?.InnerText ?? "");
            SelectComboItem(comboTime, fileInfoNode?.SelectSingleNode("TimeColumn")?.InnerText ?? "");
            SelectComboItem(comboDepth, fileInfoNode?.SelectSingleNode("DepthColumn")?.InnerText ?? "");
            SelectComboItem(comboNTU, fileInfoNode?.SelectSingleNode("NTUColumn")?.InnerText ?? "");
            XmlNodeList sscModels = _ClassConfigurationManager.GetObjects("NTU2SSC");
            if (sscModels.Count > 0)
            {
                comboSSCModel.Items.Clear();
                foreach (XmlNode modelNode in sscModels)
                {
                    if (modelNode is not XmlElement model)
                        continue;
                    comboSSCModel.Items.Add(new ComboBoxItem(text: model.GetAttribute("name"), id: model.GetAttribute("id")));
                }
                comboSSCModel.DisplayMember = "Text";
                string sscModelId = fileInfoNode?.SelectSingleNode("SSCModelID")?.InnerText ?? string.Empty;
                if (!string.IsNullOrEmpty(sscModelId))
                {
                    for (int i = 0; i < comboSSCModel.Items.Count; i++)
                    {
                        if (comboSSCModel.Items[i] is ComboBoxItem item && item.ID == sscModelId)
                        {
                            comboSSCModel.SelectedIndex = i;
                            break;
                        }
                    }
                }
                comboSSCModel.Enabled = true; // Enable if models are available
                lblSSCModel.Enabled = true; // Enable label if models are available
            }
            else
            {
                comboSSCModel.Items.Clear();
                comboSSCModel.Enabled = false; // Disable if no models are available
                lblSSCModel.Enabled = false; // Disable label if no models are available
            }

            XmlNode? maskingNode = obsElement.SelectSingleNode("Masking");
            if (maskingNode == null)
            {
                checkMaskingDateTime.Checked = false;
                txtMaskingStartDateTime.Text = string.Empty;
                txtMaskingEndDateTime.Text = string.Empty;
                checkMaskingDepth.Checked = false;
                txtMaskingMinDepth.Text = string.Empty;
                txtMaskingMaxDepth.Text = string.Empty;
                checkMaskingNTU.Checked = false;
                txtMaskingMinNTU.Text = string.Empty;
                txtMaskingMaxNTU.Text = string.Empty;
            }
            else
            {
                XmlNode? maskingDateTimeNode = maskingNode.SelectSingleNode("MaskDateTime");
                if (maskingDateTimeNode == null)
                {
                    checkMaskingDateTime.Checked = false;
                    txtMaskingStartDateTime.Text = string.Empty;
                    txtMaskingEndDateTime.Text = string.Empty;
                }
                else
                {
                    checkMaskingDateTime.Checked = (maskingDateTimeNode as XmlElement)?.GetAttribute("Enabled") == "true";
                    txtMaskingStartDateTime.Text = maskingDateTimeNode.SelectSingleNode("Start")?.InnerText ?? string.Empty;
                    txtMaskingEndDateTime.Text = maskingDateTimeNode.SelectSingleNode("End")?.InnerText ?? string.Empty;
                }
                XmlNode? maskingDepthNode = maskingNode.SelectSingleNode("MaskDepth");
                if (maskingDepthNode == null)
                {
                    checkMaskingDepth.Checked = false;
                    txtMaskingMinDepth.Text = string.Empty;
                    txtMaskingMaxDepth.Text = string.Empty;
                }
                else
                {
                    checkMaskingDepth.Checked = (maskingDepthNode as XmlElement)?.GetAttribute("Enabled") == "true";
                    txtMaskingMinDepth.Text = maskingDepthNode.SelectSingleNode("Min")?.InnerText ?? string.Empty;
                    txtMaskingMaxDepth.Text = maskingDepthNode.SelectSingleNode("Max")?.InnerText ?? string.Empty;
                }
                XmlNode? maskingNTUNode = maskingNode.SelectSingleNode("MaskNTU");
                if (maskingNTUNode == null)
                {
                    checkMaskingNTU.Checked = false;
                    txtMaskingMinNTU.Text = string.Empty;
                    txtMaskingMaxNTU.Text = string.Empty;
                }
                else
                {
                    checkMaskingNTU.Checked = (maskingNTUNode as XmlElement)?.GetAttribute("Enabled") == "true";
                    txtMaskingMinNTU.Text = maskingNTUNode.SelectSingleNode("Min")?.InnerText ?? string.Empty;
                    txtMaskingMaxNTU.Text = maskingNTUNode.SelectSingleNode("Max")?.InnerText ?? string.Empty;
                }
            }
            isSaved = true;
        }

        public OBSVerticalProfile(_SurveyManager _surveyManager, XmlNode? obsNode)
        {
            InitializeComponent();
            if (obsNode == null)
            {
                surveyManager = _surveyManager;
                InitializeOBS();
                mode = 0;
                this.Text = "Add OBS Vertical Profile";
            }
            else
            {
                surveyManager = _surveyManager;
                obsElement = obsNode as XmlElement;
                PopulateOBS();
                menuNew.Visible = false;
                tableColumnInfo.Enabled = true;
                tableMasking.Enabled = true;
                mode = 1;
                this.Text = "Edit OBS Vertical Profile";
            }

            this.KeyPreview = true; // Enable form to capture key events
            this.KeyDown += OBSVerticalProfile_KeyDown; // Attach key down event handler
            if (surveyManager != null && surveyManager.survey != null)
                project = surveyManager.survey.OwnerDocument;
            if (project == null)
            {
                MessageBox.Show("Project document is not initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    "Current OBS vertical profile has unsaved changes. Do you want to save them before creating a new one?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveOBS();
                    if (status == 1)
                        InitializeOBS(); // Reinitialize the form for a new instrument
                    else
                        return; // Prevent creating a new instrument if save failed

                }
                else if (result == DialogResult.Cancel)
                {
                    return; // User chose to cancel, do not create a new instrument
                }
                else if (result == DialogResult.No)
                {
                    InitializeOBS(); // Reinitialize the form for a new instrument without saving
                }
            }
            else
            {
                InitializeOBS(); // Reinitialize the form for a new instrument if there are no unsaved changes
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveOBS();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    "Current OBS vertical profile has unsaved changes. Do you want to save them before creating a new one?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveOBS();
                    if (status == 1)
                    {
                        this.Close();
                        return;
                    }
                    else
                        return;
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.No)
                {
                    this.Close(); // Close the form without saving
                }
            }
            else
            {
                this.Close(); // Close the form if there are no unsaved changes
            }
        }

        private void OBSVerticalProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    "Current OBS vertical profile has unsaved changes. Do you want to save them before exiting?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveOBS();
                    if (status == 0)
                        e.Cancel = true; // Cancel the closing event if save failed
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Cancel the closing event
                }
            }
        }

        private void input_Changed(object sender, EventArgs e)
        {
            isSaved = false; // Mark as unsaved changes
        }

        public static void updateCombo(ComboBox combo, string[] items, int index)
        {
            combo.Items.Clear();
            combo.Items.AddRange(items);
            combo.SelectedIndex = index;
        }

        private void btnLoadPath_Click(object sender, EventArgs e)
        {
            string[] columns;
            OpenFileDialog openFileDialog = new()
            {
                Filter = "OBS Vertical Profile File (*.csv;*.txt)|*.csv;*.txt",
                Title = "Select OBS Vertical Profile File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                int nLines = File.ReadAllLines(filePath).Length;
                UtilsCSVImportOptions csvOptions = new(nLines);
                if (csvOptions.ShowDialog() == DialogResult.OK)
                {
                    _headerLine = csvOptions._headerLine;
                    _delimiter = csvOptions._delimiter;
                    columns = _Utils.ParseCSVAndReturnColumns(filePath, _delimiter, _headerLine);
                    if (columns.Length < 4)
                    {
                        MessageBox.Show("The selected CSV file does not contain enough columns for OBS Vertical Profile data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return; // User cancelled the CSV options dialog
                }
            }
            else
            {
                return; // User cancelled the file dialog
            }
            XmlNodeList sscModels = _ClassConfigurationManager.GetObjects(type: "NTU2SSC");
            if (sscModels.Count > 0)
            {
                comboSSCModel.Items.Clear();
                foreach (XmlNode modelNode in sscModels)
                {
                    if (modelNode is not XmlElement model)
                        continue;
                    comboSSCModel.Items.Add(new ComboBoxItem(text: model.GetAttribute("name"), id: model.GetAttribute("id")));
                }
                comboSSCModel.DisplayMember = "Text";
                comboSSCModel.Enabled = true; // Enable if models are available
                lblSSCModel.Enabled = true; // Enable label if models are available
                comboSSCModel.SelectedIndex = 0; // Select the first model by default
            }
            else
            {
                MessageBox.Show(
                    "No SSC model found. Please add one and update the setting later",
                    Text = "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                comboSSCModel.Items.Clear();
                comboSSCModel.Enabled = false; // Disable if no models are available
                lblSSCModel.Enabled = false; // Disable label if no models are available
            }
            txtFilePath.Text = openFileDialog.FileName;
            tableColumnInfo.Enabled = true;
            updateCombo(comboDate, columns, 0);
            updateCombo(comboTime, columns, 1);
            updateCombo(comboDepth, columns, 2);
            updateCombo(comboNTU, columns, 3);
            tableMasking.Enabled = true;
            isSaved = false; // Mark as unsaved after loading a new position file
        }

        private void checkMaskingDateTime_CheckedChanged(object sender, EventArgs e)
        {
            if (checkMaskingDateTime.Checked)
            {
                lblMaskingStartDateTime.Enabled = true;
                txtMaskingStartDateTime.Enabled = true;
                lblMaskingEndDateTime.Enabled = true;
                txtMaskingEndDateTime.Enabled = true;
            }
            else
            {
                lblMaskingStartDateTime.Enabled = false;
                txtMaskingStartDateTime.Enabled = false;
                lblMaskingEndDateTime.Enabled = false;
                txtMaskingEndDateTime.Enabled = false;
            }
            isSaved = false; // Mark as unsaved when masking options change
        }

        private void checkMaskingDepth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkMaskingDepth.Checked)
            {
                lblMaskingMinDepth.Enabled = true;
                txtMaskingMinDepth.Enabled = true;
                lblMaskingMaxDepth.Enabled = true;
                txtMaskingMaxDepth.Enabled = true;
            }
            else
            {
                lblMaskingMinDepth.Enabled = false;
                txtMaskingMinDepth.Enabled = false;
                lblMaskingMaxDepth.Enabled = false;
                txtMaskingMaxDepth.Enabled = false;
            }
            isSaved = false; // Mark as unsaved when masking options change
        }

        private void checkMaskingNTU_CheckedChanged(object sender, EventArgs e)
        {
            if (checkMaskingNTU.Checked)
            {
                lblMaskingMinNTU.Enabled = true;
                txtMaskingMinNTU.Enabled = true;
                lblMaskingMaxNTU.Enabled = true;
                txtMaskingMaxNTU.Enabled = true;
            }
            else
            {
                lblMaskingMinNTU.Enabled = false;
                txtMaskingMinNTU.Enabled = false;
                lblMaskingMaxNTU.Enabled = false;
                txtMaskingMaxNTU.Enabled = false;
            }
            isSaved = false; // Mark as unsaved when masking options change
        }

        private int SaveOBS()
        {
            if (String.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show("Please enter a name for the OBS Vertical Profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            if (String.IsNullOrEmpty(txtFilePath.Text.Trim()))
            {
                MessageBox.Show("Please select a file path for the OBS Vertical Profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            if (!File.Exists(_Utils.GetFullPath(txtFilePath.Text.Trim())))
            {
                MessageBox.Show("The specified file path does not exist. Please select a valid file.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            if (comboDate.SelectedItem == null || comboTime.SelectedItem == null || comboDepth.SelectedItem == null || comboNTU.SelectedItem == null)
            {
                MessageBox.Show("Please select valid columns for Date, Time, Depth, and NTU.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            if (comboDate.SelectedIndex < 0 || comboTime.SelectedIndex < 0 || comboDepth.SelectedIndex < 0 || comboNTU.SelectedIndex < 0)
            {
                MessageBox.Show("Please select valid columns for Date, Time, Depth, and NTU.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            int status;
            if (mode == 0)
                status = SAVE();
            else
                status = UPDATE();
            if (status == 0)
            {
                MessageBox.Show("Failed to save the OBS Vertical Profile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            isSaved = true; // Mark as saved after successful save
            return 1;
        }

        private int SAVE()
        {
            id = _ClassConfigurationManager.GetNextId();
            if (surveyManager == null || surveyManager.survey == null)
            {
                MessageBox.Show("Survey manager is not initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            obsElement = surveyManager.survey.OwnerDocument.CreateElement("OBSVerticalProfile");
            obsElement.SetAttribute("id", id.ToString());
            obsElement.SetAttribute("type", "OBSVerticalProfile");
            obsElement.SetAttribute("name", txtName.Text.Trim());
            while (obsElement.HasChildNodes && obsElement.FirstChild != null)
            {
                obsElement.RemoveChild(obsElement.FirstChild);
            }
            // Pd0 related attributes
            if (project == null)
            {
                MessageBox.Show("Project document is not initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            XmlElement fileInfo = project.CreateElement("FileInfo");
            XmlElement filePath = project.CreateElement("Path");
            filePath.InnerText = txtFilePath.Text.Trim();
            fileInfo.AppendChild(filePath);
            XmlElement fileColumns = project.CreateElement("Columns");
            for (int i = 0; i < comboDate.Items.Count; i++)
            {
                XmlElement column = project.CreateElement($"Column{i}");
                column.InnerText = comboDate.Items[i]?.ToString() ?? string.Empty;
                fileColumns.AppendChild(column);
            }
            fileInfo.AppendChild(fileColumns);
            XmlElement xmlElement = project.CreateElement("Header");
            xmlElement.InnerText = _headerLine.ToString();
            fileInfo.AppendChild(xmlElement);
            XmlElement delimiterElement = project.CreateElement("Sep");
            delimiterElement.InnerText = _delimiter ?? ",";
            fileInfo.AppendChild(delimiterElement);
            XmlElement dateColumn = project.CreateElement("DateColumn");
            dateColumn.InnerText = comboDate.Text.Trim() ?? string.Empty;
            fileInfo.AppendChild(dateColumn);
            XmlElement timeColumn = project.CreateElement("TimeColumn");
            timeColumn.InnerText = comboTime.Text.Trim() ?? string.Empty;
            fileInfo.AppendChild(timeColumn);
            XmlElement depthColumn = project.CreateElement("DepthColumn");
            depthColumn.InnerText = comboDepth.Text.Trim() ?? string.Empty;
            fileInfo.AppendChild(depthColumn);
            XmlElement ntuColumn = project.CreateElement("NTUColumn");
            ntuColumn.InnerText = comboNTU.Text.Trim() ?? string.Empty;
            fileInfo.AppendChild(ntuColumn);
            XmlElement sscModel = project.CreateElement("SSCModelID");
            XmlNodeList sscModels = _ClassConfigurationManager.GetObjects(type: "NTU2SSC");
            if (sscModels.Count == 0)
            {
                sscModel.InnerText = string.Empty;
            }
            else
            {
                if (comboSSCModel.SelectedItem is not ComboBoxItem selectedItem)
                {
                    MessageBox.Show("Please select a valid SSC Model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
                string selectedId = selectedItem.ID;
                sscModel.InnerText = selectedId;
            }
            fileInfo.AppendChild(sscModel);
            obsElement.AppendChild(fileInfo);
            XmlElement masking = project.CreateElement("Masking");
            XmlElement maskingDateTime = project.CreateElement("MaskDateTime");
            maskingDateTime.SetAttribute("Enabled", checkMaskingDateTime.Checked.ToString().ToLower());
            XmlElement maskingStartDateTime = project.CreateElement("Start");
            maskingStartDateTime.InnerText = txtMaskingStartDateTime.Text.Trim();
            maskingDateTime.AppendChild(maskingStartDateTime);
            XmlElement maskingEndDateTime = project.CreateElement("End");
            maskingEndDateTime.InnerText = txtMaskingEndDateTime.Text.Trim();
            maskingDateTime.AppendChild(maskingEndDateTime);
            masking.AppendChild(maskingDateTime);
            XmlElement maskingDepth = project.CreateElement("MaskDepth");
            maskingDepth.SetAttribute("Enabled", checkMaskingDepth.Checked.ToString().ToLower());
            XmlElement maskingMinDepth = project.CreateElement("Min");
            maskingMinDepth.InnerText = txtMaskingMinDepth.Text.Trim();
            maskingDepth.AppendChild(maskingMinDepth);
            XmlElement maskingMaxDepth = project.CreateElement("Max");
            maskingMaxDepth.InnerText = txtMaskingMaxDepth.Text.Trim();
            maskingDepth.AppendChild(maskingMaxDepth);
            masking.AppendChild(maskingDepth);
            XmlElement maskingNTU = project.CreateElement("MaskNTU");
            maskingNTU.SetAttribute("Enabled", checkMaskingNTU.Checked.ToString().ToLower());
            XmlElement maskingMinNTU = project.CreateElement("Min");
            maskingMinNTU.InnerText = txtMaskingMinNTU.Text.Trim();
            maskingNTU.AppendChild(maskingMinNTU);
            XmlElement maskingMaxNTU = project.CreateElement("Max");
            maskingMaxNTU.InnerText = txtMaskingMaxNTU.Text.Trim();
            maskingNTU.AppendChild(maskingMaxNTU);
            masking.AppendChild(maskingNTU);
            obsElement.AppendChild(masking);

            surveyManager.survey.AppendChild(obsElement);
            surveyManager.SaveSurvey(name: surveyManager.GetAttribute(attribute: "name"));
            //_project.SaveConfig();
            return 1;
        }

        private static bool UpdateNode(XmlElement? parentElement, string name, string value)
        {
            if (parentElement == null)
            {
                MessageBox.Show(text: "Invalid OBS Vertical Profile element for update.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return false;
            }
            XmlNode? node = parentElement.SelectSingleNode(name);
            if (node != null)
            {
                node.InnerText = value;
            }
            else
            {
                XmlNode newNode = _Globals.Config.CreateElement(name);
                newNode.InnerText = value;
                parentElement.AppendChild(newNode);
            }
            return true;
        }
        private static bool UpdateNode2(XmlElement? parentElement, string name, bool enabled)
        {
            if (parentElement == null)
            {
                MessageBox.Show(text: "Invalid OBS Vertical Profile element for update.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return false;
            }
            XmlNode? node = parentElement.SelectSingleNode(name);
            if (node != null && node is XmlElement element)
            {
                element.SetAttribute("Enabled", enabled.ToString().ToLower());
            }
            else
            {
                XmlElement newElement = _Globals.Config.CreateElement(name);
                newElement.SetAttribute("Enabled", enabled.ToString().ToLower());
                parentElement.AppendChild(newElement);
            }
            return true;
        }

        private int UPDATE()
        {
            if (obsElement == null)
            {
                MessageBox.Show("OBS Vertical Profile element is not initialized properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            obsElement.SetAttribute("name", txtName.Text.Trim());
            XmlElement? fileInfo = obsElement.SelectSingleNode("FileInfo") as XmlElement;
            if (!UpdateNode(fileInfo, "Path", txtFilePath.Text.Trim())) return 0;
            if (fileInfo?.SelectSingleNode("Columns") is not XmlElement columnsElement)
                columnsElement = _Globals.Config.CreateElement("Columns");

            columnsElement.RemoveAll(); // Clear existing columns
            for (int i = 0; i < comboDate.Items.Count; i++)
            {
                XmlElement columnElement = obsElement.OwnerDocument.CreateElement($"Column{i}");
                columnElement.InnerText = comboDate.Items[i]?.ToString() ?? string.Empty;
                columnsElement.AppendChild(columnElement);
            }

            if (!UpdateNode(fileInfo, "Header", _headerLine.ToString())) return 0;
            if (!UpdateNode(fileInfo, "Sep", _delimiter ?? ",")) return 0;
            if (!UpdateNode(fileInfo, "DateColumn", comboDate.SelectedItem?.ToString() ?? string.Empty)) return 0;
            if (!UpdateNode(fileInfo, "TimeColumn", comboTime.SelectedItem?.ToString() ?? string.Empty)) return 0;
            if (!UpdateNode(fileInfo, "DepthColumn", comboDepth.SelectedItem?.ToString() ?? string.Empty)) return 0;
            if (!UpdateNode(fileInfo, "NTUColumn", comboNTU.SelectedItem?.ToString() ?? string.Empty)) return 0;

            XmlNode? sscModelNode = fileInfo?.SelectSingleNode("SSCModelID");
            if (sscModelNode == null)
            {
                sscModelNode = _Globals.Config.CreateElement("SSCModelID");
                fileInfo?.AppendChild(sscModelNode);
            }
            if (comboSSCModel.SelectedItem is not ComboBoxItem selectedItem)
            {
                MessageBox.Show("Please select a valid SSC Model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            string selectedId = selectedItem.ID;
            sscModelNode.InnerText = selectedId;

            if (obsElement.SelectSingleNode("Masking") is XmlElement maskingNode)
            {
                if (!UpdateNode2(maskingNode, "MaskDateTime", checkMaskingDateTime.Checked)) return 0;
                XmlElement? maskingDateTimeNode = maskingNode.SelectSingleNode("MaskDateTime") as XmlElement;
                if (!UpdateNode(maskingDateTimeNode, "Start", txtMaskingStartDateTime.Text.Trim())) return 0;
                if (!UpdateNode(maskingDateTimeNode, "End", txtMaskingEndDateTime.Text.Trim())) return 0;
                if (!UpdateNode2(maskingNode, "MaskDepth", checkMaskingDepth.Checked)) return 0;
                XmlElement? maskingDepthNode = maskingNode.SelectSingleNode("MaskDepth") as XmlElement;
                if (!UpdateNode(maskingDepthNode, "Min", txtMaskingMinDepth.Text.Trim())) return 0;
                if (!UpdateNode(maskingDepthNode, "Max", txtMaskingMaxDepth.Text.Trim())) return 0;
                if (!UpdateNode2(maskingNode, "MaskNTU", checkMaskingNTU.Checked)) return 0;
                XmlElement? maskingNTUNode = maskingNode.SelectSingleNode("MaskNTU") as XmlElement;
                if (!UpdateNode(maskingNTUNode, "Min", txtMaskingMinNTU.Text.Trim())) return 0;
                if (!UpdateNode(maskingNTUNode, "Max", txtMaskingMaxNTU.Text.Trim())) return 0;
            }
            else
            {
                MessageBox.Show("Masking node is missing in the OBS Vertical Profile configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            _project.SaveConfig();
            return 1;
        }

        private void OBSVerticalProfile_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S) // Ctrl + S
            {
                e.SuppressKeyPress = true; // Prevent default behavior
                if (sender != null)
                    menuSave_Click(sender, e); // Save the current project
            }
            else if (e.Control && e.KeyCode == Keys.N) // Ctrl + N
            {
                if (mode == 0)
                {
                    e.SuppressKeyPress = true; // Prevent default behavior
                    if (sender != null)
                        menuNew_Click(sender, e); // Create a new project
                }
            }
        }
    }

}
