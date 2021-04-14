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
	public class MailboxIdParameter : RecipientIdParameter
	{
		public MailboxIdParameter(string identity) : base(identity)
		{
		}

		public MailboxIdParameter()
		{
		}

		public MailboxIdParameter(MailboxEntry storeMailboxEntry) : this((storeMailboxEntry == null) ? null : storeMailboxEntry.Identity)
		{
		}

		public MailboxIdParameter(MailboxId storeMailboxId) : this((null == storeMailboxId) ? null : ((Guid.Empty == storeMailboxId.MailboxGuid) ? storeMailboxId.MailboxExchangeLegacyDn : storeMailboxId.MailboxGuid.ToString()))
		{
		}

		public MailboxIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxIdParameter(Mailbox mailbox) : base(mailbox)
		{
		}

		public MailboxIdParameter(ConsumerMailbox mailbox) : base(mailbox)
		{
		}

		public MailboxIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailboxIdParameter.AllowedRecipientTypes;
			}
		}

		public new static MailboxIdParameter Parse(string identity)
		{
			return new MailboxIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox
		};
	}
}
