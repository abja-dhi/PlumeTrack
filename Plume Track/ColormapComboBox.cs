using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Plume_Track
{
    public class ColormapComboBox : ComboBox
    {
        public string CMapsPath { get; private set; }
        [DefaultValue(100)]
        public int TextColumnWidth { get; set; } = 100;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? SelectedColormapName
        {
            get => (SelectedItem as ColormapItem)?.Name;
            set => SelectColormapByName(value);
        }
        public ColormapComboBox(string cmapsPath, string name)
        {
            CMapsPath = cmapsPath;
            Dock = DockStyle.Fill;
            DropDownStyle = ComboBoxStyle.DropDownList;
            FormattingEnabled = true;
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 24;
            Name = name;
            DrawItem += OnDrawItem;
            RefreshFromPath();
        }

        public void SetPath(string cmapsPath)
        {
            CMapsPath = cmapsPath;
            RefreshFromPath();
        }

        public void RefreshFromPath()
        {
            BeginUpdate();
            try
            {
                foreach (var it in Items.OfType<ColormapItem>())
                    it.Preview?.Dispose();
                Items.Clear();
                if (Directory.Exists(CMapsPath))
                {
                    var files = Directory.GetFiles(CMapsPath, "*.png");
                    foreach (var file in files)
                    {
                        var name = Path.GetFileNameWithoutExtension(file);
                        string path = Path.Combine(CMapsPath, file);
                        Image? img = null;
                        try { img = Image.FromFile(path); } catch {  /* skip unreadable */}
                        Items.Add(new ColormapItem(name, img));
                    }
                }
                if (Items.Count > 0)
                    SelectedIndex = 0;
            }
            finally
            {
                EndUpdate();
            }
        }
        public void SelectColormapByName(string? name)
        {
            if (Items.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(name))
                name = "turbo";

            int index = FindColormapIndex(name);

            if (index < 0)
            {
                // fallback to "turbo"
                index = FindColormapIndex("turbo");
            }

            if (index < 0)
            {
                // final fallback: first item
                index = 0;
            }

            SelectedIndex = index;
        }
        private int FindColormapIndex(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is ColormapItem item &&
                    string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
        private void OnDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();

            var item = (ColormapItem)Items[e.Index];

            using (var brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(
                    item.Name,
                    e.Font,
                    brush,
                    e.Bounds.Left + 2,
                    e.Bounds.Top + (e.Bounds.Height - e.Font.Height) / 2f
                );
            }

            if (item.Preview is not null)
            {
                int imageX = e.Bounds.Left + TextColumnWidth;
                int imageWidth = Math.Max(0, e.Bounds.Right - imageX - 2);
                int imageHeight = e.Bounds.Height - 4;
                var imageRect = new Rectangle(imageX, e.Bounds.Top + 2, imageWidth, imageHeight);
                e.Graphics.DrawImage(item.Preview, imageRect);
            }

            e.DrawFocusRectangle();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var it in Items.OfType<ColormapItem>())
                    it.Preview?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
