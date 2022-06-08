
using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace EnhancedComponents
{


    /// <summary>
    /// Interaktionslogik für HierarchyTreeView.xaml
    /// </summary>
    public partial class HierarchyTreeView : TreeView
    {
        public HierarchyTreeView()
        {
            InitializeComponent();
        }

        private string _filesystem;

        public string filesystem
        {
            get { return _filesystem; }
            set 
            {
                InitializeFileSystem(value);
                _filesystem = value; 
            }
        }


        private void InitializeFileSystem(string filesystem)
        {
            this.Items.Clear();
            HierarchyItem hierarchyItem = new HierarchyItem(HierarchyType.Folder);
            hierarchyItem.isRoot = true;
            hierarchyItem.rootPath = filesystem;
            hierarchyItem.filename = new DirectoryInfo(filesystem).Name;
            this.Items.Add(hierarchyItem);
            hierarchyItem.Items.Add(new TokenItem());
            hierarchyItem.IsExpanded = true;
        }

        public HierarchyItem SelectedHierarchyItem
        {
            get 
            {
                return this.SelectedItem as HierarchyItem;
            }
        }

    }
}
