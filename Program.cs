using System.CommandLine;
class Program
{
    static void Main(string[] args)
    {
var bundleOption1=new Option<FileInfo>("--output","file path and name");
var bundleOption2=new Option<String>("--language","language of the file"){IsRequired=true};
var bundleOption3=new Option<Boolean>("--note","option to note the path file");
var bundleOption4=new Option<String>("--sort","the order of the files");
var bundleOption5=new Option<Boolean>("--remove-empty-lines","remove empty lines");
var bundleOption6=new Option<String>("--author","author of the file");

var bundleCommand=new Command("bundle","bundle the files");
bundleCommand.AddOption(bundleOption1);
bundleCommand.AddOption(bundleOption2);
bundleCommand.AddOption(bundleOption3);
bundleCommand.AddOption(bundleOption4);
bundleCommand.AddOption(bundleOption5);
bundleCommand.AddOption(bundleOption6);



bundleCommand.SetHandler((output,language,note,sort,removeEmptyLines,author) =>
{
    string[] filesToBundle;

    try{
        
        filesToBundle = GetFilesByLanguage(language);

        if(filesToBundle.Length==0){
            Console.WriteLine("no files found");
            return;
        }

        if (sort == "abc")
        {
            filesToBundle = filesToBundle.OrderBy(file => file).ToArray();
        }
        else if (sort == "desc")
        {
            filesToBundle = filesToBundle.OrderByDescending(file => file).ToArray();
        }
        else
        {
            Console.WriteLine("Invalid sort order specified. Using default (ABC).");
            filesToBundle = filesToBundle.OrderBy(file => file).ToArray();
        }
    
       
         using (var writer = new StreamWriter(output.FullName))
            {
            if(note){
                writer.WriteLine($"**** File: {Path.GetFileName(output.FullName)}");
                writer.WriteLine($"**** Path: {Path.GetFullPath(output.FullName)}");
                writer.WriteLine();
                
                Console.WriteLine("note was added");
            }    

            if (!string.IsNullOrEmpty(author))
            {
                writer.WriteLine($"**** Author: {author}");
                writer.WriteLine();
                writer.WriteLine();
                Console.WriteLine("author was added");
            }

            foreach (var file in filesToBundle)
            {
                var content = File.ReadAllText(file);

                if (removeEmptyLines)
                {
                    content = string.Join(Environment.NewLine, content
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                        .Where(line => !string.IsNullOrWhiteSpace(line)));
                        Console.WriteLine("empty lines were removed");
                }

                writer.WriteLine($"///// File: {Path.GetFileName(file)}");
                writer.WriteLine(content);
                writer.WriteLine();
            } 
             Console.WriteLine("file was created");
       
            }                   
        
    }
    catch(DirectoryNotFoundException ex){
            Console.WriteLine("your path is invalid!");
    }
   
},bundleOption1,bundleOption2,bundleOption3,bundleOption4,bundleOption5,bundleOption6);



var createRspCommand=new Command("create-rsp","Create an RSP file with user input");

createRspCommand.SetHandler(()=>
{

    try{
    Console.WriteLine("what is the name of the file?, and if you want enter the path");
    string output=Console.ReadLine();

    Console.WriteLine("enter the language of the files, if you want all enter 'all'");
    string language=Console.ReadLine();

    Console.WriteLine("do you want to note path and?");
    string note=Console.ReadLine();

    Console.WriteLine("enter the order you want to insert the files, default is 'abc'");
    string sort=Console.ReadLine();

    Console.WriteLine("do you want to remove empty lines?");
    string removeEmptyLines=Console.ReadLine();

    Console.WriteLine("enter the author of the file");
    string author=Console.ReadLine();


     var lines = new[]
        {
            $"bundle",
            $"--output \"{output}\"",
            $"--language \"{language}\"",
            $"--note \"{note}\"",
            $"--sort \"{sort}\"",
            $"--remove-empty-lines {removeEmptyLines}",
            $"--author \"{author}\""
        };
    
    File.WriteAllLines("options.rsp", lines);
    Console.WriteLine($"RSP file created ");

    }
    catch(Exception  ex)
    {
        Console.WriteLine(ex.Message);
    }
});


var rootCommand = new RootCommand("root command for file Bundler CLI");
rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);

rootCommand.InvokeAsync(args);
}


private static string[] GetFilesByLanguage(string language)
{
    string[] filesToBundle;

    if(language=="all"){
            filesToBundle = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories)
            .Where(file => !file.Contains(@"\bin\") && !file.Contains(@"\obj\") && !file.Contains(@"\publish\")).ToArray();

            Console.WriteLine("all files will be bundled");
        }
        else{

            string extension = language.ToLower() switch
            {
                 "c#" => "*.cs",
                 "javascript" => "*.js",
                 "python" => "*.py",
                 "java" => "*.java",
                 "ruby" => "*.rb",
                 "php" => "*.php",
                 "html" => "*.html",
                 "css" => "*.css",
                 "typescript" => "*.ts",
                 "kotlin" => "*.kt",
                 "rust" => "*.rs",
                 "c++" => "*.cpp",
                 "c" => "*.c",
                 "sql" => "*.sql",
                 "xml" => "*.xml",
                "json" => "*.json",
             _ => throw new ArgumentException("Unsupported language specified")
            };

            filesToBundle = Directory.GetFiles(".", extension, SearchOption.AllDirectories);

            Console.WriteLine("only {0} files will be bundled",language);
        }

    return filesToBundle;
}
}