using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditLogSearchBaseEwsSchema : ObjectSchema
	{
		public static readonly Guid BasePropertySetId = new Guid("9cff9e83-a0b3-4110-bcd8-617e9ea1e0fe");

		public static readonly Guid MailboxPropertySetId = new Guid("7295f845-69ad-401b-a1ae-13dcadbe4c60");

		public static readonly Guid AdminPropertySetId = new Guid("00914066-1863-4c10-82f1-d52616e61eab");

		public static readonly SimplePropertyDefinition ObjectState = EwsStoreObjectSchema.ObjectState;

		public static readonly EwsStoreObjectPropertyDefinition Identity = new EwsStoreObjectPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(AuditLogSearchId), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 1, 5));

		public static readonly EwsStoreObjectPropertyDefinition Name = new EwsStoreObjectPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 2, 25));

		public static readonly EwsStoreObjectPropertyDefinition CreationTime = new EwsStoreObjectPropertyDefinition("CreationTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.DateTimeCreated);

		public static readonly EwsStoreObjectPropertyDefinition StartDateUtc = new EwsStoreObjectPropertyDefinition("StartDateUtc", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 3, 23));

		public static readonly EwsStoreObjectPropertyDefinition EndDateUtc = new EwsStoreObjectPropertyDefinition("EndDateUtc", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 4, 23));

		public static readonly EwsStoreObjectPropertyDefinition StatusMailRecipients = new EwsStoreObjectPropertyDefinition("StatusMailRecipients", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 5, 26));

		public static readonly EwsStoreObjectPropertyDefinition CreatedByEx = new EwsStoreObjectPropertyDefinition("CreatedByEx", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 6, 2));

		public static readonly EwsStoreObjectPropertyDefinition CreatedBy = new EwsStoreObjectPropertyDefinition("CreatedBy", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, 7, 25));

		public static readonly EwsStoreObjectPropertyDefinition ExternalAccess = new EwsStoreObjectPropertyDefinition("ExternalAccess", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.MailboxPropertySetId, 4, 25));

		public static readonly EwsStoreObjectPropertyDefinition Type = new EwsStoreObjectPropertyDefinition("Type", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, null, new ExtendedPropertyDefinition(AuditLogSearchBaseEwsSchema.BasePropertySetId, "Type", 25));

		public static readonly EwsStoreObjectPropertyDefinition ItemClass = new EwsStoreObjectPropertyDefinition("ItemClass", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.ItemClass);

		public static readonly EwsStoreObjectPropertyDefinition EwsItemId = new EwsStoreObjectPropertyDefinition("EwsItemId", ExchangeObjectVersion.Exchange2010, typeof(EwsStoreObjectId), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.Id);
	}
}
