using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MailboxOwnerAdapter : IMailboxOwner
	{
		protected MailboxOwnerAdapter(IConstraintProvider constraintProvider, RecipientTypeDetails recipientTypeDetails, LogonType logonType)
		{
			this.recipientTypeDetails = recipientTypeDetails;
			this.logonType = logonType;
			this.constraintProvider = constraintProvider;
		}

		public bool SideConversationProcessingEnabled
		{
			get
			{
				return this.IsLogonTypeSupported && this.IsRecipientTypeSupported && this.VariantConfig.DataStorage.ModernMailInfra.Enabled;
			}
		}

		public bool ThreadedConversationProcessingEnabled
		{
			get
			{
				return this.IsLogonTypeSupported && this.IsRecipientTypeSupported && this.VariantConfig.DataStorage.ThreadedConversation.Enabled;
			}
		}

		public bool ModernConversationPreparationEnabled
		{
			get
			{
				return this.VariantConfig.DataStorage.ModernConversationPrep.Enabled;
			}
		}

		public bool RequestExtraPropertiesWhenSearching
		{
			get
			{
				return this.ModernConversationPreparationEnabled || this.ModernConversationEnabled;
			}
		}

		public bool SearchDuplicatedMessagesEnabled
		{
			get
			{
				return this.ModernConversationEnabled;
			}
		}

		public bool IsGroupMailbox
		{
			get
			{
				return false;
			}
		}

		public bool SentToMySelf(ICorePropertyBag item)
		{
			return MailboxOwnerAdapter.CheckUserEquality(item, ItemSchema.From, this.User) || MailboxOwnerAdapter.CheckUserEquality(item, ItemSchema.Sender, this.User);
		}

		private bool ModernConversationEnabled
		{
			get
			{
				return this.SideConversationProcessingEnabled || this.ThreadedConversationProcessingEnabled;
			}
		}

		private bool IsLogonTypeSupported
		{
			get
			{
				switch (this.logonType)
				{
				case LogonType.Owner:
				case LogonType.Delegated:
				case LogonType.Transport:
					return true;
				}
				return false;
			}
		}

		private bool IsRecipientTypeSupported
		{
			get
			{
				return this.recipientTypeDetails == RecipientTypeDetails.UserMailbox;
			}
		}

		protected VariantConfigurationSnapshot VariantConfig
		{
			get
			{
				if (this.variantConfiguration == null)
				{
					this.variantConfiguration = VariantConfiguration.GetSnapshot(this.constraintProvider, null, null);
				}
				return this.variantConfiguration;
			}
		}

		protected IGenericADUser User
		{
			get
			{
				if (this.user == null)
				{
					this.user = this.CalculateGenericADUser();
				}
				return this.user;
			}
		}

		protected abstract IGenericADUser CalculateGenericADUser();

		private static bool CheckUserEquality(ICorePropertyBag item, StorePropertyDefinition participantPropertyDefinition, IGenericADUser adUser)
		{
			Participant participant = item.GetValueOrDefault<Participant>(participantPropertyDefinition, null);
			if (participant == null)
			{
				return false;
			}
			byte[] valueOrDefault = participant.GetValueOrDefault<byte[]>(ParticipantSchema.ParticipantSID);
			byte[] array = MailboxOwnerAdapter.CalculateEffectiveId(adUser);
			return (array != null && valueOrDefault != null && ByteArrayComparer.Instance.Equals(valueOrDefault, array)) || StringComparer.OrdinalIgnoreCase.Equals(adUser.LegacyDn, participant.EmailAddress) || StringComparer.OrdinalIgnoreCase.Equals(adUser.PrimarySmtpAddress.ToString(), participant.EmailAddress) || adUser.EmailAddresses.Any((ProxyAddress address) => StringComparer.OrdinalIgnoreCase.Equals(address.AddressString, participant.EmailAddress));
		}

		private static byte[] CalculateEffectiveId(IGenericADUser adUser)
		{
			byte[] result = null;
			SecurityIdentifier securityIdentifier = IdentityHelper.CalculateEffectiveSid(adUser.Sid, adUser.MasterAccountSid);
			if (securityIdentifier != null)
			{
				result = ValueConvertor.ConvertValueToBinary(securityIdentifier, null);
			}
			return result;
		}

		private readonly LogonType logonType;

		private readonly RecipientTypeDetails recipientTypeDetails;

		private readonly IConstraintProvider constraintProvider;

		private VariantConfigurationSnapshot variantConfiguration;

		private IGenericADUser user;
	}
}
