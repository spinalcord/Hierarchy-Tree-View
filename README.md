# Hierarchy-Tree-View
WPF doesn't have a native solution to display a filesystem. Here is a lightweight solution to use a filesystem inside your application. This TreeView doesn't load the entire filesystem, instead it will place a token inside a folder node. If you expand the folder, which contains the token, more files and folders will be loaded, this method allows to use heavy filesystem without attacking the performance.

# Basic Commands


• Initialize the filesystem

SomeHierarchyComponent.filesystem = "C:/Users/..."

• Rename a file/folder

SomeHierarchyComponent.SelectedHierarchyItem.filename = "new Name"

• Delete a file/folder

SomeHierarchyComponent.SelectedHierarchyItem.Delete();

• More commands

Avaible in the example.zip

# Preview

![Animation](https://user-images.githubusercontent.com/4529150/172671733-b26b2539-37b2-49e2-8dae-fc0c516c85fd.gif)
