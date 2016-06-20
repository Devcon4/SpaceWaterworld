using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using UnityEditor;
using UnityEngine.UI;
using Jurassic;
using Jurassic.Library;

public class ConsoleParser : MonoBehaviour {

    public ShipComputer ShipComputer;
    public Text ScreenText;
    public string CurrentLine;
    public List<Command> ConsoleCommands = new List<Command>();
    public Node CurrentNode;
    private bool _isActive = true;

    public delegate void CommandDelegate(Command command, string options);

    // Use this for initialization
    void Start () {

        if (ShipComputer.Directory.Root.Count > 0) {
            CurrentNode = ShipComputer.Directory.Root[0];
            Startup();
        } else {
            ConsoleStatus(false);
        }

        // *** api declaration ***
        ShipComputer.ComponentFirmwares.Add(new Firmware {
            Name = "log",
            HelpText = "Prints onto the screen",
            Usage = "log(\"I'm a new line!\");",
            Func = new Action<string>(AddConsoleLine)
        });

        ShipComputer.ComponentFirmwares.Add(new Firmware {
            Name = "error",
            HelpText = "Logs an error to the console",
            Usage = "--usage: error(\"DIR\", \"No directory called\"); --output: \"DIRECTORY ERROR: No directory called\"",
            Func = new Action<string,string>(Error)
        });

        // *** Command Declaration ***
        ConsoleCommands.Add(new Command {
            Aliases = new List<string> {"cls", "clear"},
            HelpText = "Clears the console",
            Func = new CommandDelegate(ClearConsole)
        });

        ConsoleCommands.Add(new Command{
            Aliases = new List<string> { "ls", "dir", "list", "directory"},
            Options = new List<string> { "-E", "--Expanded", "-V", "--Volumes" },
            HelpText = "Shows everything in the current directory",
            Func = new CommandDelegate(ShowDir)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "mkdir" },
            HelpText = "Creates a new directory",
            Func = new CommandDelegate(CreateDirectory)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "cd", "chdir" },
            HelpText = "Change directory",
            Func = new CommandDelegate(ChangeDirectory)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "run" },
            HelpText = "Runs a string of Javascript",
            Func = new CommandDelegate(RunJS)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "api" },
            HelpText = GetAPIHelpText(),
            Func = new CommandDelegate(ShowApiCommands)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "read" },
            HelpText = "Read contents of a file",
            Func = new CommandDelegate(ReadFile)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "help" },
            HelpText = GetMainHelpText(),
            Func = new CommandDelegate(HelpText)
        });
    }

    private void ConsoleStatus(bool status) {
        _isActive = status;
        ScreenText.text = "---NO DRIVE FOUND---";
    }

    void Startup() {
        AddConsoleLine("STARTING...");
        AddConsoleLine("---Galaxstar Ship Terminal---");
        NewConsoleLine();
    }

    string GetMainHelpText() {
        var text = "Usage: \"help [command Name]\"\n\tCommands:\n\t\t[Aliases] - [HelpText]";
        foreach (var consoleCommand in ConsoleCommands) {
            text += "\n\t -- [\"" + string.Join("\", \"", consoleCommand.Aliases.ToArray()) + "\"] - " + consoleCommand.HelpText;
        }
        return text;
    }

    string GetAPIHelpText() {
        var text = "Usage: \"api [api Name]\"\n\tAPI's:\n\t\t[Name] - [HelpText]";
        foreach (var firmware in ShipComputer.ComponentFirmwares) {
            text += "\n\t -- [\"" + firmware.Name + "\"] - " + firmware.HelpText;
        }
        return text;
    }

    void Error(string type, string options) {
        string log;
        switch (type) {
            case "ERROR":
                log = "ERROR: "; break;
            case "INVALID":
                log = "INVALID COMMAND: "; break;
            case "PARAMCOUNT":
                log = "PARAMATER COUNT ERROR: "; break;
            case "PARAMREQ":
                log = "PARAMATER REQUIRED: "; break;
            case "PARAMUNKNOWN":
                log = "UNKNOWN PARAMATER ERROR: "; break;
            case "DIR":
                log = "DIRECTORY ERROR: "; break;
            case "UPCOMING":
                log = "INCOMPLETE FEATURE: "; break;
            case "CHVOL":
                log = "CHANGE VOLUME ERROR: "; break;
            case "UNEXPECTED":
                log = "UNEXPECTED ERROR: "; break;
            default:
                log = "UNKNOWN ERROR: "; break;
        }
        AddConsoleLine(log + options);
    }

    public void AddConsoleLine(string options) {
        ScreenText.text += "\n\t" + options;
    }

    public void NewConsoleLine() {
        if (string.IsNullOrEmpty(CurrentNode.Path())) { Error("UNEXPECTED", "---RESTART TERMINAL---"); return; }
        ScreenText.text += "\n" + CurrentNode.Path() + "> ";
    }

    public void RunCommand(string currentLine) {
        var splitLine = currentLine.Split(null);
        string result = null;
        foreach (var command in ConsoleCommands) {
            result = command.Aliases.Find(x => x == splitLine[0]);
            if (result != null) {
                command.Func.DynamicInvoke(command, String.Join(" ", splitLine.Skip(1).ToArray()));
                break;
            }
        }

        if (result == null) { Error("INVALID", "No command called " + splitLine[0]); }
    }

    #region Command Functions

    // cls, clear.
    void ClearConsole(Command command = null, string options = null) {
        ScreenText.text = "";
    }
    
    // ls, dir.
    void ShowDir(Command command, string options) {
        if (options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        string print;

        if (!string.IsNullOrEmpty(options)) {
            if (command.Options.Exists(x => new[] {"-E", "--Expanded"}.Contains(x) && options.Contains(x))) {
                Error("UPCOMING", "The expanded view is not yet completed!");
                print = "Expanded directory view:";

            } else if (command.Options.Exists(x => new[] {"-V", "--Volumes"}.Contains(x))) {
                print = "Available Volumes:";
                foreach (var vol in ShipComputer.Directory.Root) {
                    print += "\n\t -" + vol.Name + ":/";
                }

            } else {
                Error("PARAMUNKNOWN", "Unknown paramater(s), valid paramaters: [\"" + string.Join("\", \"[", command.Options.ToArray()) + "\"]");
                return;
            }

        } else {
            if (CurrentNode.ChildrenNodes.Count < 1 && CurrentNode.Files.Count < 1) {
                print = "Current directory is empty";
                AddConsoleLine(print);
                return;
            }

            print = "Items inside the current directory:";
            foreach (var child in CurrentNode.ChildrenNodes.Concat(CurrentNode.Files.Cast<IFileable>())) {
                print += "\n\t -" + child.FormattedName();
            }
        }
        AddConsoleLine(print);
    }

    // cd, chdir.
    void ChangeDirectory(Command command, string options) {
        if (options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if (string.IsNullOrEmpty(options)) { Error("PARAMREQ", "CD needs a directory"); return; }
        var virtualDir = CurrentNode;
        var splitOptions = options.Split('/');

        if (options.Contains(":")) {
            splitOptions = options.Split(':');
            if(splitOptions.Length > 2) { Error("CHVOL", "More that one ':' in CD call"); return; }
            Volume result = ShipComputer.Directory.Root.FirstOrDefault(x => x.Name == splitOptions[0]);
            if (result == null) { Error("CHVOL", "No volume called " + splitOptions[0]); return; }
            virtualDir = result;
            if (virtualDir != CurrentNode) {
                CurrentNode = virtualDir;
                return;
            }
            Error("CHVOL", "No navigation occured");
            return;
        }

        foreach (var option in splitOptions) {
            if (option == "..") {
                if (virtualDir.GetType().Name == "Folder") {
                    virtualDir = ((Folder) virtualDir).Parent;
                    break;
                }
                Error("DIR", "No parent directory found");
                return;
            }

            if (virtualDir.ChildrenNodes.Exists(x => x.FormattedName() == option)) {
                var selectedDir = virtualDir.ChildrenNodes.Find(x => x.FormattedName() == option);
                if(selectedDir == null) {
                    Error("DIR", "No directory found at \"" + virtualDir.Path() + selectedDir.Path() + "\"");
                    return;
                }

                if (selectedDir.GetType().Name != "File") {
                    virtualDir = selectedDir as Node;
                } else {
                    Error("DIR", "\"" + selectedDir.FormattedName() + "\" Is not a directory.");
                    return;
                }
            }
        }
        if (virtualDir != CurrentNode) {
            CurrentNode = virtualDir;
            return;
        }
        Error("DIR", "No navigation occured");
    }

    // run.
    void RunJS(Command command, string options) {
        if (string.IsNullOrEmpty(options)) { Error("PARAMREQ", "RUN needs some javascript code to run"); return; }
        try {
            ShipComputer.APIEngine.Execute(options);
        } catch (JavaScriptException ex) {
            AddConsoleLine(ex.Message);
        }
    }

    // api.
    void ShowApiCommands(Command command, string options) {
        if (options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if (string.IsNullOrEmpty(options)) { AddConsoleLine(command.HelpText); return; }
        foreach (var firmware in ShipComputer.ComponentFirmwares) {
            if (firmware.Name == options) {
                AddConsoleLine(firmware.HelpText);
                break;
            }
        }
        Error("INVALID", "No command called " + options);
    }

    // read.
    void ReadFile(Command command, string options) {
        if(options.Any(char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if(string.IsNullOrEmpty(options)) { Error("PARAMREQ", "READ required a file to read"); return; }
        if(CurrentNode.Files.Count < 1) { Error("PARAMREQ", "No files found in directory"); return; }

        var file = CurrentNode.Files.FirstOrDefault(x => x.FormattedName() == options);
        if (file == null) { Error("PARAMREQ", "No file called " + options); return; }

        if (file.Content != null) {
            AddConsoleLine(file.Content);
            return;
        }

        if (file.ActualLocation != null) {
            Error("UPCOMING", "ActualLocation files have not been implemented!");
            return;
        }

        Error("UNEXPECTED", "Something went wrong!");
    }

    // help.
    void HelpText(Command command, string options) {
        if(options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if(string.IsNullOrEmpty(options)) { AddConsoleLine(command.HelpText); return; }
        string result = null;
        foreach (var consoleCommand in ConsoleCommands) {
            result = consoleCommand.Aliases.Find(x => x == options);
            if (result != null) {
                AddConsoleLine(consoleCommand.HelpText);
                break;
            }
        }
        if (result != null) return;
        Error("INVALID", "No command called " + options);
    }

    // mkdir.
    void CreateDirectory(Command command, string options) {
        if (options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if (string.IsNullOrEmpty(options)) { Error("PARAMCOUNT", "MKDIR needs a name"); return; }

        new Folder(FileManager.UniqueName(options, CurrentNode), CurrentNode);

    }

    #endregion

    // Todo: Should we have a cursor?
    void Update () {
        if (_isActive) {
            foreach (char c in Input.inputString) {
                // Backspace - Remove the last character
                if (c == "\b"[0]) {
                    if (CurrentLine.Length != 0) {
                        ScreenText.text = ScreenText.text.Substring(0, ScreenText.text.Length - 1);
                        CurrentLine = CurrentLine.Substring(0, CurrentLine.Length - 1);
                    }
                }
                // End of entry
                else if (c == "\n"[0] || c == "\r"[0]) {
                    // "\n" for Mac, "\r" for windows.
                    RunCommand(CurrentLine);
                    CurrentLine = "";
                    NewConsoleLine();
                }
                // Normal text input - just append to the end
                else {
                    ScreenText.text += c;
                    CurrentLine += c;
                }
            }
        }
    }
}