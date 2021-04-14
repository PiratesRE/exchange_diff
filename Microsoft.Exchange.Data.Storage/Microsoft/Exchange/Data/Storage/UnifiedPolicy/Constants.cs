using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Constants
	{
		public const string UnifiedPolicySyncSessionClientString = "Client=UnifiedPolicy;Action=CommitChanges;Interactive=False";

		public const string TenantInfoMetadataName = "TenantInfoConfigurations";
	}
}
