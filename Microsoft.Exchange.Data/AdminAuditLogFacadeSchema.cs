using System;

namespace Microsoft.Exchange.Data
{
	internal class AdminAuditLogFacadeSchema : SimpleProviderObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition ObjectModified = new SimpleProviderPropertyDefinition("ObjectModified", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ModifiedObjectResolvedName = new SimpleProviderPropertyDefinition("ModifiedObjectResolvedName", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CmdletName = new SimpleProviderPropertyDefinition("CmdletName", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition CmdletParameters = new SimpleProviderPropertyDefinition("CmdletParameters", ExchangeObjectVersion.Exchange2007, typeof(AdminAuditLogCmdletParameter), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ModifiedProperties = new SimpleProviderPropertyDefinition("ModifiedProperties", ExchangeObjectVersion.Exchange2007, typeof(AdminAuditLogModifiedProperty), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Caller = new SimpleProviderPropertyDefinition("Caller", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Succeeded = new SimpleProviderPropertyDefinition("Succeeded", ExchangeObjectVersion.Exchange2007, typeof(bool?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Error = new SimpleProviderPropertyDefinition("Error", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RunDate = new SimpleProviderPropertyDefinition("RunDate", ExchangeObjectVersion.Exchange2007, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OriginatingServer = new SimpleProviderPropertyDefinition("OriginatingServer", ExchangeObjectVersion.Exchange2007, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
