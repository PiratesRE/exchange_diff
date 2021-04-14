using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxUserContactIdParameter : RecipientIdParameter
	{
		public MailboxUserContactIdParameter(string identity) : base(identity)
		{
		}

		public MailboxUserContactIdParameter()
		{
		}

		public MailboxUserContactIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxUserContactIdParameter(MailUser user) : base(user.Id)
		{
		}

		public MailboxUserContactIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public MailboxUserContactIdParameter(MailContact contact) : base(contact.Id)
		{
		}

		public MailboxUserContactIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailboxUserContactIdParameter.AllowedRecipientTypes;
			}
		}

		public new static MailboxUserContactIdParameter Parse(string identity)
		{
			return new MailboxUserContactIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxUserContact(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.MailContact
		};
	}
}
