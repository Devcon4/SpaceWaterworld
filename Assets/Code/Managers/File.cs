public class File : IFileable {
    public Node Parent;
    public string Name;
    public string Extension;
    public string ActualLocation = null;
    public string Content = null;

    public File(string name, string extension, Node parent, string actualLocation = null, string content = null) {
        Name = name;
        Extension = extension;
        Parent = parent;
        ActualLocation = actualLocation;
        Content = content;

        Parent.Files.Add(this);
    }

    public string Path() {
        return Parent.Path() + "/" + Name;
    }

    public string FormattedName() {
        return Name + "." + Extension;
    }
}