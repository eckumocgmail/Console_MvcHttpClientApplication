using System.Linq;
using System.Reflection;

public class AssemblyExtensionsTest : TestElement
{
     
    public void GetControllersTest() {
        if (Assembly.GetExecutingAssembly().GetAttributes().Count() > 0)
            Messages.Add("Реализована функция получения атрибутов из сборки");
    }

    public override System.Collections.Generic.List<string> OnTest()
    {     
        GetControllersTest(); return Messages;
    }
}
