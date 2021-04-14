using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UnifiedGroupParticipant
	{
		private UnifiedGroupParticipant(ADRawEntry participantEntry)
		{
			ArgumentValidator.ThrowIfNull("participantEntry", participantEntry);
			this.participantEntry = participantEntry;
		}

		public bool IsOwner
		{
			get
			{
				return this.isOwner;
			}
			internal set
			{
				this.isOwner = value;
			}
		}

		public ADObjectId Id
		{
			get
			{
				return (ADObjectId)this.participantEntry[ADObjectSchema.Id];
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this.participantEntry[ADRecipientSchema.DisplayName];
			}
		}

		public string Title
		{
			get
			{
				return (string)this.participantEntry[ADUserSchema.Title];
			}
		}

		public string Alias
		{
			get
			{
				return (string)this.participantEntry[ADRecipientSchema.Alias];
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				return (SmtpAddress)this.participantEntry[ADRecipientSchema.PrimarySmtpAddress];
			}
		}

		public string PrimarySmtpAddressToString
		{
			get
			{
				return this.PrimarySmtpAddress.ToString();
			}
		}

		public string SipAddress
		{
			get
			{
				string result;
				if ((result = this.sipAddress) == null)
				{
					result = (this.sipAddress = ADPersonToContactConverter.GetSipUri(this.participantEntry));
				}
				return result;
			}
		}

		public string ExternalDirectoryObjectId
		{
			get
			{
				return (string)this.participantEntry[ADRecipientSchema.ExternalDirectoryObjectId];
			}
		}

		internal static UnifiedGroupParticipant CreateFromADRawEntry(ADRawEntry entry)
		{
			return new UnifiedGroupParticipant(entry);
		}

		internal static readonly ADPropertyDefinition[] DefaultMemberProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName,
			ADUserSchema.Title,
			ADRecipientSchema.Alias,
			ADUserSchema.RTCSIPPrimaryUserAddress,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.ExternalDirectoryObjectId
		};

		private ADRawEntry participantEntry;

		private string sipAddress;

		private bool isOwner;
	}
}
