using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Формирует код клиентского приложения для доступа к
/// функциям по сетевому протоколу.
/// </summary>
public class MvcHttpClientApplicationBuilder
{
    private ConcurrentDictionary<string, MyApplicationModel> Models 
        = new ConcurrentDictionary<string, MyApplicationModel>();

    /// <summary>
    /// Регистрация API HttpClient для Mvc-приложения
    /// </summary>
    /// <param name="Url">доступный URL</param>
    /// <param name="Model">модель приложения</param>
    /// <param name="Checkout">выполнить проверку</param>
    public void AddApplication(string Url, MyApplicationModel Model, bool Checkout)
    {
        Console.WriteLine($"Url: {Url}");
        Models[Url] = Model;
    }

    /// <summary>
    /// Регистрация API HttpClient для Mvc-приложения
    /// </summary>
    /// <param name="Url">доступный URL</param>
    /// <param name="Model">модель приложения</param>
    /// <param name="Checkout">выполнить проверку</param>
    public void AddApplication(string File)
    {
        Console.WriteLine($"File: {File}");
        if (System.IO.File.Exists(File) == false)
            throw new ArgumentException(File);
        var assembly = Assembly.LoadFile(File);
        var model = new MyApplicationModel();
    }

    public string Build()
    {
        var csp = new CompileApplicationSources();
        
        foreach (var model in Models)
        {
            foreach(var ctrl in model.Value.controllers)
            {
                string actions = "\n";
                foreach(var action in ctrl.Value.Actions)
                {
                    foreach (var par in action.Value.Parameters)
                    {
                        foreach (var attr in par.Value.Attributes)
                        {

                        }
                        switch (par.Value.Type)
                        {
                            default: throw new ArgumentException();
                        }
                    }
                }
                csp.AddClass(model.Value.Name,
                    $@"namespace {model.Value.Name}" + "{\n" +
                    "\t" + "public class " + ctrl.Value.Name + "{\n" +
                        actions +
                    "\t}" +
                    "}\n");                
               
            }

            
        }
        var compiled = csp.Exe();
        return Encoding.UTF8.GetString(compiled);
    }
}
