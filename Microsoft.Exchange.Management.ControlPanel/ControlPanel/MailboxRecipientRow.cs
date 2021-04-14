using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(MailboxRecipientRow))]
	public class MailboxRecipientRow : RecipientRow
	{
		public MailboxRecipientRow(ReducedRecipient recipient) : base(recipient)
		{
			this.Recipient = recipient;
			this.IsUserManaged = (recipient.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UserMailbox && recipient.AuthenticationType == AuthenticationType.Managed);
			this.IsUserFederated = (recipient.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UserMailbox && recipient.AuthenticationType == AuthenticationType.Federated);
			this.IsRoom = (recipient.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RoomMailbox);
			this.Type = recipient.RecipientTypeDetails.ToString();
			this.IsResetPasswordAllowed = RbacPrincipal.Current.RbacConfiguration.IsCmdletAllowedInScope("Set-Mailbox", new string[]
			{
				"Password"
			}, recipient, ScopeLocation.RecipientWrite);
			this.IsKeepWindowsLiveIdAllowed = RbacPrincipal.Current.IsInRole("Remove-Mailbox?KeepWindowsLiveId@W:Organization");
			this.MailboxType = MailboxRecipientRow.GenerateMailboxTypeText(recipient.RecipientTypeDetails, recipient.ArchiveGuid, this.IsUserFederated);
			base.SpriteId = Icons.FromEnum(recipient.RecipientTypeDetails, recipient.ArchiveGuid, this.IsUserFederated);
		}

		public MailboxRecipientRow(MailEnabledRecipient recipient) : base(recipient)
		{
			this.IsRoom = (recipient.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RoomMailbox);
		}

		public ReducedRecipient Recipient { get; private set; }

		[DataMember]
		public string Type { get; private set; }

		[DataMember]
		public string MailboxType { get; private set; }

		[DataMember]
		public bool IsUserManaged { get; private set; }

		[DataMember]
		public bool IsUserFederated { get; private set; }

		[DataMember]
		public bool IsRoom { get; private set; }

		[DataMember]
		public bool IsResetPasswordAllowed { get; private set; }

		[DataMember]
		public bool IsKeepWindowsLiveIdAllowed { get; private set; }

		public static string GenerateMailboxTypeText(RecipientTypeDetails recipientTypeDetails, Guid archiveGuid, bool isUserFederated)
		{
			string text = string.Empty;
			if (recipientTypeDetails <= Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.LegacyMailbox)
			{
				if (recipientTypeDetails <= Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.SharedMailbox)
				{
					if (recipientTypeDetails < Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UserMailbox)
					{
						goto IL_BA;
					}
					switch ((int)(recipientTypeDetails - Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.UserMailbox))
					{
					case 0:
						text = (isUserFederated ? Strings.FederatedUserMailboxText : Strings.UserMailboxText);
						goto IL_C6;
					case 1:
						text = Strings.LinkedMailboxText;
						goto IL_C6;
					case 2:
						goto IL_BA;
					case 3:
						text = Strings.SharedMailboxText;
						goto IL_C6;
					}
				}
				if (recipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.LegacyMailbox)
				{
					text = Strings.LegacyMailboxText;
					goto IL_C6;
				}
			}
			else
			{
				if (recipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RoomMailbox)
				{
					text = Strings.RoomMailboxText;
					goto IL_C6;
				}
				if (recipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.EquipmentMailbox)
				{
					text = Strings.EquipmentMailboxText;
					goto IL_C6;
				}
				if (recipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.TeamMailbox)
				{
					text = Strings.TeamMailboxText;
					goto IL_C6;
				}
			}
			IL_BA:
			text = recipientTypeDetails.ToString();
			IL_C6:
			return archiveGuid.Equals(Guid.Empty) ? text : string.Format(Strings.ArchiveText, text);
		}
	}
}
