using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class AdminAuditLogSearchItemSchema
	{
		public const string MessageClass = "IPM.AuditLogSearch.Admin";

		public const string FolderName = "AdminAuditLogSearch";

		public static readonly StorePropertyDefinition Cmdlets = GuidIdPropertyDefinition.CreateCustom("Cmdlets", typeof(string[]), AuditLogSearchItemSchema.AdminPropertyGuid, 1, PropertyFlags.None);

		public static readonly StorePropertyDefinition Parameters = GuidIdPropertyDefinition.CreateCustom("Parameters", typeof(string[]), AuditLogSearchItemSchema.AdminPropertyGuid, 2, PropertyFlags.None);

		public static readonly StorePropertyDefinition ObjectIds = GuidIdPropertyDefinition.CreateCustom("ObjectIds", typeof(string[]), AuditLogSearchItemSchema.AdminPropertyGuid, 3, PropertyFlags.None);

		public static readonly StorePropertyDefinition RawUserIds = GuidIdPropertyDefinition.CreateCustom("RawUserIds", typeof(string[]), AuditLogSearchItemSchema.AdminPropertyGuid, 4, PropertyFlags.None);

		public static readonly StorePropertyDefinition ResolvedUsers = GuidIdPropertyDefinition.CreateCustom("ResolvedUsers", typeof(string[]), AuditLogSearchItemSchema.AdminPropertyGuid, 5, PropertyFlags.None);

		public static readonly StorePropertyDefinition RedactDatacenterAdmins = GuidIdPropertyDefinition.CreateCustom("RedactDatacenterAdmins", typeof(bool), AuditLogSearchItemSchema.AdminPropertyGuid, 6, PropertyFlags.None);
	}
}
