using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GetMsoRawObjectParameterSets
	{
		public const string ExchangeIdentity = "ExchangeIdentity";

		public const string SyncObjectId = "SyncObjectId";
	}
}
