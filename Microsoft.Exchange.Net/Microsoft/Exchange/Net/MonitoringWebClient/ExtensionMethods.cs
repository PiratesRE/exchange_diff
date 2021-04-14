using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal static class ExtensionMethods
	{
		public static ScenarioException GetScenarioException(this Exception e)
		{
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				if (ex is ScenarioException)
				{
					return ex as ScenarioException;
				}
			}
			return null;
		}

		public static byte[] ConvertToByteArray(this SecureString secureString)
		{
			IntPtr intPtr = IntPtr.Zero;
			byte[] result;
			try
			{
				int num = secureString.Length * 2;
				byte[] array = new byte[num];
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
				Marshal.Copy(intPtr, array, 0, num);
				result = array;
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocAnsi(intPtr);
				}
			}
			return result;
		}

		public static string ConvertToUnsecureString(this SecureString secureString)
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
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
			}
			return result;
		}

		public unsafe static SecureString ConvertToSecureString(this string stringToConvert)
		{
			if (stringToConvert == null)
			{
				throw new ArgumentNullException("stringToConvert");
			}
			IntPtr intPtr2;
			IntPtr intPtr = intPtr2 = stringToConvert;
			if (intPtr != 0)
			{
				intPtr2 = (IntPtr)((int)intPtr + RuntimeHelpers.OffsetToStringData);
			}
			char* value = intPtr2;
			SecureString secureString = new SecureString(value, stringToConvert.Length);
			secureString.MakeReadOnly();
			return secureString;
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			Random random = new Random();
			for (int i = list.Count - 1; i > 0; i--)
			{
				int index = random.Next(i + 1);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
		}

		public static string GetCanonicalHostAddress(this Uri uri)
		{
			IPAddress ipaddress;
			if (IPAddress.TryParse(uri.DnsSafeHost, out ipaddress))
			{
				return ipaddress.ToString();
			}
			return uri.DnsSafeHost;
		}

		public static bool ContainsMatchingSuffix(this IEnumerable<string> list, string searchString)
		{
			foreach (string value in list)
			{
				if (searchString.EndsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsMatchingSubstring(this IEnumerable<string> list, string searchString)
		{
			foreach (string value in list)
			{
				if (searchString.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
