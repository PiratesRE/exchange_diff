using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UserContactIdParameter : RecipientIdParameter
	{
		public UserContactIdParameter(string identity) : base(identity)
		{
		}

		public UserContactIdParameter()
		{
		}

		public UserContactIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UserContactIdParameter(MailUser user) : base(user.Id)
		{
		}

		public UserContactIdParameter(User user) : base(user.Id)
		{
		}

		public UserContactIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public UserContactIdParameter(Contact contact) : base(contact.Id)
		{
		}

		public UserContactIdParameter(MailContact contact) : base(contact.Id)
		{
		}

		public UserContactIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return UserContactIdParameter.AllowedRecipientTypes;
			}
		}

		public new static UserContactIdParameter Parse(string identity)
		{
			return new UserContactIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeUserContact(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.Contact,
			RecipientType.MailContact
		};
	}
}
