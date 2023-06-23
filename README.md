# Hierarchy-Tree-View
WPF does not provide a built-in solution for displaying a filesystem. However, I have developed a lightweight solution that allows you to incorporate a filesystem into your application. Instead of loading the entire filesystem into a TreeView, this solution utilizes a token system. When you expand a folder node that contains a token, additional files and folders will be loaded dynamically. This approach ensures that heavy filesystems can be used without compromising performance.

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
