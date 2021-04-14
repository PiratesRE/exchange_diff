using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class GeneralMailboxOrMailUserIdParameter : RecipientIdParameter
	{
		public GeneralMailboxOrMailUserIdParameter(string identity) : base(identity)
		{
		}

		public GeneralMailboxOrMailUserIdParameter()
		{
		}

		public GeneralMailboxOrMailUserIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public GeneralMailboxOrMailUserIdParameter(MailUser user) : base(user.Id)
		{
		}

		public GeneralMailboxOrMailUserIdParameter(User user) : base(user.Id)
		{
		}

		public GeneralMailboxOrMailUserIdParameter(Mailbox user) : base(user.Id)
		{
		}

		public GeneralMailboxOrMailUserIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return GeneralMailboxOrMailUserIdParameter.AllowedRecipientTypes;
			}
		}

		public new static GeneralMailboxOrMailUserIdParameter Parse(string identity)
		{
			return new GeneralMailboxOrMailUserIdParameter(identity);
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
