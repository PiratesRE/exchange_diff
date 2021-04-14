using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class SyncPropertyRefreshSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition Status = new HygienePropertyDefinition("Status", typeof(SyncPropertyRefreshStatus), SyncPropertyRefreshStatus.Requested, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
