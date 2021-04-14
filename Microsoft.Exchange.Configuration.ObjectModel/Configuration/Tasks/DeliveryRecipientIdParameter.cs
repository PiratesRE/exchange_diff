using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DeliveryRecipientIdParameter : RecipientIdParameter
	{
		public DeliveryRecipientIdParameter(string identity) : base(identity)
		{
		}

		public DeliveryRecipientIdParameter()
		{
		}

		public DeliveryRecipientIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public DeliveryRecipientIdParameter(ADPresentationObject recipient) : base(recipient)
		{
		}

		public DeliveryRecipientIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return DeliveryRecipientIdParameter.AllowedRecipientTypes;
			}
		}

		public new static DeliveryRecipientIdParameter Parse(string identity)
		{
			return new DeliveryRecipientIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxOrMailUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.DynamicDistributionGroup,
			RecipientType.UserMailbox,
			RecipientType.MailContact,
			RecipientType.MailUniversalDistributionGroup,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup,
			RecipientType.MailUser,
			RecipientType.PublicFolder,
			RecipientType.MicrosoftExchange,
			RecipientType.User
		};
	}
}
