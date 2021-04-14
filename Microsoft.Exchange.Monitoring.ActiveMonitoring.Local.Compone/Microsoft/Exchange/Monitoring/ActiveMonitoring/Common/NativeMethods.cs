using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class NativeMethods
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern long GetTickCount64();
	}
}
