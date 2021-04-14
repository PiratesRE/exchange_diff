using System;
using System.Security;

namespace Microsoft.Exchange.Extensions
{
	public static class CharArrayExtensions
	{
		public unsafe static SecureString ConvertToSecureString(this char[] password)
		{
			if (password == null || password.Length == 0)
			{
				return new SecureString();
			}
			fixed (char* ptr = password)
			{
				SecureString secureString = new SecureString(ptr, password.Length);
				secureString.MakeReadOnly();
				return secureString;
			}
		}
	}
}
