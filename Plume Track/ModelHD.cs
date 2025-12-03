using DHI.Generic.MikeZero.DFS;
using DHI.Generic.MikeZero.DFS.dfs123;
using DHI.Generic.MikeZero.DFS.dfsu;
using Plume_Track;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Plume_Track
{
    public partial class ModelHD : Form
    {

        public bool isSaved;
        public XmlElement? modelElement;
        public int mode;
        public _ClassConfigurationManager _project = new();

        private void InitializeModel()
        {
            txtModelName.Text = "New HD Model";
            txtFilePath.Text = string.Empty;
            isSaved = true;
        }

        private void PopulateModel()
        {
            if (modelElement == null)
            {
                MessageBox.Show(text: "Invalid HD model node provided.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                this.Close();
                return;
            }
            txtModelName.Text = modelElement.GetAttribute("name");
            XmlNode? pathNode = modelElement.SelectSingleNode("Path");
            if (pathNode != null)
            {
                txtFilePath.Text = pathNode?.InnerText ?? string.Empty;
                updateCombos(txtFilePath.Text);
            }
            else
            {
                MessageBox.Show(text: "Path node is missing in the HD model configuration.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                this.Close();
            }
            XmlNode? uItemNode = modelElement.SelectSingleNode("UItemNumber");
            if (uItemNode != null && int.TryParse(uItemNode.InnerText, out int uItemIndex))
            {
                uItemIndex--; // Convert to zero-based index
                if (uItemIndex >= 0 && uItemIndex < comboUItem.Items.Count)
                {
                    comboUItem.SelectedIndex = uItemIndex;
                    comboUItem.Enabled = true;
                }
                else
                {
                    MessageBox.Show(text: "U Item index is out of range in the HD model configuration.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show(text: "U Item node is missing or invalid in the HD model configuration.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                this.Close();
            }
            XmlNode? vItemNode = modelElement.SelectSingleNode("VItemNumber");
            if (vItemNode != null && int.TryParse(vItemNode.InnerText, out int vItemIndex))
            {
                vItemIndex--; // Convert to zero-based index
                if (vItemIndex >= 0 && vItemIndex < comboVItem.Items.Count)
                {
                    comboVItem.SelectedIndex = vItemIndex;
                    comboVItem.Enabled = true;
                }
                else
                {
                    MessageBox.Show(text: "V Item index is out of range in the HD model configuration.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show(text: "V Item node is missing or invalid in the HD model configuration.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                this.Close();
            }
            isSaved = true;
        }

        public ModelHD(XmlNode? modelNode)
        {
            InitializeComponent();
            if (modelNode == null)
            {
                InitializeModel();
                mode = 0; // New model mode
                this.Text = "Add HD Model";
            }
            else
            {
                modelElement = modelNode as XmlElement;
                PopulateModel();
                menuNew.Visible = false; // Hide New menu option if editing an existing model
                mode = 1; // Edit model mode
                this.Text = "Edit HD Model";
            }

            this.KeyPreview = true; // Enable form to capture key events
            this.KeyDown += Model_KeyDown; // Attach key down event handler
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (!isSaved)
            {
                DialogResult result = MessageBox.Show(
                    text: "You have unsaved changes. Do you want to save before creating a new HD model?",
                    caption: "Confirm New HD Model",
                    buttons: MessageBoxButtons.YesNoCancel,
                    icon: MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int status = SaveModel();
                    if (status == 1)
                        InitializeModel();
                    else
                        return;

                }
                else if (result == DialogResult.Cancel)
                {
                    return; // User chose to cancel
                }
                else
                {
                    InitializeModel(); // User chose not to save, proceed with new model
                }
            }
            else
            {
                InitializeModel();
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            SaveModel();
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
                    int status = SaveModel();
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

        private void HDModel_FormClosing(object sender, FormClosingEventArgs e)
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
                    int status = SaveModel();
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

        private void updateCombos(string fileName)
        {
            comboUItem.Items.Clear();
            comboVItem.Items.Clear();
            //string extension = System.IO.Path.GetExtension(fileName).ToLower();
            DfsuFile dfsFile = DfsuFile.Open(fileName);
            int NItems = dfsFile.ItemInfo.Count;
            if (NItems < 2)
            {
                MessageBox.Show(text: "The selected DFSU file does not contain enough items for U and V components.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < NItems; i++)
            {
                string name = dfsFile.ItemInfo[i].Name;
                comboUItem.Items.Add(name);
                comboVItem.Items.Add(name);
            }
            dfsFile.Close();
            comboUItem.SelectedIndex = 0;
            comboVItem.SelectedIndex = 1;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Select Model File",
                Filter = "DFSU (*.dfsu)|*.dfsu",
                Multiselect = false,
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = ofd.FileName;
                updateCombos(txtFilePath.Text);
                comboUItem.Enabled = true;
                comboVItem.Enabled = true;
                isSaved = false; // Mark as unsaved changes
            }
        }

        private int SaveModel()
        {
            if (String.IsNullOrEmpty(txtModelName.Text))
            {
                MessageBox.Show(text: "Model name cannot be empty.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return 0;
            }
            if (String.IsNullOrEmpty(txtFilePath.Text))
            {
                MessageBox.Show(text: "File path cannot be empty.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return 0;
            }
            if (!File.Exists(_Utils.GetFullPath(txtFilePath.Text.Trim())))
            {
                MessageBox.Show(text: "File does not exist at the specified path.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return 0;
            }
            if (comboUItem.SelectedIndex < 0 || comboVItem.SelectedIndex < 0)
            {
                MessageBox.Show(text: "Please select a valid item from the model file.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return 0;
            }
            if (mode == 0)
                SAVE();
            else
                UPDATE();
            isSaved = true;
            return 1; // Return 1 to indicate success
        }

        private void SAVE()
        {
            int id = _ClassConfigurationManager.GetNextId();
            modelElement = _Globals.Config.CreateElement("HDModel");
            if (modelElement != null)
            {
                modelElement.SetAttribute("name", txtModelName.Text);
                modelElement.SetAttribute("type", "HDModel");
                modelElement.SetAttribute("id", id.ToString());
                modelElement.SetAttribute("name", txtModelName.Text);
                XmlElement path = _Globals.Config.CreateElement("Path");
                path.InnerText = _Utils.GetFullPath(txtFilePath.Text.Trim());
                modelElement?.AppendChild(path);
                XmlElement uItem = _Globals.Config.CreateElement("UItemNumber");
                uItem.InnerText = (comboUItem.SelectedIndex + 1).ToString();
                modelElement?.AppendChild(uItem);
                XmlElement vItem = _Globals.Config.CreateElement("VItemNumber");
                vItem.InnerText = (comboVItem.SelectedIndex + 1).ToString();
                modelElement?.AppendChild(vItem);
                var doc = _Globals.Config.DocumentElement;
                if (doc != null && modelElement != null)
                {
                    doc.AppendChild(modelElement);
                }
                _project.SaveConfig();
            }
            else
            {
                MessageBox.Show(text: "Failed to create model element.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }

        }

        private void UpdateNode(string name, string value)
        {
            if (modelElement == null)
            {
                MessageBox.Show(text: "Invalid model element for update.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }
            XmlNode? node = modelElement.SelectSingleNode(name);
            if (node != null)
            {
                node.InnerText = value;
            }
            else
            {
                XmlNode newNode = _Globals.Config.CreateElement(name);
                newNode.InnerText = value;
                modelElement.AppendChild(newNode);
            }
        }

        private void UPDATE()
        {
            if (modelElement == null)
            {
                MessageBox.Show(text: "Invalid model element for update.", caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }
            modelElement.SetAttribute("name", txtModelName.Text);
            UpdateNode("Path", _Utils.GetFullPath(txtFilePath.Text.Trim()));
            UpdateNode("UItemNumber", (comboUItem.SelectedIndex + 1).ToString());
            UpdateNode("VItemNumber", (comboVItem.SelectedIndex + 1).ToString());
            _project.SaveConfig();
        }

        private void Model_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S) // Ctrl + S
            {
                e.SuppressKeyPress = true; // Prevent default behavior
                SaveModel(); // Save the project
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

        private void menuDfs22Dfsu_Click(object sender, EventArgs e)
        {
            var iofd = new OpenFileDialog
            {
                Title = "Select Model File",
                Filter = "DFS2 (*.dfs2)|*.dfs2",
                Multiselect = false,
            };
            if (iofd.ShowDialog() == DialogResult.OK)
            {
                var osfd = new SaveFileDialog
                {
                    Title = "Select Output DFSU File",
                    Filter = "DFSU (*.dfsu)|*.dfsu",
                };
                if (osfd.ShowDialog() == DialogResult.OK)
                {
                    var input = new Dictionary<string, string>
                {
                    {"Task", "Dfs2ToDfsu" },
                    {"InPath", _Utils.GetFullPath(iofd.FileName) },
                    {"OutPath", _Utils.GetFullPath(osfd.FileName) }
                };
                    string xmlInput = _Tools.GenerateInput(input);
                    XmlDocument doc = _Tools.CallPython(xmlInput);
                    Dictionary<string, string> output = _Tools.ParseOutput(doc);
                    if (output.TryGetValue("Error", out string? value))
                    {
                        MessageBox.Show(value, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Dfs2 file converted to Dfsu successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
