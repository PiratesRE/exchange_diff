using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ADCrossRef : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADCrossRef.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADCrossRef.mostDerivedClass;
			}
		}

		public ADObjectId NCName
		{
			get
			{
				return (ADObjectId)this.propertyBag[ADCrossRefSchema.NCName];
			}
		}

		public MultiValuedProperty<string> DnsRoot
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[ADCrossRefSchema.DnsRoot];
			}
		}

		public string NetBiosName
		{
			get
			{
				return (string)this.propertyBag[ADCrossRefSchema.NetBiosName];
			}
		}

		public ADObjectId TrustParent
		{
			get
			{
				return (ADObjectId)this.propertyBag[ADCrossRefSchema.TrustParent];
			}
		}

		private static ADCrossRefSchema schema = ObjectSchema.GetInstance<ADCrossRefSchema>();

		private static string mostDerivedClass = "crossRef";
	}
}
