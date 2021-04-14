using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(DistributionGroupRow))]
	[DataContract]
	public class DistributionGroupRow : RecipientRow
	{
		public DistributionGroupRow(ReducedRecipient distributionGroup) : base(distributionGroup)
		{
			this.Initalize(distributionGroup.RecipientTypeDetails);
		}

		public DistributionGroupRow(MailEnabledRecipient distributionGroup) : base(distributionGroup)
		{
			this.Initalize(distributionGroup.RecipientTypeDetails);
		}

		[DataMember]
		public string GroupType { get; private set; }

		private void Initalize(RecipientTypeDetails recipientTypeDetails)
		{
			this.GroupType = DistributionGroupHelper.GenerateGroupTypeText(recipientTypeDetails);
		}
	}
}
