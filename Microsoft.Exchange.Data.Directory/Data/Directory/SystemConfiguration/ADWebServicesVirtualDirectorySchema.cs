using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ADWebServicesVirtualDirectorySchema : ExchangeVirtualDirectorySchema
	{
		internal static object MRSProxyEnabledGetter(IPropertyBag propertyBag)
		{
			MRSProxyFlagsEnum mrsproxyFlagsEnum = (MRSProxyFlagsEnum)propertyBag[ADWebServicesVirtualDirectorySchema.MRSProxyFlags];
			return (mrsproxyFlagsEnum & MRSProxyFlagsEnum.Enabled) == MRSProxyFlagsEnum.Enabled;
		}

		internal static void MRSProxyEnabledSetter(object value, IPropertyBag propertyBag)
		{
			bool flag = (bool)value;
			MRSProxyFlagsEnum mrsproxyFlagsEnum = (MRSProxyFlagsEnum)propertyBag[ADWebServicesVirtualDirectorySchema.MRSProxyFlags];
			if (flag)
			{
				propertyBag[ADWebServicesVirtualDirectorySchema.MRSProxyFlags] = (mrsproxyFlagsEnum | MRSProxyFlagsEnum.Enabled);
				return;
			}
			propertyBag[ADWebServicesVirtualDirectorySchema.MRSProxyFlags] = (mrsproxyFlagsEnum & ~MRSProxyFlagsEnum.Enabled);
		}

		public static readonly ADPropertyDefinition InternalNLBBypassUrl = new ADPropertyDefinition("InternalNLBBypassURL", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchInternalNLBBypassHostName", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition ADGzipLevel = new ADPropertyDefinition("GzipLevel", ExchangeObjectVersion.Exchange2007, typeof(GzipLevel), null, ADPropertyDefinitionFlags.TaskPopulated, GzipLevel.High, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MRSProxyFlags = new ADPropertyDefinition("MRSProxyFlags", ExchangeObjectVersion.Exchange2010, typeof(MRSProxyFlagsEnum), "msExchMRSProxyFlags", ADPropertyDefinitionFlags.PersistDefaultValue, MRSProxyFlagsEnum.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MRSProxyEnabled = new ADPropertyDefinition("MRSProxyEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADWebServicesVirtualDirectorySchema.MRSProxyFlags
		}, null, new GetterDelegate(ADWebServicesVirtualDirectorySchema.MRSProxyEnabledGetter), new SetterDelegate(ADWebServicesVirtualDirectorySchema.MRSProxyEnabledSetter), null, null);
	}
}
