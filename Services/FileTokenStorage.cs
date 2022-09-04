using System;
using System.ComponentModel;
using System.Threading.Tasks;


[Description("Контроллер предназначен для .")]
public class FileTokenStorage: ITokenStorage
{
    public static void Test()
    {
        var storage = new FileTokenStorage();
        Console.WriteLine(storage.Get());
        storage.Set(DateTime.Now.ToString());
        Console.WriteLine(storage.Get());
    }
    public FileTokenStorage( string file = "cookies.txt" )
    {
        if(System.IO.File.Exists(nameof(ITokenStorage))==false)
            System.IO.File.Create(nameof(ITokenStorage));
    }

    public string Get ()
    {
     
        return System.IO.File.ReadAllText(nameof(ITokenStorage));
    }
    public void Set(string value)
    {
        System.IO.File.WriteAllText(nameof(ITokenStorage), value);

    }
}
