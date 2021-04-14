using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExBindingStoreObjectSchema : EwsStoreObjectSchema
	{
		public static readonly EwsStoreObjectPropertyDefinition PolicyId = EwsStoreObjectSchema.AlternativeId;

		public static readonly EwsStoreObjectPropertyDefinition Name = new EwsStoreObjectPropertyDefinition("Name", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, ExBindingStoreObjectExtendedStoreSchema.Name);

		public static readonly EwsStoreObjectPropertyDefinition MasterIdentity = new EwsStoreObjectPropertyDefinition("MasterIdentity", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, ExBindingStoreObjectExtendedStoreSchema.MasterIdentity);

		public static readonly EwsStoreObjectPropertyDefinition Workload = new EwsStoreObjectPropertyDefinition("Workload", ExchangeObjectVersion.Exchange2012, typeof(Workload), PropertyDefinitionFlags.PersistDefaultValue, Microsoft.Office.CompliancePolicy.PolicyConfiguration.Workload.Exchange, Microsoft.Office.CompliancePolicy.PolicyConfiguration.Workload.Exchange, ExBindingStoreObjectExtendedStoreSchema.Workload);

		public static readonly EwsStoreObjectPropertyDefinition PolicyVersion = new EwsStoreObjectPropertyDefinition("PolicyVersion", ExchangeObjectVersion.Exchange2012, typeof(Guid), PropertyDefinitionFlags.None, Guid.Empty, Guid.Empty, ExBindingStoreObjectExtendedStoreSchema.PolicyVersion);

		public static readonly EwsStoreObjectPropertyDefinition RawAppliedScopes = new EwsStoreObjectPropertyDefinition("RawAppliedScopes", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.ReturnOnBind, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition WhenCreated = new EwsStoreObjectPropertyDefinition("WhenCreated", ExchangeObjectVersion.Exchange2012, typeof(DateTime), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.DateTimeCreated);

		public static readonly EwsStoreObjectPropertyDefinition WhenChanged = new EwsStoreObjectPropertyDefinition("WhenChanged", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.ReadOnly, null, null, ItemSchema.LastModifiedTime);
	}
}
