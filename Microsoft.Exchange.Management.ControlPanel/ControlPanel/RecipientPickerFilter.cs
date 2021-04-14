using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RecipientPickerFilter : RecipientPickerFilterBase
	{
		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.RoomMailbox,
					RecipientTypeDetails.EquipmentMailbox,
					RecipientTypeDetails.LegacyMailbox,
					RecipientTypeDetails.LinkedMailbox,
					RecipientTypeDetails.UserMailbox,
					RecipientTypeDetails.MailContact,
					RecipientTypeDetails.DynamicDistributionGroup,
					RecipientTypeDetails.MailForestContact,
					RecipientTypeDetails.MailNonUniversalGroup,
					RecipientTypeDetails.MailUniversalDistributionGroup,
					RecipientTypeDetails.MailUniversalSecurityGroup,
					RecipientTypeDetails.MailUser,
					RecipientTypeDetails.PublicFolder,
					RecipientTypeDetails.TeamMailbox,
					RecipientTypeDetails.SharedMailbox
				};
			}
		}
	}
}
