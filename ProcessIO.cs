using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace DOL
{

    public interface IProcessMemory
    {
        void ReadMemory(int start, byte[] buffer);
        void WriteMemory(int start, byte[] buffer);
    }

    internal sealed class GameProcess : IDisposable, IProcessMemory
    {
        private Process _process;
        private bool _suspended;

        public GameProcess(LaunchArguments arguments)
        {
            string args =
                $"{arguments.Host} " +
                $"{arguments.Port} " +
                $"{42} " +
                $"{arguments.Account} " +
                $"{arguments.Password} " +
                $"{arguments.Character}" +
                $"{arguments.Realm}";

            _process = new Process
            {
                StartInfo =
            {
                FileName = arguments.GameDll.FullName,
                Arguments = args.Trim(),
                UseShellExecute = false,
                WorkingDirectory = arguments.GameDll.DirectoryName
            }
            };

            _process.Start();

            Suspend();
        }

        private void Suspend()
        {
            if (_suspended)
                return;

            foreach (IntPtr threadHandle in _process.Threads
                .OfType<ProcessThread>()
                .Select(t => NativeMethods.OpenThread(
                    0x0002,
                    false,
                    (uint)t.Id))
                .Where(h => h != IntPtr.Zero))
            {
                NativeMethods.SuspendThread(threadHandle);
            }

            _suspended = true;
        }

        private void Resume()
        {
            if (!_suspended)
                return;

            foreach (IntPtr threadHandle in _process.Threads
                .OfType<ProcessThread>()
                .Select(t => NativeMethods.OpenThread(
                    0x0002,
                    false,
                    (uint)t.Id))
                .Where(h => h != IntPtr.Zero))
            {
                NativeMethods.ResumeThread(threadHandle);
            }

            _suspended = false;
        }

        public void ReadMemory(int start, byte[] buffer)
        {
            uint bytesRead = 0;

            NativeMethods.ReadProcessMemory(
                _process.Handle,
                (IntPtr)start,
                buffer,
                (uint)buffer.Length,
                ref bytesRead);

            if (bytesRead != buffer.Length)
                throw new AccessViolationException("Could not read Process Memory");
        }

        public void WriteMemory(int start, byte[] buffer)
        {
            uint bytesWritten;

            if (!NativeMethods.WriteProcessMemory(
                    _process.Handle,
                    (IntPtr)start,
                    buffer,
                    (IntPtr)buffer.Length,
                    out bytesWritten)
                || bytesWritten != buffer.Length)
            {
                throw new AccessViolationException("Could not write Process Memory");
            }
        }

        public void Dispose()
        {
            Resume();
            _process = null;
        }
    }
    internal static class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenThread(
        uint dwDesiredAccess,
        bool bInheritHandle,
        uint dwThreadId);

    [DllImport("kernel32.dll")]
    public static extern uint SuspendThread(
        IntPtr hThread);

    [DllImport("kernel32.dll")]
    public static extern uint ResumeThread(
        IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        uint dwSize,
        ref uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        IntPtr nSize,
        out uint lpNumberOfBytesWritten);
}

}
