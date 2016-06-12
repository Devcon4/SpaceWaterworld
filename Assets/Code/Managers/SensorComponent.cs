using System;
using UnityEngine;

public class SensorComponent : MonoBehaviour {
    public ShipComputer Computer;

    void Start() {
        Computer.ComponentFirmwares.Add(new Firmware {
            Name = "getOutput",
            HelpText = "Some test api call!",
            Func = new Func<string>(GetOutput)
        });
    }

    public string GetOutput() {
        return "I work!";
    }
}