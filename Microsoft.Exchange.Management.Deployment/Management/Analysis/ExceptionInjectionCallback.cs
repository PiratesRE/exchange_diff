using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.Analysis
{
	internal static class ExceptionInjectionCallback
	{
		public static Exception Win32InvalidOperationException(string exceptionType)
		{
			Exception result = null;
			if (!string.IsNullOrEmpty(exceptionType) && exceptionType.Equals("System.InvalidOperationException", StringComparison.OrdinalIgnoreCase))
			{
				result = new InvalidOperationException("Fault Injection", new Win32Exception(1072));
			}
			return result;
		}
	}
}
