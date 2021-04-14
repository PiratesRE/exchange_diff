using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class UnpublishedObjectSchema : ADObjectSchema
	{
		public static readonly HygienePropertyDefinition TenantIdProp = CommonSyncProperties.TenantIdProp;

		public static readonly HygienePropertyDefinition ObjectIdProp = CommonSyncProperties.ObjectIdProp;

		public static readonly HygienePropertyDefinition ObjectTypeProp = CommonSyncProperties.ObjectTypeProp;

		public static readonly HygienePropertyDefinition ServiceInstanceProp = CommonSyncProperties.ServiceInstanceProp;

		public static readonly HygienePropertyDefinition CreatedDateProp = new HygienePropertyDefinition("CreatedDate", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LastRetriedDateProp = new HygienePropertyDefinition("LastRetriedDate", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ErrorMessageProp = new HygienePropertyDefinition("ErrorMessage", typeof(string));
	}
}
