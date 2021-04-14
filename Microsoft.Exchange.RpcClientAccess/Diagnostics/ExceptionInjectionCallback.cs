using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal static class ExceptionInjectionCallback
	{
		public static Exception ExceptionLookup(string exceptionType)
		{
			Exception result = null;
			if (!string.IsNullOrEmpty(exceptionType))
			{
				if (exceptionType.Equals("System.NullReferenceException", StringComparison.OrdinalIgnoreCase))
				{
					result = new NullReferenceException();
				}
				else if (exceptionType.Equals("System.ArgumentNullException", StringComparison.OrdinalIgnoreCase))
				{
					result = new ArgumentNullException();
				}
				else if (exceptionType.Equals("NonGrayException", StringComparison.OrdinalIgnoreCase))
				{
					result = new NonGrayException();
				}
			}
			return result;
		}
	}
}
