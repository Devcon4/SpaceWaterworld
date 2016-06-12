using System.Collections.Generic;

public abstract class Node : IFileable {
    public string Name;
    public List<IFileable> ChildrenNodes = new List<IFileable>();

    public string FormattedName() {
        return Name;
    }

    public abstract string Path();
}