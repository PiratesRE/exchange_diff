using System;
using System.Security;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Extensions
{
	internal static class SafeHandleExtensions
	{
		public static SafeSecureHGlobalHandle ConvertToUnsecureHGlobal(this SecureString securePassword)
		{
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			return SafeSecureHGlobalHandle.DecryptAndAllocHGlobal(securePassword);
		}
	}
}
