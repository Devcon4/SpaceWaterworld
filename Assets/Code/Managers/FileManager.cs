public static class FileManager {

    public static string UniqueName(string name, Node parent) {
        var index = 1;
        if(parent.ChildrenNodes.TrueForAll(x => x.FormattedName() != name)) { return name; }
        while (parent.ChildrenNodes.Exists(x => x.FormattedName() == name + "-" + index)) {
            index++;
        }
        return name + "-" + index;
    }
}