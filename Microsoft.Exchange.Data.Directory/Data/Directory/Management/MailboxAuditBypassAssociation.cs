using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class MailboxAuditBypassAssociation : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MailboxAuditBypassAssociation.schema;
			}
		}

		public MailboxAuditBypassAssociation()
		{
		}

		public MailboxAuditBypassAssociation(ADRecipient dataObject) : base(dataObject)
		{
		}

		internal static MailboxAuditBypassAssociation FromDataObject(ADRecipient dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new MailboxAuditBypassAssociation(dataObject);
		}

		public ADObjectId ObjectId
		{
			get
			{
				return (ADObjectId)this[MailboxAuditBypassAssociationSchema.ObjectId];
			}
		}

		public bool AuditBypassEnabled
		{
			get
			{
				return (bool)this[MailboxAuditBypassAssociationSchema.AuditBypassEnabled];
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		private static MailboxAuditBypassAssociationSchema schema = ObjectSchema.GetInstance<MailboxAuditBypassAssociationSchema>();
	}
}
