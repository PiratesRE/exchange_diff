using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	internal class MessageClassificationValidator : ConfigValidator
	{
		public MessageClassificationValidator(ReplicationTopology topology) : base(topology, "Message Classification")
		{
			base.ConfigDirectoryPath = "CN=Message Classifications,CN=Transport Settings";
			base.LdapQuery = Schema.Query.QueryAll;
		}

		protected override string[] PayloadAttributes
		{
			get
			{
				return Schema.MessageClassification.PayloadAttributes;
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
