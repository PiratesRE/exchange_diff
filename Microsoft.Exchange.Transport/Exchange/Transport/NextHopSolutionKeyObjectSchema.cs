using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NextHopSolutionKeyObjectSchema : ObjectSchema
	{
		public static readonly SimpleProviderPropertyDefinition Version = new SimpleProviderPropertyDefinition("Version", ExchangeObjectVersion.Exchange2010, typeof(ExchangeObjectVersion), PropertyDefinitionFlags.ReadOnly, ExchangeObjectVersion.Exchange2010, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ObjectState = new SimpleProviderPropertyDefinition("ObjectState", ExchangeObjectVersion.Exchange2010, typeof(ObjectState), PropertyDefinitionFlags.ReadOnly, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Id = new SimpleProviderPropertyDefinition("Id", ExchangeObjectVersion.Exchange2010, typeof(NextHopSolutionKeyId), PropertyDefinitionFlags.ReadOnly, NextHopSolutionKeyId.DefaultNextHopSolutionKeyId, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TlsDomain = new SimpleProviderPropertyDefinition("TlsDomain", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.WriteOnce, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition IsLocalDeliveryGroupRelay = new SimpleProviderPropertyDefinition("IsLocalDeliveryGroupRelay", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.WriteOnce, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition TlsAuthLevel = new SimpleProviderPropertyDefinition("TlsAuthLevel", ExchangeObjectVersion.Exchange2010, typeof(RequiredTlsAuthLevel?), PropertyDefinitionFlags.WriteOnce, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition RiskLevel = new SimpleProviderPropertyDefinition("RiskLevel", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OutboundIPPool = new SimpleProviderPropertyDefinition("OutboundIPPool", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition OverrideSource = new SimpleProviderPropertyDefinition("OverrideSource", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.PersistDefaultValue, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
