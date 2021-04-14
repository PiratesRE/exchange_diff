using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class TransportServerValidator : ConfigValidator
	{
		public TransportServerValidator(ReplicationTopology topology) : base(topology, "Transport Server")
		{
			base.ConfigDirectoryPath = "CN=" + AdministrativeGroup.DefaultName + ",CN=Administrative Groups";
			base.LdapQuery = Schema.Query.QueryBridgeheads;
		}

		protected override string[] PayloadAttributes
		{
			get
			{
				return Schema.Server.PayloadAttributes;
			}
		}

		protected override string[] ReadAttributes
		{
			get
			{
				return new string[]
				{
					"msExchServerSite"
				};
			}
		}

		protected override bool CompareAttributes(ExSearchResultEntry edgeEntry, ExSearchResultEntry hubEntry, string[] copyAttributes)
		{
			return this.Filter(hubEntry) && base.CompareAttributes(edgeEntry, hubEntry, copyAttributes);
		}

		protected override bool Filter(ExSearchResultEntry entry)
		{
			if (entry.Attributes.ContainsKey("msExchServerSite"))
			{
				string text = (string)entry.Attributes["msExchServerSite"][0];
				if (!string.IsNullOrEmpty(text) && string.Compare(text, base.Topology.LocalHub.ServerSite.DistinguishedName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private const string ServerSite = "msExchServerSite";
	}
}
