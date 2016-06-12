public class Folder : Node {
    public Node Parent;

    public Folder(string name, Node parent) {
        Name = name;
        Parent = parent;

        Parent.ChildrenNodes.Add(this);
    }


    public override string Path() {
        return Parent.Path() + "/" + Name + "/";
    }
}