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

                new File("copyright", "txt", docs, content: "PROPERTY OF GALAXSTAR INCORPERATED");

                Root = new List<Volume> { C };
                break;
        }
    }
}