using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HardriveComponent : MonoBehaviour {

    public ShipComputer Computer;
    public string DefaultSoftwareType;

    void Start() {
        Computer.Directory.Root.Add(DefaultVolumes(DefaultSoftwareType));
    }

    public Volume DefaultVolumes(string type) {
        var vol = new Volume(FileManager.UniqueVolumeName("C", Computer.Directory.Root));
        switch (type) {
            case "GALAXSTARCONSOLE":

                vol = new Volume(FileManager.UniqueVolumeName("C", Computer.Directory.Root));

                new Folder("bin", vol);
                var files = new Folder("files", vol);

                new Folder("executables", files);
                var docs = new Folder("documents", files);

                new File("copyright", "txt", docs, content: "\n---PROPERTY OF GALAXSTAR INCORPERATED---\n");

                return vol;
            case "SCIENCE_1":
                vol = new Volume(FileManager.UniqueVolumeName("H", Computer.Directory.Root));
                var logs = new Folder("logs", vol);
                new Folder("assignments", vol);
                new Folder("experiments", vol);

                new File("Log-2543", "txt", logs, content: "\n---Log-2543---\n\tWe have seen odd behavior with subject 23. \n\tFurther study is required. \n--Scientist-12\n");
                new File("Log-3215", "txt", logs, content: "\n---Log-3215---\n\tSubject 23 has shown aggressive behavior.\n\tTransfer subject 23 to detainment.\n--Scientist-12\n");
                new File("Log-4563", "txt", logs, content: "\n---Log-4563---\n\tArrival of subject 23 to cell 8.\n\t--Guard-5\n");
                new File("Log-5643", "txt", logs, content: "\n---Log-5643---\n\tInmate 8 refuses to co-operate.\n\tAdvanced --- Techniques --- must be implemented.\n--Guard-5\n");
                new File("Log-6421", "txt", logs, content: "\n---Log-6421---\n\tInmate 8 has disapeared over night.\n\tRounds have been scheduled to locate this nuisance.\n--Guard-5\n");
                new File("Log-7982", "txt", logs, content: "\n---Log-7982---\n\tThe terror attacked lab 12 today.\n\tAll of those people.\n\tPlease find him.\n--Scientist-12\n");

                return vol;
            default:
                return vol;
        }
    }
}
