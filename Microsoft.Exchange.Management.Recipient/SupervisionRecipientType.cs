using System;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum SupervisionRecipientType
	{
		None,
		[LocDescription(Strings.IDs.IndividualRecipient)]
		IndividualRecipient,
		[LocDescription(Strings.IDs.DistributionGroup)]
		DistributionGroup,
		[LocDescription(Strings.IDs.ExternalAddress)]
		ExternalAddress
	}
}
