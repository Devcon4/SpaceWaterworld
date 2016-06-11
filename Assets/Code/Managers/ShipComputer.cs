using System.Collections.Generic;
using UnityEngine;

public class ShipComputer : MonoBehaviour {
    public string Name;
    public FileSystem Directory = new FileSystem("GALAXSTARCONSOLE");
    public List<Component> Components;

    void Start() {
    }
}