 
using System;
using System.Collections.Generic;
using System.Reflection;
 
using System.Linq;
using Microsoft.AspNetCore.Mvc;

public class MyApplicationModelController
{


    public MyApplicationModelController(IServiceProvider models)  
    {
    }


    /// <summary>
    /// Получение данных о контроллерах реализованных в данной сборке
    /// </summary>
    /// <returns> карта контроллеров </returns>
    public MyApplicationModel CreateModels()
        => CreateModels(Assembly.GetExecutingAssembly());

    public MyApplicationModel CreateModels(Assembly assembly)
    {
        var controllers = GetControllers(assembly);
        if (controllers == null || controllers.Count() == 0)
        {
            throw new Exception("Контроллеры не найдены в приложении");
        }


        MyApplicationModel controllersMap = new MyApplicationModel();
        foreach (Type controllerType in controllers)
        {
            if (controllerType.IsAbstract) continue;
            var model = CreateModel(controllerType);
            controllersMap.controllers[controllerType.Name] = model;
        }   
        return controllersMap;
    }

    private IEnumerable<Type> GetControllers(Assembly assembly)
        => assembly.GetTypes().Where(t => t.Name.EndsWith("Controller"));
        
        
    public static MyControllerModel CreateModel(Type controllerType)
    {
        var uri = "/";
        var attrs = GetAttrsForExtendedTypes(controllerType);
        if (attrs.ContainsKey("AreaAttribute")) uri += attrs["AreaAttribute"].ToString() + "/";
        if (attrs.ContainsKey("ForBusinessResourceAttribute")) uri += attrs["ForBusinessResourceAttribute"].ToString() + "/";
        string path = PathForController(controllerType);          
            
        MyControllerModel model = new MyControllerModel()
        {
            Name = controllerType.Name.Replace("`1", ""),
            Path = "/"+ controllerType.Name,
            Actions = new Dictionary<string, MyActionModel>()
        };
          
        foreach (MethodInfo method in controllerType.GetMethods())
        {
            if (typeof(Controller).GetMethods().Select(m => m.Name).Contains(method.Name)) continue;
              

                Dictionary<string, string> attributes =MyApplicationModel.GetAttrsForMethod(controllerType, method.Name);
                Dictionary<string, object> pars = new Dictionary<string, object>();
                model.Actions[method.Name] = new MyActionModel()
                {
                    Name = method.Name,
                    Attributes = attributes,
                    Method =
                            attributes.ContainsKey("HttpGet") ? "GET" :
                            attributes.ContainsKey("HttpGetAttribute") ? "GET" :
                            attributes.ContainsKey("HttpPost") ? "POST" :
                            attributes.ContainsKey("HttpPostAttribute") ? "POST" :
                            attributes.ContainsKey("HttpPut") ? "PUT" :
                            attributes.ContainsKey("HttpPutAttribute") ? "PUT" :
                            attributes.ContainsKey("HttpDelete") ? "DELETE" :
                            attributes.ContainsKey("HttpDeleteAttribute") ? "DELETE" :
                            "GET",
                    Parameters = new Dictionary<string, MyParameterDeclarationModel>(),
                    Path = model.Path + "/" + method.Name
                };
                foreach (ParameterInfo par in method.GetParameters())
                {
                    model.Actions[method.Name].Parameters[par.Name] = new MyParameterDeclarationModel()
                    {
                        Name = par.Name,
                        Type = par.ParameterType.Name,
                        IsOptional = par.IsOptional
                    };
                }              
        }      
        return model;
    }

    /// <summary>
    /// Возвращает все атрибуты типа и его базовых типов.
    /// </summary>
    private static Dictionary<string, string> GetAttrsForExtendedTypes(Type controllerType)
    {
        var res = new Dictionary<string, string>();
        Type p = controllerType;
        while (p.Name != "Controller" && p != null)
        {
            foreach(var kv in MyApplicationModel.GetAttrsForType(p))
            {
                res[kv.Key] = kv.Value;
            }
            
            p = p.BaseType;
        }
        return res;
       
    }

