using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal sealed class ThreadTimes
	{
		public TimeSpan Kernel { get; private set; }

		public TimeSpan User { get; private set; }

		private ThreadTimes(TimeSpan kernel, TimeSpan user)
		{
			this.Kernel = kernel;
			this.User = user;
		}

		public static ThreadTimes GetFromCurrentThread()
		{
			TimeSpan kernel;
			TimeSpan user;
			if (ThreadTimes.GetFromCurrentThread(out kernel, out user))
			{
				return new ThreadTimes(kernel, user);
			}
			return null;
		}

		public static bool GetFromCurrentThread(out TimeSpan kernelTime, out TimeSpan userTime)
		{
			bool result;
			using (SafeThreadHandle currentThread = NativeMethods.GetCurrentThread())
			{
				long num;
				long num2;
				long value;
				long value2;
				if (ThreadTimes.GetThreadTimes(currentThread.DangerousGetHandle(), out num, out num2, out value, out value2))
				{
					kernelTime = TimeSpan.FromTicks(value);
					userTime = TimeSpan.FromTicks(value2);
					result = true;
				}
				else
				{
					kernelTime = default(TimeSpan);
					userTime = default(TimeSpan);
					result = false;
				}
			}
			return result;
		}

		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);
	}
}
