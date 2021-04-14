using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Exchange.Common.Net.Cryptography
{
	public class TextUtil
	{
		public static string Truncate(string str, Encoding encoding, int maxByteCount, bool appendDotDotDot)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (appendDotDotDot)
			{
				maxByteCount -= TextUtil.dotDotDot.Length;
			}
			if (maxByteCount <= 0)
			{
				return string.Empty;
			}
			bool flag = false;
			if (str.Length > maxByteCount)
			{
				str = str.Substring(0, maxByteCount);
				flag = true;
			}
			while (encoding.GetByteCount(str) > maxByteCount)
			{
				str = str.Remove(str.Length - 1, 1);
				flag = true;
			}
			if (flag && appendDotDotDot)
			{
				return str + TextUtil.dotDotDot;
			}
			return str;
		}

		public static string ConvertToUnsecureString(SecureString secureString)
		{
			if (secureString == null)
			{
				throw new ArgumentNullException("secureString");
			}
			IntPtr intPtr = IntPtr.Zero;
			string result;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
				result = Marshal.PtrToStringUni(intPtr);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
			}
			return result;
		}

		public static SecureString ConvertToSecureString(string unsecureString)
		{
			SecureString secureString = new SecureString();
			foreach (char c in unsecureString.ToCharArray())
			{
				secureString.AppendChar(c);
			}
			SecureString result;
			try
			{
				secureString.MakeReadOnly();
				result = secureString;
			}
			finally
			{
				unsecureString = null;
			}
			return result;
		}

		private static string dotDotDot = "...";
	}
}
