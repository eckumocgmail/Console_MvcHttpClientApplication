using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// Модель параметров вызова удаленной процедуры
/// </summary>
//[EntityIcon("")]
//[Label("")]
//[Description("Контроллер предназначен для .")]
public class MyActionModel   
{

 

    //[Icon("account_tree")]
    //[Label("Путь")]
    //[InputText()]
    [Required(ErrorMessage ="Необходимо ввести путь")]
    public virtual string Path { get; set; }

    public virtual string Method { get; set; }

    //[NotMapped]
    //[JsonIgnore]
    public virtual List<string> PathStr
    {
        set
        {
            string spath = "";
            value.ForEach(p => { spath += "/" + p; });
            Path = spath;
        }
    }



    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, MyParameterDeclarationModel> Parameters { get; set; } = new Dictionary<string, MyParameterDeclarationModel>();
    public string Name { get; internal set; }
}
