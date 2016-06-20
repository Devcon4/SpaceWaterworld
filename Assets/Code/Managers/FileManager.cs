using System.Collections.Generic;
using System.Linq;

public static class FileManager {

    public static string UniqueName(string name, Node parent) {
        var index = 1;
        if(parent.ChildrenNodes.Concat(parent.Files.Cast<IFileable>()).ToList().TrueForAll(x => x.FormattedName() != name)) { return name; }
        while (parent.ChildrenNodes.Concat(parent.Files.Cast<IFileable>()).ToList().Exists(x => x.FormattedName() == name + "-" + index)) {
            index++;
        }
        return name + "-" + index;
    }

    public static string UniqueVolumeName(string name, List<Volume> root) {
        string[] validDrives = new[] {"C", "D", "E", "F", "G", "H", "I", "J"};
        var index = 1;
        if (root.TrueForAll(x => x.Name != name)) { return name; }
        while (root.Exists(x => x.Name == name)) {
            name = validDrives[index % validDrives.Length];
        }
        return name;
    }
}