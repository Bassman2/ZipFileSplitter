using System.CommandLine;

namespace ZipAnalyser;

internal class Program
{
    static int Main(string[] args)
    {
        RootCommand rootCommand = new("Zip File Analyser");
        var fileOption = new Option<FileInfo>("-f", "--file" ) { Description = "Zip File", Required = true };
        rootCommand.Add(fileOption);

        rootCommand.SetAction(parseResult =>
        {
            FileInfo file = parseResult.GetValue(fileOption)!;
            return Analyser.Analyse(file);
        });

        ParseResult parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
