using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class AuditLogSearchItemSchema
	{
		public static readonly Guid BasePropertyGuid = new Guid("9cff9e83-a0b3-4110-bcd8-617e9ea1e0fe");

		public static readonly Guid MailboxPropertyGuid = new Guid("7295f845-69ad-401b-a1ae-13dcadbe4c60");

		public static readonly Guid AdminPropertyGuid = new Guid("00914066-1863-4c10-82f1-d52616e61eab");

		public static readonly StorePropertyDefinition Identity = GuidIdPropertyDefinition.CreateCustom("Identity", typeof(Guid), AuditLogSearchItemSchema.BasePropertyGuid, 1, PropertyFlags.None);

		public static readonly StorePropertyDefinition Name = GuidIdPropertyDefinition.CreateCustom("Name", typeof(string), AuditLogSearchItemSchema.BasePropertyGuid, 2, PropertyFlags.None);

		public static readonly StorePropertyDefinition StartDate = GuidIdPropertyDefinition.CreateCustom("StartDate", typeof(ExDateTime), AuditLogSearchItemSchema.BasePropertyGuid, 3, PropertyFlags.None);

		public static readonly StorePropertyDefinition EndDate = GuidIdPropertyDefinition.CreateCustom("EndDate", typeof(ExDateTime), AuditLogSearchItemSchema.BasePropertyGuid, 4, PropertyFlags.None);

		public static readonly StorePropertyDefinition StatusMailRecipients = GuidIdPropertyDefinition.CreateCustom("StatusMailRecipients", typeof(string[]), AuditLogSearchItemSchema.BasePropertyGuid, 5, PropertyFlags.None);

		public static readonly StorePropertyDefinition CreatedByEx = GuidIdPropertyDefinition.CreateCustom("CreatedByEx", typeof(byte[]), AuditLogSearchItemSchema.BasePropertyGuid, 6, PropertyFlags.None);

		public static readonly StorePropertyDefinition CreatedBy = GuidIdPropertyDefinition.CreateCustom("CreatedBy", typeof(string), AuditLogSearchItemSchema.BasePropertyGuid, 7, PropertyFlags.None);

		public static readonly StorePropertyDefinition ExternalAccess = GuidIdPropertyDefinition.CreateCustom("ExternalAccess", typeof(string), AuditLogSearchItemSchema.MailboxPropertyGuid, 4, PropertyFlags.None);
	}
}
