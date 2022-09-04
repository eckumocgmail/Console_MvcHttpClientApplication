using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public interface IApplicationModelBuilder
{

}

public interface IControllerModelBuilder
{

}

public interface IActionModelBuilder
{

}

public interface IParameterModelBuilder
{

}

public interface INavigationBuilder
{

}
 

/// <summary>
/// Коллекция сетевых сервисов
/// </summary>
//[Label("Модель приложения")]
public class MyApplicationModel
{ 
    
    public string Name { get; set; }    
    public string Version { get; set; }
    public string Url { get; set; }
    public string Auth { get; set; }



    public SortedDictionary<string, MyControllerModel> controllers { get; } = new SortedDictionary<string, MyControllerModel>();
    
 

    /// <summary>
    /// Получение сведений о правах доступа определённых атрибутами
    /// </summary> 
    public static string BusinessResourceFor(Type type)
    {
        var attrs = GetAttrsForType(type);
        if (attrs.ContainsKey("ForBusinessResourceAttribute"))
        {
            return attrs["ForBusinessResourceAttribute"];
        }
        else
        {
            return null;
        }
    }
    public static Dictionary<string, string> GetAttrsForType(Type p)
    {

        Dictionary<string, string> attrs = new Dictionary<string, string>();
        if (p == null)
        {
            Console.WriteLine($"Вам слендует передать ссылку на Type в метод Utils.GetEntityContrainsts() вместо null");
            Console.WriteLine($"{new ArgumentNullException("p")}");
            return attrs;
        }
        foreach (var data in p.GetCustomAttributesData())
        {
            string key = data.AttributeType.Name;
            foreach (var arg in data.ConstructorArguments)
            {
                string value = arg.Value.ToString();
                attrs[key] = value;
            }

        }
        return attrs;
    }

    Assembly TargetAssembly;

    public MyApplicationModel()
    {
    
    }

    public static MyApplicationModel GetExecutingModel()
        => GetModel(Assembly.GetExecutingAssembly());

    public static IEnumerable<Type> GetControllers(Assembly assembly)
    {
        return (assembly).GetTypes().Where(type => type.Name.EndsWith("Controller"));
    }

    public static MyApplicationModel GetModel(Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException("assembly");
        var result = new MyApplicationModel();
        result.Name = assembly.GetName().Name;
        result.Version = assembly.GetName().Version.Build.ToString();
        result.Url = "https://localhost:5001";
        result.Auth = "Basic";
        foreach (var type in (assembly).GetTypes().Where(type => type.Name.EndsWith("Controller")))
        {
            try
            {
                result.controllers[type.Name] = result.CreateModel(type);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка при формировании модели контролера "+type.Name + "\n" + ex.Message + "\n" + ex.ToString());
            }
        }
        return result;
    }




    /// <summary>
    /// Путь кконтроллеру
    /// </summary>
    public string PathForController(Type controllerType)
    {
        string role = BusinessResourceFor(controllerType);
        if (role != null)
        {
            return "/" + role + "/" + controllerType.Name.Replace("Controller", "");
        }
        else
        {
            return "/" + controllerType.Name.Replace("Controller", "");
        }
    }

   
    public static Dictionary<string, string> GetAttrsForProperty(Type type, String property)
    {
        if (type == null)
        {
            throw new ArgumentNullException();
        }


        Dictionary<string, string> attrs = null;
        PropertyInfo info = null;

        try
        {
            attrs = new Dictionary<string, string>();
            info = type.GetProperty(property);
        }
        catch (AmbiguousMatchException ex)
        {
            Console.WriteLine(ex.Message);
        }

        if (info == null)
        {
            throw new Exception($"Свойство {property} не найдено в обьекте типа {type.Name}");
        }
        var datas = info.GetCustomAttributesData();
        if (datas != null)
            foreach (var data in datas)
            {

                string key = data.AttributeType.Name;
                //ParameterInfo[] pars = data.AttributeType.GetConstructors()[0].GetParameters();
                if (data.ConstructorArguments == null || data.ConstructorArguments.Count == 0)
                {
                    attrs[key] = "";
                }
                else
                {
                    foreach (var arg in data.ConstructorArguments)
                    {

                        string value = arg.Value == null ? "" : arg.Value.ToString();
                        attrs[key] = value;
                    }
                }

                //model.Attributes[data.AttributeType] = null;

            }

        if (attrs == null)
        {
            throw new Exception($"Не удалось получить атрибуты свойсва {property} класса {type.Name}");
        }
        return attrs;
    }


