using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class ContactIdParameter : RecipientIdParameter
	{
		public ContactIdParameter(string identity) : base(identity)
		{
		}

		public ContactIdParameter()
		{
		}

		public ContactIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ContactIdParameter(Contact contact) : base(contact.Id)
		{
		}

		public ContactIdParameter(MailContact contact) : base(contact.Id)
		{
		}

		public ContactIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return ContactIdParameter.AllowedRecipientTypes;
			}
		}

		public new static ContactIdParameter Parse(string identity)
		{
			return new ContactIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailEnabledContact(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Contact,
			RecipientType.MailContact
		};
	}
}
