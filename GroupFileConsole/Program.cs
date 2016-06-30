using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupFileConsole
{
    class Program
    {
        public static string PATH_ROOT_UPLOADED;
        private static char[] DELIMITER_CHARS = { ' ', '-', '_' };
        private static int indexGroup = 1;
        private static GroupFileModel cannotGroup = new GroupFileModel();

        static void Main(string[] args)
        {
            Console.Title = "Pixelz";
            Console.WriteLine("Skill test for Senior .NET Developer candidate");
            Console.WriteLine("Enter folder path rootupladed:");
            PATH_ROOT_UPLOADED = Console.ReadLine();

            List<GroupFileModel> groupFiles = ProcessDirectory(PATH_ROOT_UPLOADED);

            if (groupFiles.Count > 0)
            {
                for (int i = 0; i < groupFiles.Count; i++)
                {
                    Console.WriteLine("Group {0}", groupFiles[i].Group);
                    foreach(var file in groupFiles[i].Files)
                    {
                        Console.WriteLine("\t {0}", file.FullPath);
                    }
                }
            }

            if (cannotGroup.Files.Count > 0)
            {
                Console.WriteLine("Cannot Grouped");
                foreach (var file in cannotGroup.Files)
                {
                    Console.WriteLine("\t {0}", file.FullPath);
                }
            }

            Console.ReadLine();
        }

        public static List<GroupFileModel> ProcessDirectory(string pathfile)
        {
            List<GroupFileModel> group = new List<GroupFileModel>();
            try
            {
                foreach (string directory in Directory.GetDirectories(pathfile))
                {
                    group.AddRange(ProcessDirectory(directory));
                }

                List<FileModel> listFiles = new List<FileModel>();
                foreach (string file in Directory.GetFiles(pathfile))
                {
                    FileModel listFile = new FileModel();
                    listFile.FullPath = Path.GetFullPath(file);
                    listFile.SpliFileName = SplitFileName(Path.GetFileNameWithoutExtension(file));
                    listFiles.Add(listFile);
                }
                group.AddRange(GroupFile(listFiles));
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory Not Found!");
            }

            return group;
        }

        public static List<GroupFileModel> GroupFile(List<FileModel> listFiles)
        {
            List<GroupFileModel> group = new List<GroupFileModel>();

            var groupFiles = listFiles.OrderByDescending(d => d.SpliFileName.isGroupDigit).ThenBy(l => l.SpliFileName.link).ThenBy(p => p.SpliFileName.prefix)
                .GroupBy(g => new { g.SpliFileName.isGroupDigit, g.SpliFileName.link, g.SpliFileName.prefix })
                .ToList();

            foreach (var groupFile in groupFiles)
            {
                if (groupFile.Count() > 1)
                {
                    GroupFileModel groupFileModel = new GroupFileModel();
                    List<FileModel> Files = new List<FileModel>();
                    int numberGroup = (int)Math.Ceiling(((double)groupFile.Count() / 5));
                    int maxNumberValue = (int)Math.Ceiling(((double)groupFile.Count() / numberGroup));
                    int i = 0,k = 0;
                    foreach (FileModel file in groupFile)
                    {
                        i++;k++;
                        Files.Add(file);

                        if (i == maxNumberValue || k == groupFile.Count())
                        {
                            groupFileModel.Files = Files;
                            groupFileModel.Group = indexGroup++;
                            group.Add(groupFileModel);

                            if (numberGroup > 1)
                            {
                                groupFileModel = new GroupFileModel();
                                Files = new List<FileModel>();
                                numberGroup = (int)Math.Ceiling(((double)(groupFile.Count() - maxNumberValue) / 5)); ;
                                maxNumberValue = (int)Math.Ceiling(((double)(groupFile.Count() - maxNumberValue) / numberGroup));
                                i = 0;
                            }
                        }
                    }
                }
                else
                {
                    foreach (FileModel file in groupFile)
                    {
                        cannotGroup.Files.Add(file);
                    }
                }
            }

            return group;
        }

        public static SplitFileModel SplitFileName(string filename)
        {
            SplitFileModel splitFile = new SplitFileModel();

            int index = NumberInLastString(filename);
            if (index == -1)
            {
                splitFile.prefix = filename;
                splitFile.isGroupDigit = 0;
                splitFile.link = -1;

                if (filename.IndexOfAny(DELIMITER_CHARS) != -1)
                {
                    for (int i = 0; i < DELIMITER_CHARS.Length; i++)
                    {
                        string[] words = filename.Split(DELIMITER_CHARS[i]);
                        if (words.Length > 1)
                        {
                            if (words[words.Length - 1].Length <= 1)
                            {
                                splitFile.link = i;
                                splitFile.suffix = words[words.Length - 1];
                                splitFile.prefix = String.Join(DELIMITER_CHARS[i].ToString(), words.Take(words.Length - 1));
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                splitFile.suffix = filename.Substring(index);
                splitFile.isGroupDigit = 1;
                if (DELIMITER_CHARS.Contains(filename[index - 1]))
                {
                    splitFile.link = Array.IndexOf(DELIMITER_CHARS, filename[index - 1]);
                    splitFile.prefix = filename.Substring(0, index - 1);
                }
                else
                {
                    splitFile.link = -1;
                    splitFile.prefix = filename.Substring(0, index);
                }
            }

            return splitFile;
        }

        public static int NumberInLastString(string text)
        {
            int index = -1;
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (Char.IsDigit(text[i]))
                {
                    index = i;
                }
                else
                {
                    break;
                }
            }
            return index;
        }

    }
}
