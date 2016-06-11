using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FileSystem {
    public List<Volume> Root = new List<Volume> { new Volume("C") };

    public FileSystem(string type = null) {
        switch (type) {
            case "GALAXSTARCONSOLE":

                var C = new Volume("C");

                new Folder("bin", C);
                var files = new Folder("files", C);

                new Folder("executables", files);
                var docs = new Folder("documents", files);

                var copyright = new File("copyright", "txt", docs, content: "PROPERTY OF GALAXSTAR INCORPERATED");

                Root = new List<Volume> { C };
                break;
        }
    }
}

public interface IFileable {
    string Path();
    string FormattedName();
}

public abstract class Node : IFileable {
    public string Name;
    public List<IFileable> ChildrenNodes = new List<IFileable>();

    public string FormattedName() {
        return Name;
    }

    public abstract string Path();
}

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

public class Volume : Node {

    public Volume(string name) {
        Name = name;
    }

    public override string Path() {
        return Name + ":/";
    }
}

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

        Parent.ChildrenNodes.Add(this);
    }

    public string Path() {
        return Parent.Path() + "/" + Name;
    }

    public string FormattedName() {
        return Name + "." + Extension;
    }
}

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