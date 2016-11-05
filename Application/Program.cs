using System;
using Framework;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fullPath = AppDomain.CurrentDomain.BaseDirectory;
            var index = fullPath.IndexOf(typeof(Program).Namespace);
            var dllDirectoryPath = fullPath.Remove(index) + "DllDirectory\\";

            var fileNames = Directory.GetFiles(dllDirectoryPath);
            List<string> dllFileNames = new List<string>();
            foreach (var file in fileNames)
            {
                if (Path.GetExtension(file) == ".dll")
                    dllFileNames.Add(file);
            }

            foreach (var dllFile in dllFileNames)
            {
                var dll = Assembly.LoadFile(dllFile);
                foreach (Type type in dll.GetExportedTypes())
                {
                    if (Activator.CreateInstance(type) is IPlugin)
                        Console.WriteLine(type.Name);                                   
                }                   
            }            
        }
    }
}
