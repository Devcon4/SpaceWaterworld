using System.Collections.Generic;
using Jurassic;
using UnityEngine;

public class ShipComputer : MonoBehaviour {
    public string Name;
    public Master Master;
    public FileSystem Directory = new FileSystem();
    public List<Firmware> ComponentFirmwares = new List<Firmware>();
    public ScriptEngine APIEngine = new ScriptEngine();

    void Start() {
        Master.AllShips.Add(this);

        InitEngine();
    }

    void InitEngine() {
        foreach (var firmware in ComponentFirmwares) {
            APIEngine.SetGlobalFunction(firmware.Name, firmware.Func);
        }
    }
}