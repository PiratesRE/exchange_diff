using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal sealed class RemoteDomainValidator : ConfigValidator
	{
		public RemoteDomainValidator(ReplicationTopology topology) : base(topology, "Remote Domain")
		{
			base.ConfigDirectoryPath = "CN=Internet Message Formats,CN=Global Settings";
			base.LdapQuery = Schema.Query.QueryAll;
		}

		protected override string[] PayloadAttributes
		{
			get
			{
				return Schema.DomainConfig.PayloadAttributes;
			}
		}

		protected override bool Filter(ExSearchResultEntry entry)
		{
			return !base.IsEntryContainer(entry);
		}

		protected override bool FilterEdge(ExSearchResultEntry entry)
		{
			return !base.IsEntryContainer(entry);
		}
	}
}
