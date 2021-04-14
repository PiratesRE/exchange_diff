using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ProvisioningReconciliationConfigSchema : ADConfigurationObjectSchema
	{
		internal static readonly ADPropertyDefinition ReconciliationCookies = new ADPropertyDefinition("ReconciliationCookies", ExchangeObjectVersion.Exchange2007, typeof(ReconciliationCookie), "msExchReconciliationCookies", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReconciliationCookiesForNextCycle = new ADPropertyDefinition("ReconciliationCookiesForNextCycle", ExchangeObjectVersion.Exchange2007, typeof(ReconciliationCookie), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReconciliationCookieForCurrentCycle = new ADPropertyDefinition("ReconciliationCookieForCurrentCycle", ExchangeObjectVersion.Exchange2007, typeof(ReconciliationCookie), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
