using MvcApplicationBuilderModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplicationBuilderModule
{
    public class MvcApplicationBuilderProgram
    {
        public static void Run(string[] args)
        {
            ConsoleProgram.RunInteractive<MvcApplicationBuilder>();
        }
    }
}
