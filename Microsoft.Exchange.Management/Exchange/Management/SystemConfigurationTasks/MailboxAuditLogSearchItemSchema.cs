using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class MailboxAuditLogSearchItemSchema
	{
		public const string MessageClass = "IPM.AuditLogSearch.Mailbox";

		public const string FolderName = "MailboxAuditLogSearch";

		public static readonly StorePropertyDefinition MailboxIds = GuidIdPropertyDefinition.CreateCustom("MailboxIds", typeof(byte[][]), AuditLogSearchItemSchema.MailboxPropertyGuid, 1, PropertyFlags.None);

		public static readonly StorePropertyDefinition LogonTypeStrings = GuidIdPropertyDefinition.CreateCustom("LogonTypeStrings", typeof(string[]), AuditLogSearchItemSchema.MailboxPropertyGuid, 2, PropertyFlags.None);

		public static readonly StorePropertyDefinition ShowDetails = GuidIdPropertyDefinition.CreateCustom("ShowDetails", typeof(bool), AuditLogSearchItemSchema.MailboxPropertyGuid, 3, PropertyFlags.None);

		public static readonly StorePropertyDefinition Operations = GuidIdPropertyDefinition.CreateCustom("Operations", typeof(string[]), AuditLogSearchItemSchema.MailboxPropertyGuid, 4, PropertyFlags.None);
	}
}
