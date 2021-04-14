using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class ADMobileVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		private static MobileClientFlagsType DefaultMobileClientFlags
		{
			get
			{
				return MobileClientFlagsType.BadItemReportingEnabled | MobileClientFlagsType.SendWatsonReport;
			}
		}

		internal static object MobileClientCertProvisioningEnabledGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				return false;
			}
			MobileClientFlagsType mobileClientFlagsType = (MobileClientFlagsType)obj;
			return (mobileClientFlagsType & MobileClientFlagsType.ClientCertProvisionEnabled) == MobileClientFlagsType.ClientCertProvisionEnabled;
		}

		internal static void MobileClientCertProvisioningEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				obj = ADMobileVirtualDirectorySchema.DefaultMobileClientFlags;
			}
			if (flag)
			{
				propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj | MobileClientFlagsType.ClientCertProvisionEnabled);
				return;
			}
			propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj & ~MobileClientFlagsType.ClientCertProvisionEnabled);
		}

		internal static void BadItemReportingEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				obj = ADMobileVirtualDirectorySchema.DefaultMobileClientFlags;
			}
			if (flag)
			{
				propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj | MobileClientFlagsType.BadItemReportingEnabled);
				return;
			}
			propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj & ~MobileClientFlagsType.BadItemReportingEnabled);
		}

		internal static object BadItemReportingEnabledGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				return true;
			}
			MobileClientFlagsType mobileClientFlagsType = (MobileClientFlagsType)obj;
			return (mobileClientFlagsType & MobileClientFlagsType.BadItemReportingEnabled) == MobileClientFlagsType.BadItemReportingEnabled;
		}

		internal static void SendWatsonReportSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				obj = ADMobileVirtualDirectorySchema.DefaultMobileClientFlags;
			}
			if (flag)
			{
				propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj | MobileClientFlagsType.SendWatsonReport);
				return;
			}
			propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj & ~MobileClientFlagsType.SendWatsonReport);
		}

		internal static object SendWatsonReportGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				return (ADMobileVirtualDirectorySchema.DefaultMobileClientFlags & MobileClientFlagsType.SendWatsonReport) == MobileClientFlagsType.SendWatsonReport;
			}
			MobileClientFlagsType mobileClientFlagsType = (MobileClientFlagsType)obj;
			return (mobileClientFlagsType & MobileClientFlagsType.SendWatsonReport) == MobileClientFlagsType.SendWatsonReport;
		}

		internal static void RemoteDocumentsActionForUnknownServersSetter(object value, IPropertyBag propertyBag)
		{
			RemoteDocumentsActions remoteDocumentsActions = (RemoteDocumentsActions)value;
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				obj = ADMobileVirtualDirectorySchema.DefaultMobileClientFlags;
			}
			if (remoteDocumentsActions == RemoteDocumentsActions.Block)
			{
				propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj | MobileClientFlagsType.RemoteDocumentsActionForUnknownServers);
				return;
			}
			if (remoteDocumentsActions == RemoteDocumentsActions.Allow)
			{
				propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags] = ((MobileClientFlagsType)obj & ~MobileClientFlagsType.RemoteDocumentsActionForUnknownServers);
				return;
			}
			throw new ArgumentException("value can only be Allow or Block");
		}

		internal static object RemoteDocumentsActionForUnknownServersGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[ADMobileVirtualDirectorySchema.MobileClientFlags];
			if (obj == null)
			{
				return RemoteDocumentsActions.Allow;
			}
			MobileClientFlagsType mobileClientFlagsType = (MobileClientFlagsType)obj;
			return ((mobileClientFlagsType & MobileClientFlagsType.RemoteDocumentsActionForUnknownServers) == MobileClientFlagsType.RemoteDocumentsActionForUnknownServers) ? RemoteDocumentsActions.Block : RemoteDocumentsActions.Allow;
		}

		public static readonly ADPropertyDefinition MobileClientFlags = new ADPropertyDefinition("MobileClientFlags", ExchangeObjectVersion.Exchange2007, typeof(MobileClientFlagsType), "msExchMobileClientFlags", ADPropertyDefinitionFlags.None, ADMobileVirtualDirectorySchema.DefaultMobileClientFlags, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MobileClientCertificateAuthorityURL = new ADPropertyDefinition("MobileClientCertificateAuthorityURL", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileClientCertificateAuthorityURL", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MobileClientCertTemplateName = new ADPropertyDefinition("MobileClientCertTemplateName", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileClientCertTemplateName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MobileClientCertificateProvisioningEnabled = new ADPropertyDefinition("MobileClientCertificateProvisioningEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.MobileClientFlags
		}, null, new GetterDelegate(ADMobileVirtualDirectorySchema.MobileClientCertProvisioningEnabledGetter), new SetterDelegate(ADMobileVirtualDirectorySchema.MobileClientCertProvisioningEnabledSetter), null, null);

		public static readonly ADPropertyDefinition BadItemReportingEnabled = new ADPropertyDefinition("BadItemReportingEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.MobileClientFlags
		}, null, new GetterDelegate(ADMobileVirtualDirectorySchema.BadItemReportingEnabledGetter), new SetterDelegate(ADMobileVirtualDirectorySchema.BadItemReportingEnabledSetter), null, null);

		public static readonly ADPropertyDefinition SendWatsonReport = new ADPropertyDefinition("SendWatsonReport", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.MobileClientFlags
		}, null, new GetterDelegate(ADMobileVirtualDirectorySchema.SendWatsonReportGetter), new SetterDelegate(ADMobileVirtualDirectorySchema.SendWatsonReportSetter), null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsActionForUnknownServers = new ADPropertyDefinition("RemoteDocumentsActionForUnknownServers", ExchangeObjectVersion.Exchange2007, typeof(RemoteDocumentsActions), null, ADPropertyDefinitionFlags.Calculated, RemoteDocumentsActions.Allow, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.MobileClientFlags
		}, null, new GetterDelegate(ADMobileVirtualDirectorySchema.RemoteDocumentsActionForUnknownServersGetter), new SetterDelegate(ADMobileVirtualDirectorySchema.RemoteDocumentsActionForUnknownServersSetter), null, null);

		internal static readonly ADPropertyDefinition ADRemoteDocumentsAllowedServers = new ADPropertyDefinition("ADRemoteDocumentsAllowedServers", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileRemoteDocumentsAllowedServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsAllowedServers = new ADPropertyDefinition("RemoteDocumentsAllowedServers", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.ADRemoteDocumentsAllowedServers,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADMobileVirtualDirectory.RemoteDocumentsAllowedServersGetter), new SetterDelegate(ADMobileVirtualDirectory.RemoteDocumentsAllowedServersSetter), null, null);

		internal static readonly ADPropertyDefinition ADRemoteDocumentsBlockedServers = new ADPropertyDefinition("ADRemoteDocumentsBlockedServers", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileRemoteDocumentsBlockedServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsBlockedServers = new ADPropertyDefinition("RemoteDocumentsBlockedServers", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 256)
		}, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.ADRemoteDocumentsBlockedServers,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADMobileVirtualDirectory.RemoteDocumentsBlockedServersGetter), new SetterDelegate(ADMobileVirtualDirectory.RemoteDocumentsBlockedServersSetter), null, null);

		internal static readonly ADPropertyDefinition ADRemoteDocumentsInternalDomainSuffixList = new ADPropertyDefinition("ADRemoteDocumentsInternalDomainSuffixList", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchMobileRemoteDocumentsInternalDomainSuffixList", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteDocumentsInternalDomainSuffixList = new ADPropertyDefinition("RemoteDocumentsInternalDomainSuffixList", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ValidDomainConstraint()
		}, new ProviderPropertyDefinition[]
		{
			ADMobileVirtualDirectorySchema.ADRemoteDocumentsInternalDomainSuffixList,
			ADObjectSchema.Id
		}, null, new GetterDelegate(ADMobileVirtualDirectory.RemoteDocumentsInternalDomainSuffixListGetter), new SetterDelegate(ADMobileVirtualDirectory.RemoteDocumentsInternalDomainSuffixListSetter), null, null);

		public static readonly ADPropertyDefinition BasicAuthEnabled = new ADPropertyDefinition("BasicAuthEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WindowsAuthEnabled = new ADPropertyDefinition("WindowsAuthEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CompressionEnabled = new ADPropertyDefinition("CompressionEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ClientCertAuth = new ADPropertyDefinition("ClientCertAuth", ExchangeObjectVersion.Exchange2007, typeof(ClientCertAuthTypes), null, ADPropertyDefinitionFlags.TaskPopulated, ClientCertAuthTypes.Ignore, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WebsiteName = new ADPropertyDefinition("WebsiteName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WebSiteSSLEnabled = new ADPropertyDefinition("WebSiteSSLEnabled", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition VirtualDirectoryName = new ADPropertyDefinition("VirtualDirectoryName", ExchangeObjectVersion.Exchange2007, typeof(string), null, ADPropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
