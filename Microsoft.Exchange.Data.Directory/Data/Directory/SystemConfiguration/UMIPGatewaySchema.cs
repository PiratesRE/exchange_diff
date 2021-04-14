using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class UMIPGatewaySchema : ADConfigurationObjectSchema
	{
		private static object IPAddressFamilyGetter(IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[UMIPGatewaySchema.UMIPGatewaySet];
			UMIPGatewayFlags2 umipgatewayFlags = (UMIPGatewayFlags2)propertyBag[UMIPGatewaySchema.UMIPGatewaySet2];
			bool flag = (num & 4) == 4;
			bool flag2 = (umipgatewayFlags & UMIPGatewayFlags2.IPv4Enabled) == UMIPGatewayFlags2.IPv4Enabled;
			if (flag2 && flag)
			{
				return Microsoft.Exchange.Data.Directory.IPAddressFamily.Any;
			}
			if (flag)
			{
				return Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv6Only;
			}
			if (flag2)
			{
				return Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv4Only;
			}
			ExAssert.RetailAssert(false, "At least one of SIPFEServerConfigurationSchema IPv4Enabled and IPv6Enabled must be set");
			return (IPAddressFamily)(-1);
		}

		private static void IPAddressFamilySetter(object value, IPropertyBag propertyBag)
		{
			int num = (int)propertyBag[UMIPGatewaySchema.UMIPGatewaySet];
			UMIPGatewayFlags2 umipgatewayFlags = (UMIPGatewayFlags2)propertyBag[UMIPGatewaySchema.UMIPGatewaySet2];
			IPAddressFamily ipaddressFamily = (IPAddressFamily)value;
			if (ipaddressFamily == Microsoft.Exchange.Data.Directory.IPAddressFamily.Any)
			{
				num |= 4;
				umipgatewayFlags |= UMIPGatewayFlags2.IPv4Enabled;
			}
			else if (ipaddressFamily == Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv6Only)
			{
				num |= 4;
				umipgatewayFlags &= ~UMIPGatewayFlags2.IPv4Enabled;
			}
			else if (ipaddressFamily == Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv4Only)
			{
				num &= -5;
				umipgatewayFlags |= UMIPGatewayFlags2.IPv4Enabled;
			}
			else
			{
				ExAssert.RetailAssert(false, "IPAddressFamily set value must be Any, IPv6Only, or IPv4Only");
			}
			propertyBag[UMIPGatewaySchema.UMIPGatewaySet] = num;
			propertyBag[UMIPGatewaySchema.UMIPGatewaySet2] = (int)umipgatewayFlags;
		}

		private const int SimulatorMask = 1;

		private const int DelayedSourcePartyInfoEnabledMask = 2;

		private const int IPv6EnabledMask = 4;

		public static readonly ADPropertyDefinition Address = new ADPropertyDefinition("Address", ExchangeObjectVersion.Exchange2007, typeof(UMSmartHost), "msExchUMIPGatewayAddress", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMIPGatewaySet = new ADPropertyDefinition("UMIPGatewaySet", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMIPGatewayFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMIPGatewaySet2 = new ADPropertyDefinition("UMIPGatewaySet2", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchUMIPGatewayFlags2", ADPropertyDefinitionFlags.PersistDefaultValue, -1, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Simulator = new ADPropertyDefinition("Simulator", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMIPGatewaySchema.UMIPGatewaySet
		}, null, ADObject.FlagGetterDelegate(1, UMIPGatewaySchema.UMIPGatewaySet), ADObject.FlagSetterDelegate(1, UMIPGatewaySchema.UMIPGatewaySet), null, null);

		public static readonly ADPropertyDefinition DelayedSourcePartyInfoEnabled = new ADPropertyDefinition("DelayedSourcePartyInfoEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMIPGatewaySchema.UMIPGatewaySet
		}, null, ADObject.FlagGetterDelegate(2, UMIPGatewaySchema.UMIPGatewaySet), ADObject.FlagSetterDelegate(2, UMIPGatewaySchema.UMIPGatewaySet), null, null);

		public static readonly ADPropertyDefinition MessageWaitingIndicatorAllowed = new ADPropertyDefinition("MessageWaitingIndicatorAllowed", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMIPGatewaySchema.UMIPGatewaySet2
		}, null, ADObject.FlagGetterDelegate(1, UMIPGatewaySchema.UMIPGatewaySet2), ADObject.FlagSetterDelegate(1, UMIPGatewaySchema.UMIPGatewaySet2), null, null);

		public static readonly ADPropertyDefinition IPAddressFamily = new ADPropertyDefinition("IPAddressFamily", ExchangeObjectVersion.Exchange2010, typeof(IPAddressFamily), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv4Only, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			UMIPGatewaySchema.UMIPGatewaySet,
			UMIPGatewaySchema.UMIPGatewaySet2
		}, null, new GetterDelegate(UMIPGatewaySchema.IPAddressFamilyGetter), new SetterDelegate(UMIPGatewaySchema.IPAddressFamilySetter), null, null);

		public static readonly ADPropertyDefinition Port = new ADPropertyDefinition("Port", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchUMIPGatewayPort", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65535)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition OutcallsAllowed = new ADPropertyDefinition("OutcallsAllowed", ExchangeObjectVersion.Exchange2007, typeof(bool), "msExchUMOutcallsAllowed", ADPropertyDefinitionFlags.None, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Status = new ADPropertyDefinition("Status", ExchangeObjectVersion.Exchange2007, typeof(GatewayStatus), "msExchUMIPGatewayStatus", ADPropertyDefinitionFlags.None, GatewayStatus.Enabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HuntGroups = new ADPropertyDefinition("HuntGroups", ExchangeObjectVersion.Exchange2007, typeof(UMHuntGroup), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition GlobalCallRoutingScheme = new ADPropertyDefinition("GlobalCallRoutingScheme", ExchangeObjectVersion.Exchange2010, typeof(UMGlobalCallRoutingScheme), "msExchUMGlobalCallRoutingScheme", ADPropertyDefinitionFlags.PersistDefaultValue, UMGlobalCallRoutingScheme.None, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMGlobalCallRoutingScheme))
		}, PropertyDefinitionConstraint.None, null, null);
	}
}
