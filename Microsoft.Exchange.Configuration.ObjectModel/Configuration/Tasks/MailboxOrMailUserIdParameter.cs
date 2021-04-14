using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxOrMailUserIdParameter : RecipientIdParameter
	{
		public MailboxOrMailUserIdParameter(string identity) : base(identity)
		{
		}

		public MailboxOrMailUserIdParameter()
		{
		}

		public MailboxOrMailUserIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxOrMailUserIdParameter(MailUser user) : base(user.Id)
		{
		}

		public MailboxOrMailUserIdParameter(User user) : base(user.Id)
		{
		}

		public MailboxOrMailUserIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public MailboxOrMailUserIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailboxOrMailUserIdParameter.AllowedRecipientTypes;
			}
		}

		public new static MailboxOrMailUserIdParameter Parse(string identity)
		{
			return new MailboxOrMailUserIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxOrMailUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox,
			RecipientType.MailUser
		};
	}
}
