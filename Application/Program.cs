using System;
using Framework;
using System.IO;
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

            var dllFileNames = Directory.GetFiles(dllDirectoryPath, "*.dll");

            foreach (var dllFile in dllFileNames)
            {
                var dll = Assembly.LoadFile(dllFile);
                foreach (Type type in dll.GetExportedTypes())
                {
                    if (type.GetConstructor(Type.EmptyTypes) != null && typeof(IPlugin) == type.GetInterfaces()[0])
                    {
                        var obj = Activator.CreateInstance(type);
                        Console.WriteLine(type.GetProperties()[0].GetValue(obj));
                    }                            
                }                   
            }            
        }
    }
}
