using System.CommandLine;
using System.IO.Compression;

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
            zipCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Files or folder to zip", AllowMultipleArgumentsPerToken = true, Required = true });
            zipCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            zipCommand.Add(new Option<SplitSizes>("-s", "--splitsize") { Description = "Split size", DefaultValueFactory = _ => SplitSizes.G2 });
            zipCommand.Add(new Option<ExtentionFormat>("-e", "--extention") { Description = "Extension format", DefaultValueFactory = _ => ExtentionFormat.SingleExtention });
            rootCommand.Subcommands.Add(zipCommand);

            Command unzipCommand = new("unzip", "Unzip to folder");
            unzipCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files", Required = true });
            unzipCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            rootCommand.Subcommands.Add(unzipCommand);

            Command splitCommand = new("split", "Split one file to multiple");
            splitCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files", Required = true });
            splitCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            splitCommand.Add(new Option<SplitSizes>("-s", "--splitsize") { Description = "Split size", DefaultValueFactory = _ => SplitSizes.G2 });
            splitCommand.Add(new Option<ExtentionFormat>("-e", "--extention") { Description = "Extension format", DefaultValueFactory = _ => ExtentionFormat.SingleExtention });
            rootCommand.Subcommands.Add(splitCommand);

            Command mergeCommand = new("merge", "Merge multiple files to one");
            mergeCommand.Add(new Option<FileInfo>("-i", "--input") { Description = "Output folder for unzipped files", Required = true });
            mergeCommand.Add(new Option<FileInfo>("-o", "--output") { Description = "Output folder for unzipped files", Required = true });
            rootCommand.Subcommands.Add(mergeCommand);


            zipCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                string input = parseResult.CommandResult.GetValue<FileInfo>("-i")?.FullName ?? "";
                string output = parseResult.CommandResult.GetValue<FileInfo>("-o")?.FullName ?? "";
                SplitSizes splitSize = parseResult.CommandResult.GetValue<SplitSizes>("-s");
                long size = ((long)splitSize) * 1024;
                ExtentionFormat extentionFormat = parseResult.CommandResult.GetValue<ExtentionFormat>("-e");
                return zipper.Zip(input, output, size, extentionFormat);
            });

            unzipCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                string input = parseResult.CommandResult.GetValue<FileInfo>("-i")?.FullName ?? "";
                string output = parseResult.CommandResult.GetValue<FileInfo>("-o")?.FullName ?? "";
                return zipper.Unzip(input, output);
            });

            splitCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                string input = parseResult.CommandResult.GetValue<FileInfo>("-i")?.FullName ?? "";
                string output = parseResult.CommandResult.GetValue<FileInfo>("-o")?.FullName ?? "";
                SplitSizes splitSize = parseResult.CommandResult.GetValue<SplitSizes>("-s");
                long size = ((long)splitSize) * 1024;
                ExtentionFormat extentionFormat = parseResult.CommandResult.GetValue<ExtentionFormat>("-e");
                return zipper.Split(input, output, size, extentionFormat);
            });

            mergeCommand.SetAction(parseResult =>
            {
                FileZipper zipper = new();
                string input = parseResult.CommandResult.GetValue<FileInfo>("-i")?.FullName ?? "";
                string output = parseResult.CommandResult.GetValue<FileInfo>("-o")?.FullName ?? "";
                return zipper.Merge(input, output);
            });

            ParseResult parseResult = rootCommand.Parse(args);
            return parseResult.Invoke();
        }
    }
}
