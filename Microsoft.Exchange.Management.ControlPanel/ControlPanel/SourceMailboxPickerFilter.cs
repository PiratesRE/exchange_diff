using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SourceMailboxPickerFilter : RecipientPickerVersionFilter
	{
		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.UserMailbox,
					RecipientTypeDetails.LinkedMailbox,
					RecipientTypeDetails.TeamMailbox,
					RecipientTypeDetails.SharedMailbox,
					RecipientTypeDetails.LegacyMailbox,
					RecipientTypeDetails.RoomMailbox,
					RecipientTypeDetails.EquipmentMailbox,
					(RecipientTypeDetails)((ulong)int.MinValue),
					RecipientTypeDetails.RemoteRoomMailbox,
					RecipientTypeDetails.RemoteEquipmentMailbox,
					RecipientTypeDetails.RemoteTeamMailbox,
					RecipientTypeDetails.RemoteSharedMailbox
				};
			}
		}

		protected override RecipientTypeDetails[] RecipientTypeDetailsWithoutVersionRestriction
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.MailUniversalDistributionGroup,
					RecipientTypeDetails.MailUniversalSecurityGroup,
					RecipientTypeDetails.MailNonUniversalGroup,
					RecipientTypeDetails.DynamicDistributionGroup
				};
			}
		}
	}
}
