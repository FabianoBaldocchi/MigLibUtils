using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MigLibUtils.Environment
{
    public class Processor
    {
        public static int ProcessorQty()
        {
            return System.Environment.ProcessorCount;
        }

        public static bool SetProcessCPUAffinity(int pctentrada, System.Diagnostics.Process process = null)
        {
            if (process == null)
                process = System.Diagnostics.Process.GetCurrentProcess();

            if (pctentrada < 1)
                pctentrada = 1;
            else if (pctentrada > 100)
                pctentrada = 100;

            var cpuCount = ProcessorQty();

            var pct = (int)Math.Round(cpuCount * pctentrada / 100.0, 0);

            //se ficar com 100% mas a entrada é menor que 100%, reduz um
            if (pct == cpuCount && pctentrada < 100)
                pct--;

            //define ao menos um processador
            if (pct == 0)
                pct = 1;

            var direcao = (new Random().Next(10) < 5);

            uint ptr = 0;

            if (direcao)
                for (int i = 0; i < pct; ++i)
                {
                    ptr = ptr | (uint)(1 << i);
                }
            else
                for (int i = pct - 1; i >= 0; --i)
                {
                    ptr = ptr | (uint)(1 << i);
                }



            process.ProcessorAffinity = (IntPtr)ptr;
            return true;
        }



    }
}
