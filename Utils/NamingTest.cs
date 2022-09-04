 
public class NamingTest  
{
    public   System.Collections.Generic.List<string> OnTest()
    {
        canConvertIdentifier();
        canConvertNameToDiffrentStyles();
        return Messages;
    }

    System.Collections.Generic.List<string> Messages = new System.Collections.Generic.List<string>();   
    protected  void canConvertIdentifier()
    {
        if (Naming.ToCamelStyle("HomeController") != "homeController")
            Messages.Add("Не удалось применить CamelStyle");
        Messages.Add("Есть функция получения идентификатора в форме CamelStyle");
    }

    private void canConvertNameToDiffrentStyles()
    {
        string capitalStyle = "AppModule";
        
        Messages.Add($"Имя: [{capitalStyle}] в SnakeStyle:[{capitalStyle.ToSnakeStyle()}]");
        Messages.Add($"Имя: [{capitalStyle}] в CamelStyle:[{capitalStyle.ToCamelStyle()}]");
        Messages.Add($"Имя: [{capitalStyle}] в KebabStyle:[{capitalStyle.ToKebabStyle()}]");        
    }

}

 
