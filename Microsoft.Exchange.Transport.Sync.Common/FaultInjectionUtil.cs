using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class FaultInjectionUtil
	{
		public static Exception Callback(string exceptionType)
		{
			Exception result = null;
			if (exceptionType != null)
			{
				if (typeof(NullReferenceException).FullName.Contains(exceptionType))
				{
					result = new NullReferenceException("Fault Injection");
				}
				if (typeof(UnauthorizedAccessException).FullName.Contains(exceptionType))
				{
					result = new UnauthorizedAccessException("Fault Injection");
				}
				if (typeof(EndpointContainerNotFoundException).FullName.Contains(exceptionType))
				{
					result = new EndpointContainerNotFoundException("Fault Injection");
				}
				if (typeof(StorageTransientException).FullName.Contains(exceptionType))
				{
					result = new StorageTransientException(LocalizedString.Empty);
				}
				if (typeof(StoragePermanentException).FullName.Contains(exceptionType))
				{
					result = new StoragePermanentException(LocalizedString.Empty);
				}
			}
			return result;
		}

		private const string ExceptionMessage = "Fault Injection";
	}
}
