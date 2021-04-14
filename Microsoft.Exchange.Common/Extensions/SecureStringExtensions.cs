using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Extensions
{
	public static class SecureStringExtensions
	{
		public static string ConvertToUnsecureString(this SecureString securePassword)
		{
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			IntPtr intPtr = IntPtr.Zero;
			string result;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
				result = Marshal.PtrToStringUni(intPtr);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
			}
			return result;
		}

		public static SecureArray<char> ConvertToSecureCharArray(this SecureString securePassword)
		{
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			SecureArray<char> secureArray = new SecureArray<char>(securePassword.Length);
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToBSTR(securePassword);
				Marshal.Copy(intPtr, secureArray.ArrayValue, 0, securePassword.Length);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(intPtr);
				}
			}
			return secureArray;
		}

		public static string AsUnsecureString(this SecureString securePassword)
		{
			if (securePassword == null)
			{
				return null;
			}
			return securePassword.ConvertToUnsecureString();
		}

		public static SecureArray<char> TransformToSecureCharArray(this SecureString securePassword, CharTransformDelegate transform)
		{
			if (securePassword == null)
			{
				throw new ArgumentNullException("securePassword");
			}
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			SecureArray<char> secureArray = null;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				secureArray = securePassword.ConvertToSecureCharArray();
				disposeGuard.Add<SecureArray<char>>(secureArray);
				int num = 0;
				bool flag = false;
				foreach (char c in secureArray.ArrayValue)
				{
					char[] array = transform(c);
					num += ((array == null) ? 1 : array.Length);
					flag |= (array != null);
				}
				if (flag)
				{
					using (SecureArray<char> secureArray2 = secureArray)
					{
						secureArray = new SecureArray<char>(num);
						disposeGuard.Add<SecureArray<char>>(secureArray);
						int num2 = 0;
						foreach (char c2 in secureArray2.ArrayValue)
						{
							char[] array2 = transform(c2);
							if (array2 == null)
							{
								secureArray.ArrayValue[num2] = c2;
								num2++;
							}
							else
							{
								array2.CopyTo(secureArray.ArrayValue, num2);
								num2 += array2.Length;
							}
						}
					}
				}
				disposeGuard.Success();
			}
			return secureArray;
		}
	}
}
