using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Стилизация идентификаторов
/// </summary>
public static class TextNamingExtensions
{

    


    /// <summary>
    /// Метод определния стиля записи идентификатора
    /// </summary>
    /// <param name="name"> идентификатор </param>
    /// <returns> стиль записи </returns>
    public static NamingStyles ParseStyle(this string name)
    {
        return Naming.ParseStyle(name);
    }


    /// <summary>
    /// Запись идентификатора в CamelStyle
    /// </summary> 
    public static string ToCamelStyle(this string name)
    {
        return Naming.ToCamelStyle(name);
    }

    /// <summary>
    /// Запись идентификатора в CapitalStyle
    /// </summary> 
    public static string ToCapitalStyle(this string name)
    {
        return Naming.ToCapitalStyle(name);
    }




    /// <summary>
    /// Запись идентификатора в KebabStyle
    /// </summary> 
    public static string ToKebabStyle(this string lastname)
    {
        return Naming.ToKebabStyle(lastname);
    }
    


    /// <summary>
    /// Запись идентификатора в KebabStyle
    /// </summary> 
    public static string ToTSQLStyle(this string lastname)
    {
        return Naming.ToKebabStyle(lastname);
    }

    /// <summary>
    /// Запись идентификатора в SnakeStyle
    /// </summary>

    public static string ToSnakeStyle(this string lastname)
    {
        return Naming.ToSnakeStyle(lastname);
    }

}
