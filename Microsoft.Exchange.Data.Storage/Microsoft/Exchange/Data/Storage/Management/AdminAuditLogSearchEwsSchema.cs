using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AdminAuditLogSearchEwsSchema : AuditLogSearchBaseEwsSchema
	{
		public static readonly EwsStoreObjectPropertyDefinition Cmdlets = new EwsStoreObjectPropertyDefinition("Cmdlets", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.ReturnOnBind, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.AdminPropertySetId, 1, 26));

		public static readonly EwsStoreObjectPropertyDefinition Parameters = new EwsStoreObjectPropertyDefinition("Parameters", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.AdminPropertySetId, 2, 26));

		public static readonly EwsStoreObjectPropertyDefinition ObjectIds = new EwsStoreObjectPropertyDefinition("ObjectIds", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.ReturnOnBind, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.AdminPropertySetId, 3, 26));

		public static readonly EwsStoreObjectPropertyDefinition UserIds = new EwsStoreObjectPropertyDefinition("UserIds", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.ReturnOnBind, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.AdminPropertySetId, 4, 26));

		public static readonly EwsStoreObjectPropertyDefinition ResolvedUsers = new EwsStoreObjectPropertyDefinition("ResolvedUsers", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.AdminPropertySetId, 5, 26));

		public static readonly EwsStoreObjectPropertyDefinition RedactDatacenterAdmins = new EwsStoreObjectPropertyDefinition("RedactDatacenterAdmins", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.None, false, false, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.AdminPropertySetId, 6, 4));
	}
}
