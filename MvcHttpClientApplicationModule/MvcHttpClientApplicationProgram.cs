using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

public class MvcHttpClientApplicationProgram
{
    public static void Start(params string[] args)
    {        
        if (Environment.UserInteractive)
        {
            string url = null;
            //do
            //{
                url = "https://localhost:5001"; //Input("URL");
                Build(url);
            //}
            //while (String.IsNullOrWhiteSpace(url) == false);
        }
        else
        {
            foreach (string arg in args)
                Build(arg);
        } 
    }

    public static void Build(string url)
    {
        Console.WriteLine(@$"Build( ""{url}"" )");
        if (String.IsNullOrWhiteSpace(url))
            throw new ArgumentNullException("url");// file
        var builder = new MvcHttpClientApplicationBuilder();

        // url
        if (url.ToLower().StartsWith("http:") || url.ToLower().StartsWith("https:"))
        {

            Console.WriteLine(@$"""{url}"" is url )");

            var ctrl = new HttpClientController();
            MyApplicationModel model = ctrl.Get<MyApplicationModel>($"{url}/api").Result;
            builder.AddApplication(url, model, true);
        }

        // file
        else if( url[1]==':')
        {
            Console.WriteLine(@$"""{url}"" is file )");

            builder.AddApplication(url);
        } 
        else
        {
            throw new ArgumentException(url);
        }
        builder.Build();
    }

    private static string Input(string message)
    {
        Console.Write(message + ">");
        return Console.ReadLine();
    }
}
 
