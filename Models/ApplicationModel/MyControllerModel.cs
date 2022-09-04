
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;




/// <summary>
/// Модель mvc-контроллера ///[controller]///[action]
/// </summary>
//[EntityIcon("")]
//[Label("")]
//[Description("Контроллер предназначен для .")]
public class MyControllerModel: MyActionModel
{

    
   
    public Type ControllerType { get; set; }

    //[Icon("account_tree")]
    //[Label("Путь")]
    //[InputText()]
    [Required(ErrorMessage ="Необходимо ввести путь")]
    public override string Path { get; set; }



    [Required(ErrorMessage ="Необходимо зарегистрировать операции")]
    //[Label("Поддерживаемые операции")]
    //[InputStructureCollection()]
    public Dictionary<string, MyActionModel> Actions { get; set; }
            = new Dictionary<string, MyActionModel>();


    //[InputStructureCollection()]
    [Required(ErrorMessage ="Необходимо зарегистрировать операции")]
    //[Label("Поддерживаемые операции")]    
   
    public IList<string> Services { get; set; } = new List<string>();
    public string Name { get; internal set; }

    public IEnumerable<MyActionModel> GetMyActions()
    {
        var actionList = new List<MyActionModel>();
        foreach(var p in Actions)
        {
            actionList.Add(p.Value);
        }
        return actionList.ToArray();
    }


    /// <summary>
    /// Запись информации о методах в справочник
    /// </summary>
    /// <param name="data"></param>
    public void WriteTo(IDictionary data)
    {
        Actions.ToList().ForEach(a =>
        {
            
            data[a.Key] = Newtonsoft.Json.JsonConvert.SerializeObject(a.Value);
        });
    }




    public string GetAnnotationForService()
    {
        return "@Injectable({ providedIn: 'root' })\n";
    }


    public string GetImportsForService()
    {
        return
            "import { Observable } from 'rxjs';\n" +
            "import { Injectable } from '@angular/core';\n" +
            "import { HttpClient } from '@angular/common/http';\n\n";
    } 
}