    /// <summary>
    /// JNrhsnst vtnjls
    /// </summary>
    public List<MethodInfo> GetOwnPublicMethods(Type type)
    {
        return (from m in new List<MethodInfo>(type.GetMethods())
                where m.IsPublic &&
                        !m.IsStatic &&
                        m.DeclaringType.FullName == type.FullName
                select m).ToList<MethodInfo>();
    }

    internal static Type GetTypeForName(string typeName)
    {
        throw new NotImplementedException();
    }

    public static Dictionary<string, string> GetAttrsForMethod(Type controllerType, string name)
    {
        try
        {
            Dictionary<string, string> attrs = new Dictionary<string, string>();
            var method = controllerType.GetMethods().Where(m => m.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (method == null)
                throw new ArgumentException("Не правильно задан аргемент: name=" + name);
            foreach (var data in method.GetCustomAttributesData())
            {
                string key = data.AttributeType.Name;
                foreach (var arg in data.ConstructorArguments)
                {
                    string value = arg.Value.ToString();
                    attrs[key] = value;
                }

            }
            return attrs;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new Exception("Не удалось получить атрибуты для метода " + GetNameOfType(controllerType) + "." + name + "");
        }
    }

    public static string GetNameOfType(Type propertyType)
    {
        if (propertyType == null)
            throw new ArgumentNullException("type");
        string name = propertyType.Name;
        if (name == null) return "";
        if (name.IndexOf("`") != -1)
            name = name.Substring(0, name.IndexOf("`"));

        var arr = propertyType.GetGenericArguments();
        if (arr.Length > 0)
        {
            name += '<';
            foreach (var arg in arr)
            {
                name += GetNameOfType(arg) + ",";
            }
            name = name.Substring(0, name.Length - 1);
            name += '>';
        }
        return name;
    }

    /// <summary>
    /// Создание модели контроллера по тип
    /// </summary>
    public MyControllerModel CreateModel(Type controllerType)
    {
        var uri = "/";
        var attrs = MyApplicationModel.GetAttrsForType(controllerType);
        if (attrs.ContainsKey("AreaAttribute")) uri += attrs["AreaAttribute"].ToString() + "/";
        if (attrs.ContainsKey("ForBusinessResourceAttribute")) uri += attrs["ForBusinessResourceAttribute"].ToString() + "/";
        string path = PathForController(controllerType);
        Console.WriteLine(path);
        MyControllerModel model = new MyControllerModel()
        {
            Name = controllerType.Name.Replace("`1", ""),
            Path = path,
            Actions = new Dictionary<string, MyActionModel>()
        };

        

        while (controllerType != null)
        {
            foreach (MethodInfo method in GetOwnPublicMethods(controllerType))
            {
                if (method.IsPublic && method.Name.StartsWith("get_") == false && method.Name.StartsWith("set_") == false)
                {

                    if (typeof(Controller).GetMethods().Select(m => m.Name).Contains(method.Name))
                        continue;
                    Dictionary<string, string> attributes = GetAttrsForMethod(controllerType, method.Name);
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
            }
            controllerType = controllerType.BaseType;
        }
        return model;
    }


    /// <summary>
    /// 
    /// </summary>
    public void AddAction(
        string function,
        MyControllerModel myControllerModel)
    {
        throw new NotImplementedException();
    }

    public void AddAction(MyControllerModel myControllerModel)
    {
        throw new NotImplementedException();
    }
}

