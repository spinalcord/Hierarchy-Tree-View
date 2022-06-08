using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Diagnostics;

namespace EnhancedComponents
{
    public enum HierarchyType
    {
        File,
        Folder
    }
    //

    // A Token is a hint, so that we know there is something in a HierarchyItem (type:folder), if we expand the item we are going to read the information in the token.
    // If we expand the folder, we can read the information in the token (=> load more files) and then we remove the token,  
    public class TokenItem : TreeViewItem
    {
        public string Path
        {
            get
            {
                return (this.Parent as HierarchyItem).FolderPath;
            }
        }

    }

    public partial class HierarchyItem : TreeViewItem
    {
        public HierarchyItem(HierarchyType targetType)
        {
            InitializeComponent();
            type = targetType;
        }

        public static string FileExistsMessage = "File exists";
        public static string FolderExistsMessage = "Folder exists";
        public static string InvalidNameMessage = "Invalid Name";
        public static string NameExistsMessage = "Name exists";


        public bool isRoot = false;

        private string _rootPath;

        public string rootPath
        {
            get
            {
                return _rootPath;
            }
            set
            {
                value = value.Replace("/", "\\");
                if (value.EndsWith("\\") == false)
                    value = value + "\\";

                _rootPath = value;
            }
        }

        public bool showExtension = false;

        public string Text
        {
            get
            {
                return textBlock.Text;
            }
            set
            {
                if (showExtension == false)
                {
                    textBlock.Text = Path.GetFileNameWithoutExtension(value);
                }
                else
                {
                    textBlock.Text = value + extension;
                }
            }
        }


        private string _filename;

        public string filename
        {
            get
            {
                return _filename;
            }
            set
            {

                if (this.isRoot)
                {
                    // Prevent renaming the root folder
                    _filename = new DirectoryInfo(rootPath).Name;
                    Text = _filename;
                }
                else
                {
                    
                    if(value.Any(f => Path.GetInvalidFileNameChars().Contains(f)) || string.IsNullOrWhiteSpace(value))
                    {
                        MessageBox.Show(InvalidNameMessage);
                        return;
                    }

                    bool exists = false;

                    if (_filename != null && _filename != value)
                    {

                        if (this.type == HierarchyType.Folder)
                        {
                            if (Directory.Exists((this.Parent as HierarchyItem).FolderPath + value) == false)
                            {
                                string p1 = this.FolderPath;
                                Directory.Move(p1, (this.Parent as HierarchyItem).FolderPath + value);
                            }
                            else
                            {
                                exists = true;
                            }
                        }
                        else if (this.type == HierarchyType.File)
                        {
                            if (File.Exists(FolderPath + value + this.extension) == false)
                            {
                                string p1 = this.absolutePath;
                                File.Move(p1, FolderPath + value + this.extension);
                            }
                            else
                                exists = true;
                        }

                    }
                    if (exists == false)
                    {
                        _filename = value;
                        Text = _filename;
                    }
                    else
                        MessageBox.Show(NameExistsMessage);
                }

            }
        }


        public void Delete()
        {
            if (this.isRoot == false)
            {
                string location = this.absolutePath;
                if(this.type == HierarchyType.Folder)
                    Directory.Delete(absolutePath, true);
                else if (this.type == HierarchyType.File)
                    File.Delete(absolutePath);

                (this.Parent as HierarchyItem).Items.Remove(this);
            }
        }


        private HierarchyType _type = HierarchyType.Folder;
        public HierarchyType type
        {
            get { return _type; }
            set
            {
                if (value == HierarchyType.File)
                {
                    FileIcon.Visibility = Visibility.Visible;
                    FolderIcon.Visibility = Visibility.Collapsed;
                }
                else
                {
                    FileIcon.Visibility = Visibility.Collapsed;
                    FolderIcon.Visibility = Visibility.Visible;
                }

                _type = value;
            }
        }

        private string _extension;

