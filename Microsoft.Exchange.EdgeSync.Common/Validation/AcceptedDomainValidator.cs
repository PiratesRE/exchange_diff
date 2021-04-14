using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class AcceptedDomainValidator : ConfigValidator
	{
		public AcceptedDomainValidator(ReplicationTopology topology) : base(topology, "Accepted Domain")
		{
			base.ConfigDirectoryPath = "CN=Accepted Domains,CN=Transport Settings";
			base.LdapQuery = Schema.Query.QueryAll;
		}

		protected override string[] PayloadAttributes
		{
			get
			{
				return Schema.AcceptedDomain.PayloadAttributes;
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
