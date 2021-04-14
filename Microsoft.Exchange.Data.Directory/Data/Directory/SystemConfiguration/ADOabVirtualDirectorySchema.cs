using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADOabVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		public static readonly ADPropertyDefinition PollInterval = new ADPropertyDefinition("PollInterval", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchPollInterval", ADPropertyDefinitionFlags.PersistDefaultValue, 480, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 71582)
		}, null, null);

		public static readonly ADPropertyDefinition OfflineAddressBooks = new ADPropertyDefinition("OfflineAddressBooks", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchOABVirtualDirectoriesBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RequireSSL = new ADPropertyDefinition("RequireSSL", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BasicAuthentication = new ADPropertyDefinition("BasicAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WindowsAuthentication = new ADPropertyDefinition("WindowsAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OAuthAuthentication = new ADPropertyDefinition("OAuthAuthentication", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
