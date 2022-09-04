using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ActionExtensionsTest: TestElement
{
 
    public override List<string> OnTest()
    {
       
        CanInvokeAfterTimeout();
        CanSimulateActionInvokation();
        CanCatchActionTest();

        return Messages;
    }
 
    public void CanCatchActionTest() { }
    

 
    private void CanSimulateActionInvokation()
    {
        try
        {
            int completed = 0;
            var context = new object();
            context.Simulate(() => { completed++; }, 9, 100);
            Thread.Sleep(1000);
            if (completed!=9)
            {
                Messages.Add("Реализована функция отложенного выполнения операций c заданным кол-вом итераций и таймаутом");
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            Messages.Add("Не реализована функция отложенного выполнения операций c заданным кол-вом итераций и таймаутом");
        }
    }

    private void CanInvokeAfterTimeout()
    {
        try
        {
            bool completed = false;
            var context = new object();
            context.SetTimeout(() => { completed = true; }, 100);
            Thread.Sleep(1000);
            if (completed)
            {
                Messages.Add("Реализована функция отложенного выполнения операций");
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            Messages.Add("Не реализована функция отложенного выполнения операций");
        }
    }
}