        public string extension
        {
            get
            {
                if (type == HierarchyType.File)
                {

                    if (!string.IsNullOrWhiteSpace(_extension))
                    {
                        if (_extension.StartsWith("."))
                            return _extension;
                        else
                            return "." + _extension;
                    }
                    else
                        return _extension;
                }
                else
                    return "";
            }
            set { _extension = value; }
        }

        public string relativePath
        {
            get
            {
                if (isRoot == true)
                {
                    return "";
                }
                else
                {
                    HierarchyItem current = this;
                    string p = "";
                    while (current != null)
                    {
                        if (current.Parent.GetType() == typeof(HierarchyItem))
                        {
                            if ((current.Parent as HierarchyItem).isRoot == false)
                                p = (current.Parent as HierarchyItem).filename + "\\" + p;

                            current = current.Parent as HierarchyItem;
                        }
                        else
                            current = null;
                    }


                    if (type == HierarchyType.File)
                        p = p + filename + extension;
                    else if (type == HierarchyType.Folder)
                        p = p + filename + "\\";

                    return p;

                }

            }
        }

        public string absolutePath
        {
            get
            {
                if (isRoot == true)
                    return rootPath;
                else
                {
                    HierarchyItem current = this;
                    while (current != null)
                    {
                        if (current.Parent.GetType() == typeof(HierarchyItem))
                        {
                            current = current.Parent as HierarchyItem;
                            if (current.isRoot == true)
                            {

                                return current.rootPath + relativePath;
                            }

                        }
                        else
                            current = null;
                    }
                    return "";
                }

            }
        }

        // Return FOLDER PATH if you select a file, and return FOLDER PATH if you select a folder
        public string FolderPath
        {
            get
            {
                if (isRoot == true)
                    return rootPath;
                else
                {
                    if (this.type == HierarchyType.File)
                        return (this.Parent as HierarchyItem).absolutePath;
                    else
                        return absolutePath;
                }

            }
        }

        public void OpenFolder()
        {
            Process.Start(FolderPath);
        }

        /// <summary>
        /// you can also change the name if you edit the "filename" property 
        /// </summary>
        /// <param name="newName"></param>
        public void RenameHierarchyItem(string newName)
        {
            filename = newName;
        }


        /// <summary>
        /// Create a file inside this node
        /// </summary>
        /// <param name="n">filename</param>
        /// <param name="ext">extension</param>
        /// <returns></returns>
        public string CreateHierarchyItem(HierarchyType hierarchyType, string n, string ext = ".xml")
        {
            if (n.Any(f => Path.GetInvalidFileNameChars().Contains(f)) || string.IsNullOrWhiteSpace(n))
            {
                MessageBox.Show(InvalidNameMessage);
                return "";
            }

            if (hierarchyType == HierarchyType.File)
            {
                if (!ext.StartsWith("."))
                    ext = "." + ext;

                if (File.Exists(FolderPath + n + ext) == false)
                {
                    HierarchyItem hierarchyItemFile = new HierarchyItem(HierarchyType.File);
                    hierarchyItemFile.extension = ext;
                    hierarchyItemFile.filename = n;

                

                    if (this.GetToken() == null)
                    {
                        if (this.type == HierarchyType.File)
                            (this.Parent as HierarchyItem).Items.Add(hierarchyItemFile);
                        else if (this.type == HierarchyType.Folder)
                            this.Items.Add(hierarchyItemFile);
                    }
                    else
                        this.AddToken(this);

                    File.Create(FolderPath + n + ext).Dispose();
                    return FolderPath + n + ext;
                }
                else
                {
                    MessageBox.Show(FileExistsMessage);
                    return "";
                }
            }
            else if (hierarchyType == HierarchyType.Folder)
            {
                if (Directory.Exists(FolderPath + n) == false)
                {
                    HierarchyItem hierarchyItemFile = new HierarchyItem(HierarchyType.Folder);
                    hierarchyItemFile.extension = "";
                    hierarchyItemFile.filename = n;

                    if (this.GetToken() == null)
                    {
                        if (this.type == HierarchyType.File)
                            (this.Parent as HierarchyItem).Items.Add(hierarchyItemFile);
                        else if (this.type == HierarchyType.Folder)
                            this.Items.Add(hierarchyItemFile);
                    }
                    else
                        this.AddToken(this);

                    Directory.CreateDirectory(FolderPath + n);
                    return FolderPath + n;
                }
                else
                {
                    MessageBox.Show(FolderExistsMessage);
                    return "";
                }
            }
            return "";
        }

