using Plume_Track;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Plume_Track
{

    public static class _Utils
    {
        public static string[] ParseCSVAndReturnColumns(string filePath, string separator, int headerLine)
        {
            var inputs = new Dictionary<string, string>
            {
                { "Task", "GetColumnsFromCSV" },
                { "Path", filePath },
                { "Sep", separator },
                { "Header", headerLine.ToString() }
            };
            string xmlInput = _Tools.GenerateInput(inputs);
            XmlDocument doc = _Tools.CallPython(xmlInput);
            Dictionary<string, string> output = _Tools.ParseOutput(doc);
            if (output.TryGetValue("Error", out string? value))
            {
                MessageBox.Show(value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return [];
            }
            int nColumns = Convert.ToInt32(output["NColumns"]);
            if (nColumns == 0)
            {
                MessageBox.Show("No columns found in the CSV file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return [];
            }
            else
            {
                var columns = new string[nColumns];
                for (int i = 0; i < nColumns; i++)
                {
                    columns[i] = output[$"Column{i}"] ?? $"Column{i}";
                }
                return columns;
            }
        }

        public static string GetFullPath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }
            else
            {
                string directory = _ClassConfigurationManager.GetSetting(settingName: "Directory");
                return Path.Combine(directory, filePath);
            }
        }

        public static Label CreateLabel(string name, string text)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                Name = name,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }
        public static TextBox CreateTextBox(string name, string text = "")
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Name = name,
                Text = text,
            };
        }
        public static ComboBox CreateComboBox(string name, string[] items, int selectedIndex = 0)
        {
            var comboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                Name = name,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            comboBox.Items.AddRange(items);
            if (selectedIndex > -1)
                comboBox.SelectedIndex = selectedIndex;
            return comboBox;
        }
        public static NumericUpDown CreateNumericUpDown(string name, decimal min, decimal max, decimal value, bool enabled = true)
        {
            return new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Name = name,
                Minimum = min,
                Maximum = max,
                Value = value,
                Enabled = enabled
            };
        }
        public static CheckBox CreateCheckBox(string name, string text, bool isChecked)
        {
            return new CheckBox
            {
                Dock = DockStyle.Fill,
                Name = name,
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Checked = isChecked
            };
        }
        public static Button CreateButton(string name, string text)
        {
            return new Button
            {
                Dock = DockStyle.Fill,
                Name = name,
                Text = text
            };
        }

        public static Panel CreatePanel(string name, Color color)
        {
            return new Panel
            {
                Name = name,
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
            };
        }

        public static GroupBox CreateGroupBox(string name, string text)
        {
            return new GroupBox
            {
                Name = name,
                Text = text,
                Dock = DockStyle.Fill,
            };
        }

        public static TableLayoutPanel CreateTable(string name, int columnCount, float[] columnWidths)
        {
            TableLayoutPanel table = new()
            {
                Name = name,
                Dock = DockStyle.Fill,
                ColumnCount = columnCount,
            };
            for (int i = 0; i < columnCount; i++)
            {
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, columnWidths[i]));
            }
            return table;
        }


        public static void ConfigureTable(TableLayoutPanel table, IList<List<Control?>> rows, List<float> rowHeights, bool addLast=false)
        {
            table.SuspendLayout();
            table.Controls.Clear();
            table.RowStyles.Clear();
            int nRows = rows.Count;
            if (addLast)
                nRows += 1;
            table.RowCount = nRows;
            // Determine how many columns we need (max controls per row)
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                // Set row style
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeights[rowIndex]));
                var row = rows[rowIndex];
                if (row == null)
                    continue;
                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    var control = row[colIndex];
                    if (control == null)
                        continue; // allow "holes"
                    table.Controls.Add(control, colIndex, rowIndex);
                }
            }
            if (addLast)
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            table.ResumeLayout();
        }

     
    }
}
