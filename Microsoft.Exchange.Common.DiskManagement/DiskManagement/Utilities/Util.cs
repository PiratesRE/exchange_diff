using System;
using System.ComponentModel;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common.DiskManagement.Utilities
{
	public static class Util
	{
		public static string RemoveEscapeCharacters(string path)
		{
			return path.Replace("\\\\", "\\");
		}

		public static string WindowsErrorMessageLookup(int errorID)
		{
			Win32Exception ex = new Win32Exception(errorID);
			return ex.Message;
		}

		public static bool IsOperatingSystemWin8OrHigher()
		{
			return Environment.OSVersion.Version.Major > 6 || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2);
		}

		public static void AssertReturnValueExceptionInconsistency(int returnValue, string methodName, Exception ex)
		{
			if (returnValue == 0)
			{
				ExAssert.RetailAssert(ex == null, Util.GetReturnValueExceptionInconsistencyErrorMessage(methodName, ex));
			}
		}

		public static Exception ReturnWMIErrorExceptionOnExceptionOrError(int returnValue, string methodName, Exception ex)
		{
			if (ex != null)
			{
				return ex;
			}
			if (returnValue == 0)
			{
				return null;
			}
			return new WMIErrorException(returnValue, methodName, Util.WindowsErrorMessageLookup(returnValue));
		}

		public static string GetReturnValueExceptionInconsistencyErrorMessage(string methodName, Exception ex)
		{
			if (ex != null)
			{
				return DiskManagementStrings.ReturnValueExceptionInconsistency(methodName, ex.Message);
			}
			return DiskManagementStrings.ReturnValueExceptionInconsistency(methodName, "Exception is null");
		}

		public static Exception HandleExceptions(Action operation)
		{
			try
			{
				operation();
			}
			catch (COMException result)
			{
				return result;
			}
			catch (ManagementException result2)
			{
				return result2;
			}
			catch (ArgumentException result3)
			{
				return result3;
			}
			catch (MethodAccessException result4)
			{
				return result4;
			}
			catch (ObjectDisposedException result5)
			{
				return result5;
			}
			catch (InvalidOperationException result6)
			{
				return result6;
			}
			catch (NotSupportedException result7)
			{
				return result7;
			}
			catch (DirectoryNotFoundException result8)
			{
				return result8;
			}
			catch (BitlockerUtilException result9)
			{
				return result9;
			}
			return null;
		}

		public static void ThrowIfNotNull(Exception ex)
		{
			if (ex != null)
			{
				throw ex;
			}
		}
	}
}
