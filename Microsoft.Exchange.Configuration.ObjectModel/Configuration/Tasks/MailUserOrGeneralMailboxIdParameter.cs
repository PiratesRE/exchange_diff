using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailUserOrGeneralMailboxIdParameter : GeneralMailboxIdParameter
	{
		public MailUserOrGeneralMailboxIdParameter(string identity) : base(identity)
		{
		}

		public MailUserOrGeneralMailboxIdParameter()
		{
		}

		public MailUserOrGeneralMailboxIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailUserOrGeneralMailboxIdParameter(MailUser user) : base(user.Id)
		{
		}

		public MailUserOrGeneralMailboxIdParameter(User user) : base(user.Id)
		{
		}

		public MailUserOrGeneralMailboxIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public MailUserOrGeneralMailboxIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailUserOrGeneralMailboxIdParameter.AllowedRecipientTypes;
			}
		}

		public new static MailUserOrGeneralMailboxIdParameter Parse(string identity)
		{
			return new MailUserOrGeneralMailboxIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxOrMailUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox,
			RecipientType.SystemAttendantMailbox,
			RecipientType.SystemMailbox,
			RecipientType.MailUser
		};
	}
}
