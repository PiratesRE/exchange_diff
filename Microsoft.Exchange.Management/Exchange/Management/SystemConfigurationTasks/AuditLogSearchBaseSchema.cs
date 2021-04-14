using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuditLogSearchBaseSchema : ObjectSchema
	{
		public static readonly ProviderPropertyDefinition ObjectState = UserConfigurationObjectSchema.ObjectState;

		public static readonly ProviderPropertyDefinition ExchangeVersion = UserConfigurationObjectSchema.ExchangeVersion;

		public static readonly ProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(AuditLogSearchId), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Name = new SimpleProviderPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255)
		});

		public static readonly ProviderPropertyDefinition StartDateUtc = new SimpleProviderPropertyDefinition("StartDateUtc", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition EndDateUtc = new SimpleProviderPropertyDefinition("EndDateUtc", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StatusMailRecipients = new SimpleProviderPropertyDefinition("StatusMailRecipients", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CreatedByEx = new SimpleProviderPropertyDefinition("CreatedByEx", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CreatedBy = new SimpleProviderPropertyDefinition("CreatedBy", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ExternalAccess = new SimpleProviderPropertyDefinition("ExternalAccess", ExchangeObjectVersion.Exchange2010, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
