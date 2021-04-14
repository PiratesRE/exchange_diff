using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExchangeRpcClientAccessSchema : ADConfigurationObjectSchema
	{
		private const int MaximumConnectionValue = 65536;

		public static readonly ADPropertyDefinition Server = new ADPropertyDefinition("Server", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(ExchangeRpcClientAccess.ServerGetter), null, null, null);

		public static readonly ADPropertyDefinition MaximumConnections = new ADPropertyDefinition("MaximumConnections", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchMaxIncomingConnections", ADPropertyDefinitionFlags.PersistDefaultValue, 65536, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65536)
		}, null, null);

		public static readonly ADPropertyDefinition IsEncryptionRequired = new ADPropertyDefinition("IsEncryptionRequired", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchEncryptionRequired", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BlockedClientVersions = new ADPropertyDefinition("BlockedClientVersions", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchBlockedClientVersions", ADPropertyDefinitionFlags.None, string.Empty, new PropertyDefinitionConstraint[]
		{
			new DelegateConstraint(new ValidationDelegate(ConstraintDelegates.ValidateClientVersion))
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
