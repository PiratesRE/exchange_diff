using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADCrossRefContainer : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADCrossRefContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADCrossRefContainer.mostDerivedClass;
			}
		}

		public MultiValuedProperty<string> UPNSuffixes
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[ADCrossRefContainerSchema.UPNSuffixes];
			}
		}

		private static ADCrossRefContainerSchema schema = ObjectSchema.GetInstance<ADCrossRefContainerSchema>();

		private static string mostDerivedClass = "crossRefContainer";
	}
}