    /// <summary>
    /// Метод получения публичных методов типа
    /// </summary>
    /// <param name="type"> тип </param>
    /// <returns> открытые методы </returns>
    private string GetPublicMethods(Type type)
    {
        string actions = "";
        foreach (MethodInfo method in GetOwnPublicMethods(type))
        {
            string path = "/" + type.Name;
            actions += "\n\t" +
                method.Name + "(" + GetMethodParametersString(method) + "){ \n\t\t\t" +
                    "return this.http.get('" + path + "/" + method.Name + "',{params:" +
                        GetMethodParametersBlock(method) + "});\n\t\t\t}\n";
            /* new {
                name = method.Name,
                returns= method.ReturnType.Name,
                args = ReflectionService.GetMethodParameters(method),
                func = method.Name+"("+ ReflectionService.GetMethodParametersString(method) + 
                    "){ return this.http.get('" + path + "/"+method.Name+"',{params:"+
                        ReflectionService.GetMethodParametersBlock(method) + "});}",

            });*/
        }
        return actions;
    }


    [NonAction]
    public string GetMethodParametersBlock(MethodInfo method)
    {
        string s = "{";
        bool needTrim = false;
        foreach (var pair in GetMethodParameters(method))
        {
            needTrim = true;
            s += pair.Key + ':' + pair.Key + ",";
        }
        if (needTrim == true)
            return s.Substring(0, s.Length - 1) + "}";
        else
        {
            return s + "}";
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public Dictionary<string, object> GetMethodParameters(MethodInfo method)
    {


        Dictionary<string, object> args = new Dictionary<string, object>();
        foreach (ParameterInfo pinfo in method.GetParameters())
        {
            args[pinfo.Name] = new MyParameterDeclarationModel
            {
                Type = pinfo.ParameterType.Name,
                IsOptional = pinfo.IsOptional,
                Attributes = new Dictionary<string, string>(),
                Position = pinfo.Position,
                DefValue = pinfo.DefaultValue,
                Name = pinfo.Name                
            };
        }
        return args;
    }


    public string GetMethodParametersString(MethodInfo method)
    {
        bool needTrim = false;
        string s = "";
        foreach (var p in GetMethodParameters(method))
        {
            needTrim = true;
            s += p.Key + ",";// +":"+ p.Value + ",";
        }
        return needTrim == true ? s.Substring(0, s.Length - 1) : s;
    }

    public static MyControllerModel CreateModelOf(object model)
    {
        return MyApplicationModelController.CreateModel(model.GetType());            
    }

    /// <summary>
    /// <button>ok</button>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<MethodInfo> GetOwnPublicMethods(Type type)
    {
            var actions = (from m in new List<MethodInfo>(type.GetMethods())
                where m.IsPublic &&
                        !m.IsStatic &&
                        m.DeclaringType.Name == type.Name
                select m).ToList<MethodInfo>();
        return actions.Where(name => type.BaseType!=null&&type.BaseType.GetMethods().Select(m=>m.Name).ToList().Contains(name.Name) == false);
   
    }






    public static string BusinessResourceFor(Type type)
    {
        var attrs = MyApplicationModel.GetAttrsForType(type);
        if (attrs.ContainsKey("AreaAttribute"))
        {
            return attrs["AreaAttribute"];
        }
        else
        {
            return null;
        }
    }



    private static string PathForController(Type controllerType)
    {
       
        var attrs = GetAttrsForExtendedTypes(controllerType);
        string route =
            attrs.ContainsKey("RouteAttribute") ? attrs["RouteAttribute"] :
            attrs.ContainsKey("Route") ? attrs["Route"] :
            "";
        string path = route
                .Replace("[controller]", controllerType.Name.Replace("Controller", ""))
                .Replace("[action]", "Index");
        return path;
        /*string role = BusinessResourceFor(controllerType);
        if (role != null)
        {
            return "/" + role + "/" + controllerType.Name.Replace("Controller", "");
        }
        else
        {
            return "/" + controllerType.Name.Replace("Controller", "");
        } */
    }
}
 