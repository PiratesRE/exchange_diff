using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ModeratorIDParameter : RecipientIdParameter
	{
		public ModeratorIDParameter(string identity) : base(identity)
		{
		}

		public ModeratorIDParameter()
		{
		}

		public ModeratorIDParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ModeratorIDParameter(MailUser user) : base(user.Id)
		{
		}

		public ModeratorIDParameter(MailContact contact) : base(contact.Id)
		{
		}

		public ModeratorIDParameter(Mailbox user) : base(user.Id)
		{
		}

		public ModeratorIDParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return ModeratorIDParameter.AllowedRecipientTypes;
			}
		}

		public new static ModeratorIDParameter Parse(string identity)
		{
			return new ModeratorIDParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxOrMailUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = ADRecipient.AllowedModeratorsRecipientTypes;
	}
}
