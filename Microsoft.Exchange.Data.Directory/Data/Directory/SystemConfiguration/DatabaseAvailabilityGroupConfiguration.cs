using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DatabaseAvailabilityGroupConfiguration : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DatabaseAvailabilityGroupConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DatabaseAvailabilityGroupConfiguration.mostDerivedClass;
			}
		}

		public new string Name
		{
			get
			{
				return (string)this[DatabaseAvailabilityGroupConfigurationSchema.Name];
			}
			internal set
			{
				this[DatabaseAvailabilityGroupConfigurationSchema.Name] = value;
			}
		}

		public string ConfigurationXML
		{
			get
			{
				return (string)this[DatabaseAvailabilityGroupConfigurationSchema.ConfigurationXML];
			}
			internal set
			{
				this[DatabaseAvailabilityGroupConfigurationSchema.ConfigurationXML] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Dags
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[DatabaseAvailabilityGroupConfigurationSchema.Dags];
			}
		}

		internal static object DagConfigNameGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.RawName];
		}

		internal static void DagConfigNameSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.RawName] = value;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly DatabaseAvailabilityGroupConfigurationSchema schema = ObjectSchema.GetInstance<DatabaseAvailabilityGroupConfigurationSchema>();

		private static string mostDerivedClass = "msExchMDBAvailabilityGroupConfiguration";
	}
}
