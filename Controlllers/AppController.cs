

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


public class AppController: MyApplicationModelController
{
    public AppController(IServiceProvider models) : base(models)
    {
    }

    public Dictionary<string, string> GetWebApi()
    {
        Dictionary<string, string> api = new Dictionary<string, string>();
        foreach (var p in this.CreateModels().controllers)
        {
            try
            {
                api[p.Key] = this.GenerateAngularService(p.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                continue;
            }
        }
        return api;
    }



    public Dictionary<string, string> GetJsApi()
    {
        Dictionary<string, string> api = new Dictionary<string, string>();
        var controllers = MyApplicationModel.GetControllers(Assembly.GetExecutingAssembly());
        if (controllers == null || controllers.Count() == 0)
        {
            throw new Exception("Контроллеры не найдены в приложении");
        }           
        foreach (Type controllerType in controllers)
        {
            if (controllerType.IsAbstract) continue;
            try
            {
                api[MyApplicationModel.GetNameOfType(controllerType)] = GenerateAngularJsService(controllerType);
            }catch (Exception ex)
            {
                ex.ToString();
            }
        } 
        return api;
    }





    /// <summary>
    /// Выполнение генерации сервиса, связанного с методами контроллера
    /// </summary>
    /// <param name="ServiceControllerModel"> модель операций контроллера </param> 
    /// <returns></returns>
    public string GenerateAngularService(string typeName)
    {
        Type type = MyApplicationModel.GetTypeForName(typeName);
        return this.GenerateAngularService(type);
    }
    public string GenerateAngularService(Type type)
    {
        var model = CreateModel(type);
        return GenerateAngularService(model);
    }



    public string GenerateAngularService(MyControllerModel controllerModel )
    {
        string typeScript = "import { HttpClient,HttpParams } from '@angular/common/http';\n";            
        typeScript += "import { Injectable } from '@angular/core';\n\n";
        typeScript += "@Injectable({ providedIn: 'root' })\n";
        typeScript += $"export class {controllerModel.Name}" + "\n{\n\n";
        typeScript += $"\tconstructor( private http: HttpClient )" + "{}\n\n";
        foreach( var actionKV in controllerModel.Actions)
        {
            MyActionModel actionModel = actionKV.Value;
            string tsName = Naming.ToCamelStyle(actionModel.Name);
            string tsParamDeclaration = "";
            string tsParamMap = "{\n";
            foreach ( var key in actionModel.Parameters.Keys)
            {
                tsParamDeclaration += key + ":any,";
                tsParamMap += $"\t\t\t{key}: {key},\n";
            }
            if (tsParamMap.EndsWith(",\n"))
            {
                tsParamMap = tsParamMap.Substring(0, tsParamMap.Length - 2);
            }
            tsParamMap += "\n\t\t} ";
            if (tsParamDeclaration.EndsWith(","))
            {
                tsParamDeclaration = tsParamDeclaration.Substring(0, tsParamDeclaration.Length - 1);
            }
            typeScript += $"\tpublic {tsName}( {tsParamDeclaration} )" + "{\n";
            typeScript += $"\t\tlet pars = this.toHttpParams({tsParamMap});\n";
            typeScript += $"\t\treturn this.http.get('{actionModel.Path}',pars);\n";
            typeScript += "\t}\n\n";
        }
        typeScript +=
        "\n\ttoHttpParams(obj: any): {[property: string]: string} "+
        "\n\t{ " +
        "\n\t     const result: {[property: string]: string} = { }; " +
        "\n\t     Object.getOwnPropertyNames(obj).forEach(name => { " +
        "\n\t         result[name] = JSON.stringify(obj[name]); " +
        "\n\t     }); " +
        "\n\t     return result; " +
        "\n\t}           \n";
        typeScript += "}\n";
        return typeScript;         
    }






    /// <summary>
    /// Создание кода контроллера для AngularJS
    /// </summary>
    /// <param name="controllerModel"></param>
    /// <returns></returns>
   
    public string GenerateAngularJsService(Type type)
    {
        var model = CreateModel(type);
        return AngularJsService(model);
    }

    public static bool HasBaseType(Type targetType, string baseType)
    {
        if (targetType == null)
            throw new Exception("Тип не определён");
        Type p = targetType.BaseType;
        while (p != typeof(Object) && p != null)
        {
            if (p.Name == baseType)
            {
                return true;
            }
            p = p.BaseType;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="controllerModel"></param>
    /// <returns></returns>
    public string AngularJsService(MyControllerModel controllerModel)
    {
        string typeScript = "function "+Naming.ToCapitalStyle(controllerModel.Name ).Replace("Controller","Factory")+"( pelement ){";
        typeScript += "\n\n\t\t\tconsole.log(pelement.id,'"+ Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "');\n\n" +
        "\n\n\t\t\tfunction toHttpParams(obj: any): {[property: string]: string} " +
        "\n\t\t\t{ " +
        "\n\t\t\t     const result: {[property: string]: string} = { }; " +
        "\n\t\t\t     Object.getOwnPropertyNames(obj).forEach(name => { " +
        "\n\t\t\t         result[name] = window['convertToHttpMessageParam'](obj[name]); " +
        "\n\t\t\t     }); " +
        "\n\t\t\t     return result; " +
        "\n\t\t\t}           \n\n\n";
        foreach (var actionKV in controllerModel.Actions)
        {
            MyActionModel actionModel = actionKV.Value;
            string tsName = Naming.ToCamelStyle(actionModel.Name);
            string tsParamDeclaration = "";
            string tsParamMap = "{\n";
            foreach (var key in actionModel.Parameters.Keys)
            {
                tsParamDeclaration += key + ",";
                tsParamMap += $"\t\t\t\t{key}: {key},\n";
            }
            if (tsParamMap.EndsWith(",\n"))
            {
                tsParamMap = tsParamMap.Substring(0, tsParamMap.Length - 2);
            }
            tsParamMap += "\n\t\t\t\t} ";
            if (tsParamDeclaration.EndsWith(","))
            {
                tsParamDeclaration = tsParamDeclaration.Substring(0, tsParamDeclaration.Length - 1);
            }
            typeScript += $"\t\t\tthis.{tsName}=function( {tsParamDeclaration} )" + "{\n";
            typeScript += $"\t\t\t\tlet pars = toHttpParams({tsParamMap});\n";
            typeScript += $"\t\t\t\tconsole.log('{tsName}', pars);\n ";
            typeScript += "\t\t\t\treturn window['https']({ url: '" + actionModel.Path+ "', type: '"+ Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "', params: pars, headers: { 'Content-Type': 'text/json', Scope: angular.element(pelement).scope().$id, Model: pelement?pelement.id.replace('view-',''): '' }, method: '" + actionModel.Method+"',}).then((r)=>{ window['checkout'](); return r; });\n";
            typeScript += "\t\t\t}\n\n\n";
        }
        typeScript+="}\n\n\n";





        typeScript += "@Controller({ name: '"+Naming.ToCapitalStyle(controllerModel.Name )+ "' })\n";
        typeScript += $"class {controllerModel.Name}" + "\n{\n";
        typeScript += $"\t$http;\n";
        typeScript += $"\t$scope;\n";
        typeScript += $"\tctrlName;\n";
            
        typeScript += "\tconstructor( $scope,$element,$attrs ){ \n";            
        typeScript += "\n\t    $scope.$element=$element; this.ctrlName = '"+ Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "';\n";
        typeScript += "\n\t    $element[0].ctrl=$scope; const ctrl = this; window['app'].scopes[$scope.$id]=this;\n";
        typeScript += "\n\t    $scope.$watch('model',function(e){ console.log(e); })\n";

        typeScript += "\n\t    Object.assign($scope,new " + Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "($element[0])); \n";
        if (HasBaseType(controllerModel.ControllerType, "BaseController"))
        {
            typeScript += "\n\t    Object.assign(this,{ $onInit(){ console.log('init ',$scope); $scope.ngOnInit().then((resp)=>{Object.assign($scope,resp);},console.error); }, $onDestroy(){$scope.ngOnDesctroy();}, $onChanges(changes){ $scope.ngOnChanges(changes).then(console.log,console.error); } }); \n";                
        }
        typeScript += "\n\t    window['$" + Naming.ToCamelStyle(controllerModel.Name).Replace("Controller","") + "'] = $scope; \n";
        typeScript += "\n\t    window['$" + Naming.ToCamelStyle(controllerModel.Name) + "'] = $scope; \n";
        typeScript += "\n\t    //console.log('$" + Naming.ToCamelStyle(controllerModel.Name) + "', window['$" + Naming.ToCamelStyle(controllerModel.Name) + "'] = $scope); \n";
        typeScript += "\n\t    $scope['$" + Naming.ToCamelStyle(controllerModel.Name) + "']=window['$" + Naming.ToCamelStyle(controllerModel.Name) + "'] = $scope; \n";
        typeScript +=
        "\n\n\t\tfunction toHttpParams(obj: any): {[property: string]: string} " +
        "\n\t\t{ " +
        "\n\t\t     const result: {[property: string]: string} = { }; " +
        "\n\t\t     Object.getOwnPropertyNames(obj).forEach(name => { " +
        "\n\t\t         result[name] = window['convertToHttpMessageParam'](obj[name]); " +
        "\n\t\t     }); " +
        "\n\t\t     return result; " +
        "\n\t\t}           \n";
 
        typeScript += "\t\tthis.$scope = $scope; \n\n";
        typeScript += "\t\tObject.assign(this.$scope,{\n\n";
            
            
        typeScript += "\t\t}); \n";
        typeScript += "\t\tObject.assign($element[0],$scope);\n";
        
        typeScript += "\t}\n\n\n";

        typeScript += "}\n";
        //typeScript += $"var ${Naming.ToCamelStyle(controllerModel.Name).Replace("Controller","")} = new { controllerModel.Name }( this,[window],null );";
        return typeScript;
    }


        
}

        
