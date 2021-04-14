using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UserContactGroupIdParameter : RecipientIdParameter
	{
		public UserContactGroupIdParameter(string identity) : base(identity)
		{
		}

		public UserContactGroupIdParameter()
		{
		}

		public UserContactGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UserContactGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return UserContactGroupIdParameter.AllowedRecipientTypes;
			}
		}

		public new static UserContactGroupIdParameter Parse(string identity)
		{
			return new UserContactGroupIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeUserContactGroupIdParameter(id);
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
			RecipientType.MailNonUniversalGroup
		};
	}
}
