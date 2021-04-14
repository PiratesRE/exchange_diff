using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExtendedMailboxPickerFilter : RecipientPickerFilterBase
	{
		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.UserMailbox,
					RecipientTypeDetails.LinkedMailbox,
					RecipientTypeDetails.SharedMailbox,
					RecipientTypeDetails.TeamMailbox,
					RecipientTypeDetails.LegacyMailbox,
					RecipientTypeDetails.RoomMailbox,
					RecipientTypeDetails.EquipmentMailbox,
					(RecipientTypeDetails)((ulong)int.MinValue),
					RecipientTypeDetails.RemoteRoomMailbox,
					RecipientTypeDetails.RemoteEquipmentMailbox,
					RecipientTypeDetails.RemoteSharedMailbox,
					RecipientTypeDetails.RemoteTeamMailbox
				};
			}
		}
	}
}
