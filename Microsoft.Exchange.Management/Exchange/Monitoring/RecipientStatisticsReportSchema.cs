using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Monitoring
{
	internal class RecipientStatisticsReportSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectState = new SimpleProviderPropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2010, typeof(ObjectState), PropertyDefinitionFlags.ReadOnly, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ExchangeVersion = new SimpleProviderPropertyDefinition("ExchangeVersion", ExchangeObjectVersion.Exchange2010, typeof(ExchangeObjectVersion), PropertyDefinitionFlags.ReadOnly, ExchangeObjectVersion.Exchange2010, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalNumberOfMailboxes = new SimpleProviderPropertyDefinition("TotalNumberOfMailboxes", ExchangeObjectVersion.Exchange2010, typeof(uint), PropertyDefinitionFlags.PersistDefaultValue, 0U, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TotalNumberOfActiveMailboxes = new SimpleProviderPropertyDefinition("TotalNumberOfActiveMailboxes", ExchangeObjectVersion.Exchange2010, typeof(uint), PropertyDefinitionFlags.PersistDefaultValue, 0U, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumberOfContacts = new SimpleProviderPropertyDefinition("NumberOfContacts", ExchangeObjectVersion.Exchange2010, typeof(uint), PropertyDefinitionFlags.PersistDefaultValue, 0U, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition NumberOfDistributionLists = new SimpleProviderPropertyDefinition("NumberOfDistributionLists", ExchangeObjectVersion.Exchange2010, typeof(uint), PropertyDefinitionFlags.PersistDefaultValue, 0U, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition LastUpdated = new SimpleProviderPropertyDefinition("LastUpdated", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
