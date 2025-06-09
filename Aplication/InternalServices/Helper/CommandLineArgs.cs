namespace Aplication.InternalServices.Helper;

public class CommandLineArgs
{
    public string[] Args { get; }

    public CommandLineArgs(string[] args)
    {
        Args = args;
    }

    public bool Contains(string arg) => Args.Contains(arg);
}