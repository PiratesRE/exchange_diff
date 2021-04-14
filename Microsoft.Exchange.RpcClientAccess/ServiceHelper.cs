using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ServiceHelper
	{
		public static string FormatWin32ErrorString(int win32error)
		{
			Win32Exception ex = new Win32Exception(win32error);
			string format = (win32error >= -32768 && win32error <= 32767) ? "{0} ({1:d})" : "{0} (0x{1:X8})";
			return string.Format(CultureInfo.InvariantCulture, format, new object[]
			{
				ex.Message,
				win32error
			});
		}

		public static void RegisterSPN(string className, ExEventLog eventLog, ExEventLog.EventTuple tupleError)
		{
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				if (current.User.IsWellKnown(WellKnownSidType.NetworkServiceSid) || current.User.IsWellKnown(WellKnownSidType.LocalSystemSid))
				{
					int num = ServicePrincipalName.RegisterServiceClass(className);
					if (num != 0)
					{
						eventLog.LogEvent(tupleError, string.Empty, new object[]
						{
							className,
							ServiceHelper.FormatWin32ErrorString(num)
						});
					}
				}
			}
		}
	}
}
