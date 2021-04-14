using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Serializable]
	public class BposPlacementConfiguration : ConfigurableObject
	{
		internal BposPlacementConfiguration(string configuration) : this()
		{
			this.Configuration = configuration;
		}

		internal BposPlacementConfiguration() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public string Configuration
		{
			get
			{
				return (string)base[BposPlacementConfigurationSchema.Configuration];
			}
			private set
			{
				base[BposPlacementConfigurationSchema.Configuration] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return BposPlacementConfiguration.schema;
			}
		}

		private static BposPlacementConfigurationSchema schema = ObjectSchema.GetInstance<BposPlacementConfigurationSchema>();
	}
}
