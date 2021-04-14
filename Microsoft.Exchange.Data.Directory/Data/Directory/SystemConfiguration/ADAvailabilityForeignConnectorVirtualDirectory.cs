using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADAvailabilityForeignConnectorVirtualDirectory : ExchangeVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADAvailabilityForeignConnectorVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADAvailabilityForeignConnectorVirtualDirectory.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		public string AvailabilityForeignConnectorType
		{
			get
			{
				return (string)this[ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorType];
			}
			internal set
			{
				this[ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorType] = value;
			}
		}

		public MultiValuedProperty<string> AvailabilityForeignConnectorDomains
		{
			get
			{
				return (MultiValuedProperty<string>)this[ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorDomains];
			}
			internal set
			{
				this[ADAvailabilityForeignConnectorVirtualDirectorySchema.AvailabilityForeignConnectorDomains] = value;
			}
		}

		private static readonly ADAvailabilityForeignConnectorVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADAvailabilityForeignConnectorVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchAvailabilityForeignConnectorVirtualDirectory";
	}
}
