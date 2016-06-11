using System;
using System.Collections.Generic;

public class Command {
    public List<string> Aliases;
    public List<string> Options = null;
    public string HelpText;
    public Delegate Func; 
}