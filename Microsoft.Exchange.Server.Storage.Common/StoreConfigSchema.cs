using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal class StoreConfigSchema : ConfigSchemaBase
	{
		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					this.propertyCollection = new ConfigurationPropertyCollection();
					for (int i = 0; i < ConfigurationSchema.RegisteredConfigurations.Count; i++)
					{
						ConfigurationSchema configurationSchema = ConfigurationSchema.RegisteredConfigurations[i];
						ConfigurationProperty configurationProperty = configurationSchema.ConfigurationProperty;
						this.propertyCollection.Add(configurationProperty);
					}
				}
				return this.propertyCollection;
			}
		}

		public override string Name
		{
			get
			{
				return "Store";
			}
		}

		private ConfigurationPropertyCollection propertyCollection;
	}
}
