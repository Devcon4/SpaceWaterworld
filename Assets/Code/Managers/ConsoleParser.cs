using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using UnityEditor;
using UnityEngine.UI;

public class ConsoleParser : MonoBehaviour {

    public ShipComputer ShipComputer;
    public Text ScreenText;
    public string CurrentLine;
    public List<Command> ConsoleCommands = new List<Command>();
    public Node CurrentNode;

    public delegate void CommandDelegate(Command command, string options);

    // Use this for initialization
    void Start () {
        CurrentNode = ShipComputer.Directory.Root[0];
        // Commands.
        ConsoleCommands.Add(new Command {
            Aliases = new List<string> {"cls", "clear"},
            HelpText = "Clears the console",
            Func = new CommandDelegate(ClearConsole)
        });

        ConsoleCommands.Add(new Command{
            Aliases = new List<string> { "ls", "dir", "list", "directory"},
            Options = new List<string> { "E", "Expanded" },
            HelpText = "Shows everything in the current directory",
            Func = new CommandDelegate(ShowDir)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "mkdir"},
            HelpText = "Creates a new directory",
            Func = new CommandDelegate(CreateDirectory)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "cd", "chdir" },
            HelpText = "Change directory",
            Func = new CommandDelegate(ChangeDirectory)
        });

        ConsoleCommands.Add(new Command {
            Aliases = new List<string> { "help" },
            HelpText = GetMainHelpText(),
            Func = new CommandDelegate(HelpText)
        });

        Startup();
    }

    void Startup() {
        ScreenText.text = "STARTING... \n---Galaxstar Ship Terminal---\n" + CurrentNode.Path() + "> ";
    }

    string GetMainHelpText() {
        var text = "Usage: \"help [command Name]\"\n\tCommands:\n\t\t[Aliases] - [HelpText]";
        foreach (var consoleCommand in ConsoleCommands) {
            text += "\n\t -- [\"" + string.Join("\", \"", consoleCommand.Aliases.ToArray()) + "\"] - " + consoleCommand.HelpText;
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
            case "PARAMUNKNOWN":
                log = "UNKNOWN PARAMATER ERROR: "; break;
            case "DIR":
                log = "DIRECTORY ERROR: "; break;
            case "UPCOMING":
                log = "INCOMPLETE FEATURE: "; break;
            default:
                log = "UNKNOWN ERROR: "; break;
        }
        AddConsoleLine(log + options);
    }

    void AddConsoleLine(string options) {
        ScreenText.text += "\n\t" + options;
    }

    void UpdateDirectory() {
    }

    void NewConsoleLine() {
        ScreenText.text += "\n" + CurrentNode.Path() + "> ";
    }

    // cls, clear.
    void ClearConsole(Command command = null, string options = null) {
        ScreenText.text = "";
    }
    
    // ls, dir.
    void ShowDir(Command command, string options) {
        if (options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if (!string.IsNullOrEmpty(options) && !command.Options.Contains(options)) { Error("PARAMUNKNOWN", "Unknown paramater(s), valid paramaters: [\"" + string.Join("\", \"[", command.Options.ToArray()) + "\"]"); }
        string print;
        if (command.Options.Exists(x => new [] { "-E", "--Expanded" }.Contains(x))) {
            Error("UPCOMING", "The expanded view is not yet completed!");
            print = "Expanded directory view:";
        } else {
            if (CurrentNode.ChildrenNodes.Count < 1) {
                print = "Current directory is empty";
                AddConsoleLine(print);
                return;
            }

            print = "Items inside the current directory:";
            foreach (var child in CurrentNode.ChildrenNodes) {
                print += "\n\t -" + child.FormattedName();
            }
        }
        AddConsoleLine(print);
    }

    // cd, chdir.
    void ChangeDirectory(Command command, string options) {
        if (options.Any(Char.IsWhiteSpace)) { Error("PARAMCOUNT", "Wrong amount of paramaters"); return; }
        if (string.IsNullOrEmpty(options)) { Error("PARAMCOUNT", "CD needs a directory"); return; }

        var splitOptions = options.Split('/');
        var virtualDir = CurrentNode;

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

    // Update is called once per frame
    void Update () {

        foreach (char c in Input.inputString) {
            // Backspace - Remove the last character
            if (c == "\b"[0]) {
                if (CurrentLine.Length != 0) {
                    ScreenText.text = ScreenText.text.Substring(0, ScreenText.text.Length - 1);
                    CurrentLine = CurrentLine.Substring(0, CurrentLine.Length - 1);
                }
            }
            // End of entry
            else if (c == "\n"[0] || c == "\r"[0]) {// "\n" for Mac, "\r" for windows.
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