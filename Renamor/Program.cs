using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Renamor
{
    class Program
    {
        ArgScanner scanner;
        string scanDirectory;
        string fileDirectory;
        string oldfile;
        string newfile;
        string oldfileOption;
        string newfileOption;
        bool helpMode;
        bool renameMode;
        bool pauseEnd;
        bool deleteWorkFiles;

        static void Main(string[] args)
        {
            Program p = new Program(args);
            p.Run();
        }
        public Program(string[] args)
        {
            scanner = new ArgScanner(args);
        }

        public void Run()
        {
            GetOptions();
            if (helpMode)
            {
                DoSomeHelp();
                return;
            } 
            else if (renameMode)
            {
                Rename();
            }
            else
            {
                Scan();
            }

            if (pauseEnd)
            {
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }

           
        }

        void GetOptions()
        {
            renameMode = scanner.HasOption("r");
            helpMode = scanner.HasOption("?") || scanner.HasOption("help");
            scanDirectory = scanner.FirstOrDefault("d", ".");
            oldfileOption = scanner.FirstOrDefault("o", "oldfiles.txt");
            newfileOption = scanner.FirstOrDefault("n", "newfiles.txt");

            fileDirectory = scanner.FirstOrDefault("f", ".");
            if (scanner.HasOption("f"))
            {
                oldfile = Path.Combine(fileDirectory, oldfileOption);
                newfile = Path.Combine(fileDirectory, newfileOption);
            }
            else
            {
                oldfile = oldfileOption;
                newfile = newfileOption;
            }

            pauseEnd = scanner.HasOption("p");
            deleteWorkFiles = scanner.HasOption("x");
        
        }


        void Scan()
        {
            DirectoryInfo d = new DirectoryInfo(scanDirectory);
            StringBuilder outtext = new StringBuilder();
            foreach (FileInfo f in d.EnumerateFiles())
            {
                outtext.Append(f.FullName + "\r\n");
            }
            File.WriteAllText(oldfile, outtext.ToString());
            File.WriteAllText(newfile, outtext.ToString());

            System.Console.WriteLine("Files scanned");
        }

        void Rename()
        {
            if (!new FileInfo(oldfile).Exists)
            {
                Console.WriteLine("Could not file original names file");
                Console.WriteLine("--> " + oldfile);
                return;
            }
            if (!new FileInfo(newfile).Exists)
            {
                Console.WriteLine("Could not file new names file");
                Console.WriteLine("--> " + newfile);
                return;
            }


            using (var olds = new FileStream(oldfile, FileMode.Open))
            using (var news = new FileStream(newfile, FileMode.Open))
            using (var oldstream = new StreamReader(olds))
            using (var newstream = new StreamReader(news))
            {
                while (!oldstream.EndOfStream || !newstream.EndOfStream)
                {
                    String nextOld = oldstream.ReadLine();
                    String nextNew = newstream.ReadLine();
                    if (nextOld != null && nextNew != null && !nextNew.StartsWith("#"))
                    {
                        Console.WriteLine(nextOld);
                        Console.WriteLine("--> " + nextNew);
                            
                        FileInfo file = new FileInfo(nextOld);
                        file.MoveTo(nextNew);
                    }
                }
            }
        }

        static void DoSomeHelp()
        {
            Console.WriteLine("File Rename Utility (Renamor)");
            Console.WriteLine("-----------------------------");
            Console.WriteLine();
            Console.WriteLine("This utility has two modes, scan mode and rename mode.");
            Console.WriteLine("In scan mode, the utility scans a chosen directory and creates two " +
                "files with the names of all the files in the folder.");
            Console.WriteLine("In rename mode, the utility reads both files one line at a time, " +
                "and renames the the input from the old file to the");
            Console.WriteLine("names in the new file.");
            Console.WriteLine("Lines starting with # in the new file are ignored.");
            Console.WriteLine();
            Console.WriteLine("Options");
            Console.WriteLine("-------");


            Console.WriteLine("(Default) - scan mode");
            Console.WriteLine("/r - rename mode");

            Console.WriteLine("/d [directory] - Directory to scan");

            Console.WriteLine("/f [directory] - Work text file directory");


            Console.WriteLine("/o [filename] - Target files to rename work file");
            Console.WriteLine("  Default: \"{work directory}\\oldfiles.txt\"");

            Console.WriteLine("/n [filename] - New filenames work file");
            Console.WriteLine("  Default: \"{work directory}\\newfiles.txt\"");

            Console.WriteLine("/x - Delete work files after complete");

            Console.WriteLine("/p - pause for input when finished");
        }
    }
}
