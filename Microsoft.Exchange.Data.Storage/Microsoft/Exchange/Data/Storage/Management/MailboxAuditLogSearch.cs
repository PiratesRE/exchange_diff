using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxAuditLogSearch : AuditLogSearchBase
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxAuditLogSearch.schema;
			}
		}

		internal override SearchFilter ItemClassFilter
		{
			get
			{
				return new SearchFilter.ContainsSubstring(ItemSchema.ItemClass, "IPM.AuditLogSearch.Mailbox", 1, 0);
			}
		}

		public MultiValuedProperty<ADObjectId> Mailboxes
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailboxAuditLogSearchEwsSchema.MailboxIds];
			}
			set
			{
				this[MailboxAuditLogSearchEwsSchema.MailboxIds] = value;
			}
		}

		public MultiValuedProperty<string> LogonTypes
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxAuditLogSearchEwsSchema.LogonTypeStrings];
			}
			set
			{
				this[MailboxAuditLogSearchEwsSchema.LogonTypeStrings] = value;
			}
		}

		public MultiValuedProperty<string> Operations
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxAuditLogSearchEwsSchema.Operations];
			}
			set
			{
				this[MailboxAuditLogSearchEwsSchema.Operations] = value;
			}
		}

		public bool? ShowDetails
		{
			get
			{
				return (bool?)this[MailboxAuditLogSearchEwsSchema.ShowDetails];
			}
			set
			{
				this[MailboxAuditLogSearchEwsSchema.ShowDetails] = value;
			}
		}

		internal override string ItemClass
		{
			get
			{
				return "IPM.AuditLogSearch.Mailbox";
			}
		}

		private const string ItemClassPrefix = "IPM.AuditLogSearch.Mailbox";

		private static ObjectSchema schema = new MailboxAuditLogSearchEwsSchema();
	}
}
