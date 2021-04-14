using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AcceptedDomainSchema : ADConfigurationObjectSchema
	{
		internal static GetterDelegate AuthenticationTypeGetterDelegate()
		{
			return delegate(IPropertyBag bag)
			{
				if (!AcceptedDomainSchema.IsDomainAssociatedWithLiveNamespace(bag))
				{
					return null;
				}
				return AcceptedDomainSchema.GetRawAuthenticationType(bag);
			};
		}

		internal static GetterDelegate RawAuthenticationTypeGetterDelegate()
		{
			return (IPropertyBag bag) => AcceptedDomainSchema.GetRawAuthenticationType(bag);
		}

		private static AuthenticationType GetRawAuthenticationType(IPropertyBag bag)
		{
			int num = (int)bag[AcceptedDomainSchema.AcceptedDomainFlags];
			int num2 = 32;
			if (0 == (num & num2))
			{
				return Microsoft.Exchange.Data.Directory.AuthenticationType.Managed;
			}
			return Microsoft.Exchange.Data.Directory.AuthenticationType.Federated;
		}

		internal static SetterDelegate RawAuthenticationTypeSetterDelegate()
		{
			return delegate(object value, IPropertyBag bag)
			{
				ADPropertyDefinition acceptedDomainFlags = AcceptedDomainSchema.AcceptedDomainFlags;
				int num = 32;
				int num2 = (int)bag[acceptedDomainFlags];
				bag[acceptedDomainFlags] = (((AuthenticationType)value == Microsoft.Exchange.Data.Directory.AuthenticationType.Federated) ? (num2 | num) : (num2 & ~num));
			};
		}

		internal static GetterDelegate LiveIdInstanceTypeGetterDelegate()
		{
			return delegate(IPropertyBag bag)
			{
				if (!AcceptedDomainSchema.IsDomainAssociatedWithLiveNamespace(bag))
				{
					return null;
				}
				return AcceptedDomainSchema.GetRawLiveIdInstanceType(bag);
			};
		}

		internal static GetterDelegate RawLiveIdInstanceTypeGetterDelegate()
		{
			return (IPropertyBag bag) => AcceptedDomainSchema.GetRawLiveIdInstanceType(bag);
		}

		private static LiveIdInstanceType GetRawLiveIdInstanceType(IPropertyBag bag)
		{
			int num = (int)bag[AcceptedDomainSchema.AcceptedDomainFlags];
			int num2 = 128;
			if (0 == (num & num2))
			{
				return Microsoft.Exchange.Data.Directory.LiveIdInstanceType.Consumer;
			}
			return Microsoft.Exchange.Data.Directory.LiveIdInstanceType.Business;
		}

		internal static SetterDelegate RawLiveIdInstanceTypeSetterDelegate()
		{
			return delegate(object value, IPropertyBag bag)
			{
				ADPropertyDefinition acceptedDomainFlags = AcceptedDomainSchema.AcceptedDomainFlags;
				int num = 128;
				int num2 = (int)bag[acceptedDomainFlags];
				bag[acceptedDomainFlags] = (((LiveIdInstanceType)value == Microsoft.Exchange.Data.Directory.LiveIdInstanceType.Business) ? (num2 | num) : (num2 & ~num));
			};
		}

		internal static bool IsDomainAssociatedWithLiveNamespace(IPropertyBag bag)
		{
			if (!Globals.IsMicrosoftHostedOnly)
			{
				return false;
			}
			OrganizationId a = (OrganizationId)bag[ADObjectSchema.OrganizationId];
			return !(a == null) && !(a == Microsoft.Exchange.Data.Directory.OrganizationId.ForestWideOrgId);
		}

		internal const int AcceptedDomainTypeShift = 0;

		internal const int AcceptedDomainTypeLength = 2;

		internal const int DefaultShift = 2;

		internal const int AddressBookEnabledShift = 3;

		internal const int X400AddressTypeShift = 4;

		internal const int AuthenticationTypeShift = 5;

		internal const int PendingRemovalShift = 6;

		internal const int LiveIdInstanceTypeShift = 7;

		internal const int OutboundOnlyShift = 8;

		internal const int DefaultFederatedDomain = 9;

		internal const int EnableNego2AuthBit = 10;

		internal const int CoexistenceShift = 11;

		internal const int DualProvisioningEnabledShift = 12;

		internal const int PendingFederatedAccountNamespaceShift = 13;

		internal const int PendingFederatedDomainShift = 14;

		internal const int InitialDomainShift = 15;

		internal const int MatchSubDomainsShift = 16;

		internal const int PendingCompletionShift = 17;

		internal const int InitialDomainShiftValue = 32768;

		public static readonly ADPropertyDefinition DomainName = new ADPropertyDefinition("DomainName", ExchangeObjectVersion.Exchange2007, typeof(SmtpDomainWithSubdomains), "msExchAcceptedDomainName", ADPropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CatchAllRecipient = new ADPropertyDefinition("CatchAllRecipient", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), "msExchCatchAllRecipientLink", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AcceptedDomainFlags = new ADPropertyDefinition("AcceptedDomainFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchAcceptedDomainFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition FederatedOrganizationLink = new ADPropertyDefinition("FederatedOrganizationLink", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchFedAcceptedDomainLink", ADPropertyDefinitionFlags.DoNotValidate, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MailFlowPartner = new ADPropertyDefinition("MailFlowPartner", ExchangeObjectVersion.Exchange2007, typeof(ADObjectId), "msExchTransportResellerSettingsLink", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HomeRealmRecord = new ADPropertyDefinition("HomeRealmRecord", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchOfflineOrgIdHomeRealmRecord", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AcceptedDomainType = ADObject.BitfieldProperty("AcceptedDomainType", 0, 2, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition Default = ADObject.BitfieldProperty("Default", 2, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition AddressBookEnabled = ADObject.BitfieldProperty("AddressBookEnabled", 3, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition X400AddressType = ADObject.BitfieldProperty("X400AddressType", 4, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition MatchSubDomains = ADObject.BitfieldProperty("MatchSubDomains", 16, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition RawAuthenticationType = new ADPropertyDefinition("RawAuthenticationType", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationType), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.AuthenticationType.Managed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AcceptedDomainSchema.AcceptedDomainFlags
		}, null, AcceptedDomainSchema.RawAuthenticationTypeGetterDelegate(), AcceptedDomainSchema.RawAuthenticationTypeSetterDelegate(), null, null);

		public static readonly ADPropertyDefinition AuthenticationType = new ADPropertyDefinition("AuthenticationType", ExchangeObjectVersion.Exchange2007, typeof(AuthenticationType?), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.AuthenticationType.Managed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AcceptedDomainSchema.AcceptedDomainFlags,
			ADObjectSchema.OrganizationalUnitRoot,
			ADObjectSchema.ConfigurationUnit
		}, null, AcceptedDomainSchema.AuthenticationTypeGetterDelegate(), null, null, null);

		public static readonly ADPropertyDefinition RawLiveIdInstanceType = new ADPropertyDefinition("RawLiveIdInstanceType", ExchangeObjectVersion.Exchange2007, typeof(LiveIdInstanceType), null, ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.LiveIdInstanceType.Consumer, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AcceptedDomainSchema.AcceptedDomainFlags
		}, null, AcceptedDomainSchema.RawLiveIdInstanceTypeGetterDelegate(), AcceptedDomainSchema.RawLiveIdInstanceTypeSetterDelegate(), null, null);

		public static readonly ADPropertyDefinition LiveIdInstanceType = new ADPropertyDefinition("LiveIdInstanceType", ExchangeObjectVersion.Exchange2007, typeof(LiveIdInstanceType?), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, Microsoft.Exchange.Data.Directory.LiveIdInstanceType.Consumer, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AcceptedDomainSchema.AcceptedDomainFlags,
			ADObjectSchema.OrganizationalUnitRoot,
			ADObjectSchema.ConfigurationUnit
		}, null, AcceptedDomainSchema.LiveIdInstanceTypeGetterDelegate(), null, null, null);

		public static readonly ADPropertyDefinition PendingRemoval = ADObject.BitfieldProperty("PendingRemoval", 6, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition PendingCompletion = ADObject.BitfieldProperty("PendingCompletion", 17, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition DualProvisioningEnabled = ADObject.BitfieldProperty("DualProvisioningEnabled", 12, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition OutboundOnly = ADObject.BitfieldProperty("OutboundOnly", 8, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition IsCoexistenceDomain = ADObject.BitfieldProperty("IsCoexistenceDomain", 11, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition PendingFederatedAccountNamespace = ADObject.BitfieldProperty("PendingFederatedAccountNamespace", 13, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition PendingFederatedDomain = ADObject.BitfieldProperty("PendingFederatedDomain", 14, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition PerimeterFlags = new ADPropertyDefinition("PerimeterFlags", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchTransportInboundSettings", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PerimeterDuplicateDetected = new ADPropertyDefinition("PerimeterDuplicateDetected", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AcceptedDomainSchema.PerimeterFlags
		}, null, ADObject.FlagGetterDelegate(AcceptedDomainSchema.PerimeterFlags, 1), ADObject.FlagSetterDelegate(AcceptedDomainSchema.PerimeterFlags, 1), null, null);

		public static readonly ADPropertyDefinition IsDefaultFederatedDomain = ADObject.BitfieldProperty("IsDefaultFederatedDomain", 9, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition EnableNego2Authentication = ADObject.BitfieldProperty("EnableNego2Authentication", 10, AcceptedDomainSchema.AcceptedDomainFlags);

		public static readonly ADPropertyDefinition InitialDomain = new ADPropertyDefinition("InitialDomain", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			AcceptedDomainSchema.AcceptedDomainFlags
		}, (SinglePropertyFilter filter) => ADObject.BoolFilterBuilder(filter, new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 32768UL)), ADObject.FlagGetterDelegate(AcceptedDomainSchema.AcceptedDomainFlags, 32768), ADObject.FlagSetterDelegate(AcceptedDomainSchema.AcceptedDomainFlags, 32768), null, null);

		internal static readonly ADPropertyDefinition UsnCreated = new ADPropertyDefinition("UsnCreated", ExchangeObjectVersion.Exchange2010, typeof(long), "uSNCreated", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.PersistDefaultValue, 0L, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
