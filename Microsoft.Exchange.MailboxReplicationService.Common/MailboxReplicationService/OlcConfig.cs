using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OlcConfig : ConfigBase<OlcConfigSchema>
	{
		public const string OlcTopology = "OlcTopology";
	}
}
