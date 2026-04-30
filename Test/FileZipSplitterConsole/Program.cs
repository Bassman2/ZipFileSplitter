using System.CommandLine;

namespace FileZipSplitterConsole
{
    internal class Program
    {
        static int Main(string[] args)
        {
            RootCommand rootCommand = new("File Zip Splitter Console");

            Command zipSubCommand = new("zip", "Zip files or folders");
            zipSubCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            zipSubCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Files or folder to zip", AllowMultipleArgumentsPerToken = true, Required = true });
            rootCommand.Subcommands.Add(zipSubCommand);

            Command unzipCommand = new("unzip", "Unzip to folder");
            unzipCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files", Required = true });
            unzipCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            rootCommand.Subcommands.Add(unzipCommand);
                       

            rootCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                ///FileInfo parsedFile = parseResult.GetValue(fileOption);
                return zipper.Run();
            });

            ParseResult parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();
        }
    }
}
