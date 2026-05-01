using System.CommandLine;

namespace FileZipSplitterConsole
{
    internal class Program
    {
        static int Main(string[] args)
        {
            RootCommand rootCommand = new("File Zip Splitter Console");
                
            //VersionOption versionOption = new VersionOption("-v", "--version");
            //rootCommand.Options.Add(versionOption);

            Command zipCommand = new("zip", "Zip files or folders");
            zipCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            zipCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Files or folder to zip", AllowMultipleArgumentsPerToken = true, Required = true });
            rootCommand.Subcommands.Add(zipCommand);

            Command unzipCommand = new("unzip", "Unzip to folder");
            unzipCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files", Required = true });
            unzipCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            rootCommand.Subcommands.Add(unzipCommand);

            Command splitCommand = new("split", "Split one file to multiple");
            splitCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files", Required = true });
            splitCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            splitCommand.Add(new Option<SplitSizes>("-s", "--splitsize") { Description = "Split size", DefaultValueFactory = _ => SplitSizes.None });
            splitCommand.Add(new Option<long>("--split") { Description = "Split size in Bytes", DefaultValueFactory = _ => 0 });
            rootCommand.Subcommands.Add(splitCommand);

            Command mergeCommand = new("merge", "Merge multiple files to one");
            mergeCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files" });
            mergeCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files" });
            rootCommand.Subcommands.Add(mergeCommand);


            zipCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                return zipper.Zip();
            });

            unzipCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                return zipper.Unzip();
            });

            splitCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                return zipper.Split();
            });

            mergeCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                return zipper.Merge();
            });

            



            rootCommand.SetAction(parseResult =>
            {
                var action = parseResult.Action;

                FileZipper zipper = new();
                ///FileInfo parsedFile = parseResult.GetValue(fileOption);
                return zipper.Run();
            });

            ParseResult parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();
        }
    }
}
