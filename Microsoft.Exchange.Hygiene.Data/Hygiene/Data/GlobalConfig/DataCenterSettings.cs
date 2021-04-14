using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.GlobalConfig
{
	internal class DataCenterSettings : ADObject
	{
		public MultiValuedProperty<IPRange> FfoDataCenterPublicIPAddresses
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[DataCenterSettings.DataCenterSettingsSchema.FfoDataCenterPublicIPAddressesProperty];
			}
			set
			{
				this[DataCenterSettings.DataCenterSettingsSchema.FfoDataCenterPublicIPAddressesProperty] = value;
			}
		}

		public MultiValuedProperty<SmtpX509IdentifierEx> FfoFrontDoorSmtpCertificates
		{
			get
			{
				return (MultiValuedProperty<SmtpX509IdentifierEx>)this[DataCenterSettings.DataCenterSettingsSchema.FfoFrontDoorSmtpCertificatesProperty];
			}
			set
			{
				this[DataCenterSettings.DataCenterSettingsSchema.FfoFrontDoorSmtpCertificatesProperty] = value;
			}
		}

		public MultiValuedProperty<ServiceProviderSettings> ServiceProviders
		{
			get
			{
				return (MultiValuedProperty<ServiceProviderSettings>)this[DataCenterSettings.DataCenterSettingsSchema.ServiceProvidersProperty];
			}
			set
			{
				this[DataCenterSettings.DataCenterSettingsSchema.ServiceProvidersProperty] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return DataCenterSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DataCenterSettings.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly string mostDerivedClass = "DataCenterSettings";

		private static readonly DataCenterSettings.DataCenterSettingsSchema schema = ObjectSchema.GetInstance<DataCenterSettings.DataCenterSettingsSchema>();

		internal class DataCenterSettingsSchema : ADObjectSchema
		{
			internal static readonly HygienePropertyDefinition FfoDataCenterPublicIPAddressesProperty = new HygienePropertyDefinition("FfoDataCenterPublicIPs", typeof(IPRange), null, ADPropertyDefinitionFlags.MultiValued);

			internal static readonly HygienePropertyDefinition FfoFrontDoorSmtpCertificatesProperty = new HygienePropertyDefinition("FfoFrontDoorSmtpCertificates", typeof(SmtpX509IdentifierEx), null, ADPropertyDefinitionFlags.MultiValued);

			internal static readonly HygienePropertyDefinition ServiceProvidersProperty = new HygienePropertyDefinition("ServiceProviderSettings", typeof(ServiceProviderSettings), null, ADPropertyDefinitionFlags.MultiValued);
		}
	}
}
