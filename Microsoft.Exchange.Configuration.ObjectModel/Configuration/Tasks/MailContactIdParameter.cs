using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailContactIdParameter : RecipientIdParameter
	{
		public MailContactIdParameter(string identity) : base(identity)
		{
		}

		public MailContactIdParameter()
		{
		}

		public MailContactIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailContactIdParameter(MailContact contact) : base(contact.Id)
		{
		}

		public MailContactIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailContactIdParameter.AllowedRecipientTypes;
			}
		}

		public new static MailContactIdParameter Parse(string identity)
		{
			return new MailContactIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailContact(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.MailContact
		};
	}
}
