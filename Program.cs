using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace R6S_MidGameLagFix
{
    class Program
    {
        static void Main(string[] args)
        {
            TryFix();

            Console.ReadKey();
        }

        static void TryFix()
        {
            Process[] processes = Process.GetProcessesByName("RainbowSix");
            if (processes.Length == 0)
            {
                LogEr("Processes not found");
                return;
            }

            var r6 = processes[0];
            if (null == r6)
            {
                LogEr("Process not found");
                return;
            }

            IntPtr h = r6.Handle;

            UIntPtr oneCoreMask = (UIntPtr)(0x0001);
            UIntPtr yourCoreMask;

            if (GetProcessAffinityMask(h, out yourCoreMask, out _))
            {
                LogOk("Get affinity");
            }
            else
            {
                LogEr("Cant get affinity");
                return;
            }

            if (SetProcessAffinityMask(h, oneCoreMask))
            {
                LogOk("Affinity set core 0");
            }
            else
            {
                LogEr("Cant set affinity core 0");
            }

            if (SetProcessAffinityMask(h, yourCoreMask))
            {
                LogOk("Affinity restored\nSuccess");
            }
            else
            {
                LogEr("Cant restore affinity, restore it yourself with task manager");
            }
        }

        static void LogOk(string msg) => COut(true, msg);
        static void LogEr(string msg) => COut(false, msg);

        static void COut(bool res, string msg)
        {
            if (res)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK: " + msg);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ER: " + msg);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetProcessAffinityMask(IntPtr hProcess, out UIntPtr lpProcessAffinityMask, out UIntPtr lpSystemAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetProcessAffinityMask(IntPtr hProcess, UIntPtr processAffinityMask);

    }
}

