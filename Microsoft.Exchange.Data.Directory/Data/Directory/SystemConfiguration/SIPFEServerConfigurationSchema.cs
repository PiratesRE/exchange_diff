using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class SIPFEServerConfigurationSchema : ADConfigurationObjectSchema
	{
		private static object IPAddressFamilyGetter(IPropertyBag propertyBag)
		{
			SIPFEServerConfigurationSchema.SIPFEServerSetFlags sipfeserverSetFlags = (SIPFEServerConfigurationSchema.SIPFEServerSetFlags)propertyBag[SIPFEServerConfigurationSchema.SIPFEServerSet];
			bool flag = (sipfeserverSetFlags & SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv4Enabled) == SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv4Enabled;
			bool flag2 = (sipfeserverSetFlags & SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv6Enabled) == SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv6Enabled;
			if (flag && flag2)
			{
				return Microsoft.Exchange.Data.Directory.IPAddressFamily.Any;
			}
			if (flag2)
			{
				return Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv6Only;
			}
			if (flag)
			{
				return Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv4Only;
			}
			ExAssert.RetailAssert(false, "At least one of SIPFEServerConfigurationSchema IPv4Enabled and IPv6Enabled must be set");
			return (IPAddressFamily)(-1);
		}

		private static void IPAddressFamilySetter(object value, IPropertyBag propertyBag)
		{
			SIPFEServerConfigurationSchema.SIPFEServerSetFlags sipfeserverSetFlags = (SIPFEServerConfigurationSchema.SIPFEServerSetFlags)propertyBag[SIPFEServerConfigurationSchema.SIPFEServerSet];
			IPAddressFamily ipaddressFamily = (IPAddressFamily)value;
			if (ipaddressFamily == Microsoft.Exchange.Data.Directory.IPAddressFamily.Any)
			{
				sipfeserverSetFlags |= SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv4Enabled;
				sipfeserverSetFlags |= SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv6Enabled;
			}
			else if (ipaddressFamily == Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv6Only)
			{
				sipfeserverSetFlags &= ~SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv4Enabled;
				sipfeserverSetFlags |= SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv6Enabled;
			}
			else if (ipaddressFamily == Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv4Only)
			{
				sipfeserverSetFlags |= SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv4Enabled;
				sipfeserverSetFlags &= ~SIPFEServerConfigurationSchema.SIPFEServerSetFlags.IPv6Enabled;
			}
			else
			{
				ExAssert.RetailAssert(false, "IPAddressFamily set value must be Any, IPv6Only, or IPv4Only");
			}
			propertyBag[SIPFEServerConfigurationSchema.SIPFEServerSet] = (int)sipfeserverSetFlags;
		}

		public static readonly ADPropertyDefinition CurrentServerRole = new ADPropertyDefinition("CurrentServerRole", ExchangeObjectVersion.Exchange2010, typeof(ServerRole), "msExchCurrentServerRoles", ADPropertyDefinitionFlags.PersistDefaultValue, ServerRole.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VersionNumber = new ADPropertyDefinition("VersionNumber", ExchangeObjectVersion.Exchange2010, typeof(int), "versionNumber", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition NetworkAddress = new ADPropertyDefinition("NetworkAddress", ExchangeObjectVersion.Exchange2010, typeof(NetworkAddress), "networkAddress", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DialPlans = new ADPropertyDefinition("DialPlans", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchUMServerDialPlanLink", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SipTcpListeningPort = new ADPropertyDefinition("SipTcpListeningPort", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchUMTcpListeningPort", ADPropertyDefinitionFlags.PersistDefaultValue, 5060, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 65535)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 65535)
		}, null, null);

		public static readonly ADPropertyDefinition SipTlsListeningPort = new ADPropertyDefinition("SipTlsListeningPort", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchUMTlsListeningPort", ADPropertyDefinitionFlags.PersistDefaultValue, 5061, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 65535)
		}, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 65535)
		}, null, null);

		public static readonly ADPropertyDefinition UMStartupMode = new ADPropertyDefinition("UMStartupMode", ExchangeObjectVersion.Exchange2010, typeof(UMStartupMode), "msExchUMStartupMode", ADPropertyDefinitionFlags.None, Microsoft.Exchange.Data.Directory.SystemConfiguration.UMStartupMode.TCP, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(UMStartupMode))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxCallsAllowed = new ADPropertyDefinition("MaxCallsAllowed", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchUMMaximumCallsAllowed", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedNullableValueConstraint<int>(0, 200)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalServiceFqdn = new ADPropertyDefinition("ExternalServiceFqdn", ExchangeObjectVersion.Exchange2010, typeof(UMSmartHost), "msExchUMLoadBalancerFqdn", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExternalHostFqdn = new ADPropertyDefinition("ExternalHostFqdn", ExchangeObjectVersion.Exchange2010, typeof(UMSmartHost), "msExchShadowInfo", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Status = new ADPropertyDefinition("Status", ExchangeObjectVersion.Exchange2010, typeof(ServerStatus), "msExchUMServerStatus", ADPropertyDefinitionFlags.None, ServerStatus.Enabled, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ServerStatus))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMPodRedirectTemplate = new ADPropertyDefinition("UMPodRedirectTemplate", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMSiteRedirectTarget", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMForwardingAddressTemplate = new ADPropertyDefinition("UMForwardingAddressTemplate", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMForwardingAddressTemplate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition UMCertificateThumbprint = new ADPropertyDefinition("UMCertificateThumbprint", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchUMCertificateThumbprint", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SIPFEServerSet = new ADPropertyDefinition("SIPFEServerSet", ExchangeObjectVersion.Exchange2012, typeof(int), "msExchUMEnabledFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 7, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IPAddressFamilyConfigurable = new ADPropertyDefinition("IPAddressFamilyConfigurable", ExchangeObjectVersion.Exchange2012, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SIPFEServerConfigurationSchema.SIPFEServerSet
		}, null, ADObject.FlagGetterDelegate(1, SIPFEServerConfigurationSchema.SIPFEServerSet), ADObject.FlagSetterDelegate(1, SIPFEServerConfigurationSchema.SIPFEServerSet), null, null);

		public static readonly ADPropertyDefinition IPAddressFamily = new ADPropertyDefinition("IPAddressFamily", ExchangeObjectVersion.Exchange2012, typeof(IPAddressFamily), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.IPAddressFamily.IPv4Only, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			SIPFEServerConfigurationSchema.SIPFEServerSet
		}, null, new GetterDelegate(SIPFEServerConfigurationSchema.IPAddressFamilyGetter), new SetterDelegate(SIPFEServerConfigurationSchema.IPAddressFamilySetter), null, null);

		[Flags]
		private enum SIPFEServerSetFlags
		{
			IPAddressFamilyConfigurable = 1,
			IPv4Enabled = 2,
			IPv6Enabled = 4,
			Default = 7
		}
	}
}
