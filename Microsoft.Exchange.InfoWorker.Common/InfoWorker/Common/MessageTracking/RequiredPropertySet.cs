using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal static class RequiredPropertySet
	{
		internal const RequiredProperty None = RequiredProperty.None;

		internal const RequiredProperty ServerDomainTarget = RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target;

		internal const RequiredProperty ServerDomainData = RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Data;
	}
}
