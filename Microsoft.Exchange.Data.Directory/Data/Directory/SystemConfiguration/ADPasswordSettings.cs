using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADPasswordSettings : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADPasswordSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADPasswordSettings.mostDerivedClass;
			}
		}

		public ADPasswordSettings()
		{
			this.propertyBag.Remove(ADObjectSchema.ExchangeVersion);
		}

		internal MultiValuedProperty<ADObjectId> AppliesTo
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this.propertyBag[ADPasswordSettingsSchema.AppliesTo];
			}
			set
			{
				this.propertyBag[ADPasswordSettingsSchema.AppliesTo] = value;
			}
		}

		private static ADPasswordSettingsSchema schema = ObjectSchema.GetInstance<ADPasswordSettingsSchema>();

		private static string mostDerivedClass = "msds-PasswordSettings";
	}
}
