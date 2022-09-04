using System;
using System.Collections.Generic;
using System.Linq;

public class CollectionsExtensionsTest : TestElement
{

    public void ForEachTest() {
        IEnumerable<int> numbers = new HashSet<int>() { 1, 2, 3, 4 };
        numbers.ForEach(Console.WriteLine);
        Messages.Add("Реализована фуцнкция передора перечисляемых объектов");
    }

    public void PrintTest() {
        IEnumerable<int> numbers = new HashSet<int>() { 1, 2, 3, 4 };
        numbers.Print();
        Messages.Add("Реализована фуцнкция печати перечисляемых объектов");

    }

    public void AddRangeTest() {
        HashSet<int> numbers = new HashSet<int>() { 1, 2, 3, 4 };
        numbers.AddRange<int>("7,8,9".Split(",").Select(t=>int.Parse(t)));
        Messages.Add("Реализована фуцнкция добавления множеств");
    }

    public override List<string> OnTest()
    {
        ForEachTest();
        PrintTest();
        ForEachTest();
        AddRangeTest();
        return Messages;
    }

    public void GetPageTest()
    {
        try
        {
            var list = new List<int>();
            for (int i = 0; i < 100; i++)
                list.Add(i);
            if (list.GetPage<int>(1, 10).Count() != 10)
            {
                throw new System.Exception("Метод постраничного просмотра коллекции не работает");
            }
            Messages.Add("Реализован метод постраничного просмотра сущностей для любых коллекций");
        }
        catch (Exception ex)
        {
            Messages.Add(ex.Message);
        }
    }

}
