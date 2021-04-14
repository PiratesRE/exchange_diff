using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class GeneralRecipientIdParameter : RecipientIdParameter
	{
		public GeneralRecipientIdParameter(string identity) : base(identity)
		{
		}

		public GeneralRecipientIdParameter()
		{
		}

		public GeneralRecipientIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public GeneralRecipientIdParameter(ADPresentationObject recipient) : base(recipient)
		{
		}

		public GeneralRecipientIdParameter(ReducedRecipient recipient) : base(recipient.Id)
		{
		}

		public GeneralRecipientIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return GeneralRecipientIdParameter.AllowedRecipientTypes;
			}
		}

		public new static GeneralRecipientIdParameter Parse(string identity)
		{
			return new GeneralRecipientIdParameter(identity);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.Contact,
			RecipientType.MailContact,
			RecipientType.Group,
			RecipientType.MailUniversalDistributionGroup,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup,
			RecipientType.DynamicDistributionGroup,
			RecipientType.PublicFolder,
			RecipientType.Computer
		};
	}
}
