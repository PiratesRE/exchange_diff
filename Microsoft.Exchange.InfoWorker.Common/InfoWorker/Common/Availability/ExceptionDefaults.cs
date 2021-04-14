using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class ExceptionDefaults
	{
		internal static string DefaultMachineName
		{
			get
			{
				return Environment.MachineName;
			}
		}
	}
}
