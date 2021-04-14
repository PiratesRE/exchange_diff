using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class RetentionPolicyTagDisplay : OptionsPropertyChangeTracker
	{
		[DataMember]
		public Identity Identity { get; set; }

		[DataMember]
		public Guid RetentionId { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public ElcFolderType Type { get; set; }

		[DataMember]
		public RetentionActionType RetentionAction { get; set; }

		[DataMember]
		public bool RetentionEnabled { get; set; }

		[DataMember]
		public int? AgeLimitForRetentionDays { get; set; }

		[DataMember]
		public bool OptionalTag { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}
