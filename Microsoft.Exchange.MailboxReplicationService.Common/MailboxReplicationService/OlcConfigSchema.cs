using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OlcConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "Olc";
			}
		}

		[ConfigurationProperty("OlcTopology", DefaultValue = null)]
		internal string OlcTopology
		{
			get
			{
				return this.InternalGetConfig<string>("OlcTopology");
			}
			set
			{
				this.InternalSetConfig<string>(value, "OlcTopology");
			}
		}
	}
}
