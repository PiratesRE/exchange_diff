using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Net
{
	internal static class UserInformation
	{
		public static string UserPrincipalName
		{
			get
			{
				return UserInformation.GetUserNameEx(NativeMethods.ExtendedNameFormat.UserPrincipal);
			}
		}

		private static string GetUserNameEx(NativeMethods.ExtendedNameFormat type)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			int capacity = stringBuilder.Capacity;
			if (!NativeMethods.GetUserNameEx(type, stringBuilder, ref capacity))
			{
				int hrforLastWin32Error = Marshal.GetHRForLastWin32Error();
				if (234 == hrforLastWin32Error)
				{
					stringBuilder.Capacity = capacity + 1;
					if (!NativeMethods.GetUserNameEx(NativeMethods.ExtendedNameFormat.UserPrincipal, stringBuilder, ref capacity))
					{
						hrforLastWin32Error = Marshal.GetHRForLastWin32Error();
					}
				}
				if (hrforLastWin32Error != 0)
				{
					Marshal.ThrowExceptionForHR(hrforLastWin32Error);
				}
			}
			return stringBuilder.ToString().Trim();
		}

		internal static class WindowsErrorCode
		{
			internal const int SUCCESS = 0;

			internal const int MOREDATA = 234;
		}
	}
}
