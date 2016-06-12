public class Volume : Node {

    public Volume(string name) {
        Name = name;
    }

    public override string Path() {
        return Name + ":/";
    }
}