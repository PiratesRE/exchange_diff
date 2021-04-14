using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class GeneralMailboxIdParameter : MailboxIdParameter
	{
		public GeneralMailboxIdParameter(string identity) : base(identity)
		{
		}

		public GeneralMailboxIdParameter()
		{
		}

		public GeneralMailboxIdParameter(MailboxEntry storeMailboxEntry) : base(storeMailboxEntry)
		{
		}

		public GeneralMailboxIdParameter(MailboxId storeMailboxId) : base(storeMailboxId)
		{
		}

		public GeneralMailboxIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public GeneralMailboxIdParameter(ReducedRecipient reducedRecipient) : this(reducedRecipient.Id)
		{
		}

		public GeneralMailboxIdParameter(ADUser user) : this(user.Id)
		{
		}

		public GeneralMailboxIdParameter(ADSystemAttendantMailbox systemAttendant) : this(systemAttendant.Id)
		{
		}

		public GeneralMailboxIdParameter(Mailbox mailbox) : base(mailbox)
		{
		}

		public GeneralMailboxIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return GeneralMailboxIdParameter.AllowedRecipientTypes;
			}
		}

		public new static GeneralMailboxIdParameter Parse(string identity)
		{
			return new GeneralMailboxIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeGeneralMailboxIdParameter(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox,
			RecipientType.SystemAttendantMailbox,
			RecipientType.SystemMailbox
		};
	}
}
