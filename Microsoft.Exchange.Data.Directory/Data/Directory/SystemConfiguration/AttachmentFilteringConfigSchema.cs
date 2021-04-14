using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class AttachmentFilteringConfigSchema : ADConfigurationObjectSchema
	{
		public static readonly ADPropertyDefinition RejectResponse = new ADPropertyDefinition("RejectResponse", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAttachmentFilteringRejectResponse", ADPropertyDefinitionFlags.None, string.Empty, AttachmentFilteringDefinitions.RejectResponseConstraints, AttachmentFilteringDefinitions.RejectResponseConstraints, null, null);

		public static readonly ADPropertyDefinition AdminMessage = new ADPropertyDefinition("AdminMessage", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAttachmentFilteringAdminMessage", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FilterAction = new ADPropertyDefinition("FilterAction", ExchangeObjectVersion.Exchange2007, typeof(FilterActions), "msExchAttachmentFilteringFilterAction", ADPropertyDefinitionFlags.None, FilterActions.Strip, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AttachmentNames = new ADPropertyDefinition("AttachmentNames", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchAttachmentFilteringAttachmentNames", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExceptionConnectors = new ADPropertyDefinition("ExceptionConnectors", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchAttachmentFilteringExceptionConnectorsLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
