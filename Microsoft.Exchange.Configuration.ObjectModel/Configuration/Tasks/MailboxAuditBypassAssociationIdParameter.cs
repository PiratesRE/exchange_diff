using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxAuditBypassAssociationIdParameter : RecipientIdParameter
	{
		public MailboxAuditBypassAssociationIdParameter()
		{
		}

		public MailboxAuditBypassAssociationIdParameter(string identity) : base(identity)
		{
		}

		public MailboxAuditBypassAssociationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxAuditBypassAssociationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public new static MailboxAuditBypassAssociationIdParameter Parse(string identity)
		{
			return new MailboxAuditBypassAssociationIdParameter(identity);
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailboxAuditBypassAssociationIdParameter.AllowedRecipientTypes;
			}
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.Computer
		};
	}
}
