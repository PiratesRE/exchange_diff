using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UserContactComputerIdParameter : RecipientIdParameter
	{
		public UserContactComputerIdParameter(string identity) : base(identity)
		{
		}

		public UserContactComputerIdParameter()
		{
		}

		public UserContactComputerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UserContactComputerIdParameter(MailUser user) : base(user.Id)
		{
		}

		public UserContactComputerIdParameter(User user) : base(user.Id)
		{
		}

		public UserContactComputerIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public UserContactComputerIdParameter(Contact contact) : base(contact.Id)
		{
		}

		public UserContactComputerIdParameter(MailContact contact) : base(contact.Id)
		{
		}

		public UserContactComputerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
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
			return Strings.WrongTypeUserContactComputer(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.Contact,
			RecipientType.MailContact,
			RecipientType.Computer
		};
	}
}
