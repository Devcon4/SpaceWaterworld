using System.Collections.Generic;
using Jurassic;
using UnityEngine;

public class ShipComputer : MonoBehaviour {
    public string Name;
    public FileSystem Directory = new FileSystem("GALAXSTARCONSOLE");
    public List<Firmware> ComponentFirmwares = new List<Firmware>();
    public ScriptEngine APIEngine = new ScriptEngine();

    void Start() {
        InitEngine();
    }

    void InitEngine() {
        foreach (var firmware in ComponentFirmwares) {
            APIEngine.SetGlobalFunction(firmware.Name, firmware.Func);
        }
    }
}