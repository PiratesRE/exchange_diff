using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Server)]
	[Serializable]
	public class ADServiceConnectionPoint : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADServiceConnectionPoint.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADServiceConnectionPoint.mostDerivedClass;
			}
		}

		public MultiValuedProperty<string> Keywords
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[ADServiceConnectionPointSchema.Keywords];
			}
			internal set
			{
				this.propertyBag[ADServiceConnectionPointSchema.Keywords] = value;
			}
		}

		public MultiValuedProperty<string> ServiceBindingInformation
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[ADServiceConnectionPointSchema.ServiceBindingInformation];
			}
			internal set
			{
				this.propertyBag[ADServiceConnectionPointSchema.ServiceBindingInformation] = value;
			}
		}

		public string ServiceDnsName
		{
			get
			{
				return (string)this.propertyBag[ADServiceConnectionPointSchema.ServiceDnsName];
			}
			internal set
			{
				this.propertyBag[ADServiceConnectionPointSchema.ServiceDnsName] = value;
			}
		}

		public string ServiceClassName
		{
			get
			{
				return (string)this.propertyBag[ADServiceConnectionPointSchema.ServiceClassName];
			}
			internal set
			{
				this.propertyBag[ADServiceConnectionPointSchema.ServiceClassName] = value;
			}
		}

		private static ADServiceConnectionPointSchema schema = ObjectSchema.GetInstance<ADServiceConnectionPointSchema>();

		private static string mostDerivedClass = "ServiceConnectionPoint";
	}
}
