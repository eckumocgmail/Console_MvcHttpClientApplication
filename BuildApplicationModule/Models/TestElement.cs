using System.Collections.Generic;

public abstract class TestElement
{
    public List<string> Messages { get; set; }
    public abstract List<string> OnTest();

}