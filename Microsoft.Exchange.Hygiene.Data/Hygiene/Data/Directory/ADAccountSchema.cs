using System;
using Microsoft.Exchange.Hygiene.Data.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ADAccountSchema : CommonSyncProperties
	{
		public static readonly HygienePropertyDefinition DisplayNameProperty = new HygienePropertyDefinition("DisplayName", typeof(string));
	}
}
