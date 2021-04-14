using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class DataClassification
	{
		public DataClassification(string id)
		{
			this.Id = id;
		}

		public DataClassification()
		{
			this.Id = string.Empty;
		}

		public string Id { get; set; }

		public const string Name = "DataClassification";
	}
}
