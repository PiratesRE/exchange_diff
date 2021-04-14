using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class ResubmitRequestSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ResubmitRequestIdentity = new SimpleProviderPropertyDefinition("ResubmitRequestIdentity", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Server = new SimpleProviderPropertyDefinition("Server", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.WriteOnce, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Destination = new SimpleProviderPropertyDefinition("Destination", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.WriteOnce, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition StartTime = new SimpleProviderPropertyDefinition("StartTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition EndTime = new SimpleProviderPropertyDefinition("EndTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CreationTime = new SimpleProviderPropertyDefinition("CreationTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition State = new SimpleProviderPropertyDefinition("State", ExchangeObjectVersion.Exchange2010, typeof(ResubmitRequestState), PropertyDefinitionFlags.WriteOnce, ResubmitRequestState.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition DiagnosticInformation = new SimpleProviderPropertyDefinition("DiagnosticInformation", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