        public bool Move(HierarchyItem target)
        {
            if (this.isRoot == false && this != target)
            {
                if (target.Parent == this)
                {
                    return false;
                }
                else
                {
                    if (this.type == HierarchyType.Folder)
                    {
                        if (Directory.Exists(target.FolderPath + filename) == false)
                        {
                            string p1 = this.FolderPath;
                            (this.Parent as HierarchyItem).Items.Remove(this);

                            Directory.Move(p1, target.FolderPath + filename);

                            if (target.GetToken() == null)
                            {
                                if (target.type == HierarchyType.Folder)
                                    target.Items.Add(this);//.Insert(0, this);
                                else if (target.type == HierarchyType.File)
                                    (target.Parent as HierarchyItem).Items.Add(this);
                            }
                        }
                        else
                            MessageBox.Show(FolderExistsMessage);
                    }
                    else if (this.type == HierarchyType.File)
                    {
                        if (File.Exists(target.FolderPath + filename + this.extension) == false)
                        {
                            string p1 = this.absolutePath;
                            (this.Parent as HierarchyItem).Items.Remove(this);
                            File.Move(p1, target.FolderPath + filename + this.extension);
                            if (target.GetToken() == null)
                            {
                                if (target.type == HierarchyType.File)
                                    (target.Parent as HierarchyItem).Items.Insert(0, this);
                                else
                                    target.Items.Add(this);//.Insert(0, this);
                            }
                        }
                        else
                            MessageBox.Show(FileExistsMessage);
                    }

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public TokenItem GetToken()
        {
            return this.Items.SourceCollection.OfType<TokenItem>().FirstOrDefault();
        }

        public void AddToken(HierarchyItem target)
        {
            if (target.IsExpanded == false && target.GetToken() == null)
                target.Items.Add(new TokenItem());
        }

        protected override void OnExpanded(RoutedEventArgs e)
        {

            if (this.Items.Count != 0 && GetToken() != null) // If there is no token, we don't need to load everything again.
            {
                foreach (string directory in Directory.GetDirectories(GetToken().Path))
                {

                    HierarchyItem hierarchyItem = new HierarchyItem(HierarchyType.Folder);
                    this.Items.Add(hierarchyItem);

                    hierarchyItem.filename = new DirectoryInfo(directory).Name;


                    if (Directory.GetFiles(directory).Length > 0 || Directory.GetDirectories(directory).Length > 0)
                    {
                        // Add a Token => so that we know that this folder must contain something
                        AddToken(hierarchyItem);
                    }
                }


                foreach (string f in Directory.GetFiles(GetToken().Path))
                {
                    HierarchyItem hierarchyItemFile = new HierarchyItem(HierarchyType.File);
                    hierarchyItemFile.extension = Path.GetExtension(f);
                    hierarchyItemFile.filename = Path.GetFileNameWithoutExtension(f);
                    this.Items.Add(hierarchyItemFile);
                }

                List<TokenItem> RemoveList = new List<TokenItem>();
                foreach (TokenItem a in this.Items.SourceCollection.OfType<TokenItem>())
                    RemoveList.Add(a);

                foreach (TokenItem b in RemoveList)
                    this.Items.Remove(b);




            }

            base.OnExpanded(e);
        }


    }
}
