using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Infotecs1
{
    class Program
    {
        // Функция проверки доступа к папке
        static private bool hasWriteAccessToFolder(string folderPath)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        //Функция проверки доступа к файлам
        private static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDirPath = dirs.Pop();
                if (searchOption == SearchOption.AllDirectories)
                {
                    try
                    {
                        string[] subDirs = Directory.GetDirectories(currentDirPath);
                        foreach (string subDirPath in subDirs)
                        {
                            dirs.Push(subDirPath);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDirPath, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

                foreach (string filePath in files)
                {
                    yield return filePath;
                }
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать!");
            try
            {
                var file = File.ReadAllText("../../config.json");
                {
                    models json = Newtonsoft.Json.JsonConvert.DeserializeObject<models>(file);
                    var target_path = json.Target;
                    var source_path_multie = json.Source;
                    if (Directory.Exists(target_path) && hasWriteAccessToFolder(target_path))
                    {
                        string directory_name = System.IO.Path.Combine(target_path, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ssfff"));
                        Console.WriteLine(directory_name);
                        System.IO.Directory.CreateDirectory(directory_name);
                        foreach(string source_path in source_path_multie) 
                        { 
                        if (Directory.Exists(source_path) && hasWriteAccessToFolder(source_path))
                        {
                            var f = SafeEnumerateFiles(source_path,"*", SearchOption.AllDirectories);
                            foreach (string i in f)
                            {
                 
                                var x = i.Replace(source_path, directory_name);
                                try
                                {
                                    File.Copy(i, x, true);
                                }
                                catch (Exception e)
                                {
                                    string newpath = x.Replace(i.Split('\\').Last(),"");
                                    Directory.CreateDirectory(newpath);

                                    try 
                                    { 
                                        File.Copy(i, i.Replace(source_path, directory_name), true);
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }

                        else
                        {

                            string msg = $@"{DateTime.Now.ToString("dd MM yyyy|HH:mm:ssfff")}" +
                                "\nИзвините, возникли проблемы!\n" +
                                "Исходной директории не найдено или вы не имеете прав доступа.\n" +
                                "Данные не могу быть скопированы.\nПроверьте исходные данные...\n" +
                                $@"Попытка скопировать данные из {source_path}";
                            Console.WriteLine(msg);
                            //создать/открыть если есть и записать текст 
                            System.IO.File.WriteAllText($@"{directory_name}\\errorlog.txt",msg);

                        }
                    }
                        Console.WriteLine("Готово!");
                    }
                    else
                    {
                        Console.WriteLine("Извините,возникли проблемы!\n" +
                            "Целевая директория не существует или вы не имеете прав доступа.\n" +
                            "Проверьте исходные данные...\n" +
                            $@"Попытка создать папку в {target_path}");
                    }
                }
            }
            catch (IOException e)
            {
          
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}
