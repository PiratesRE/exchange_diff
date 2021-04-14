using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxFilter : RecipientFilter
	{
		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

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
					RecipientTypeDetails.EquipmentMailbox
				};
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			try
			{
				switch (string.IsNullOrEmpty(base.SelectedView) ? MailboxView.All : ((MailboxView)Enum.Parse(typeof(MailboxView), base.SelectedView)))
				{
				case MailboxView.User:
					base.RecipientTypeDetailsList = new RecipientTypeDetails[]
					{
						RecipientTypeDetails.UserMailbox
					};
					break;
				case MailboxView.ManagedUser:
					base.RecipientTypeDetailsList = new RecipientTypeDetails[]
					{
						RecipientTypeDetails.UserMailbox
					};
					base["AuthenticationType"] = AuthenticationType.Managed;
					break;
				case MailboxView.FederatedUser:
					base.RecipientTypeDetailsList = new RecipientTypeDetails[]
					{
						RecipientTypeDetails.UserMailbox
					};
					base["AuthenticationType"] = AuthenticationType.Federated;
					break;
				case MailboxView.RoomMailbox:
					base.RecipientTypeDetailsList = new RecipientTypeDetails[]
					{
						RecipientTypeDetails.RoomMailbox
					};
					break;
				case MailboxView.LitigationHold:
					base["Filter"] = "LitigationHoldEnabled -eq $true";
					break;
				}
			}
			catch (ArgumentException)
			{
			}
		}
	}
}
