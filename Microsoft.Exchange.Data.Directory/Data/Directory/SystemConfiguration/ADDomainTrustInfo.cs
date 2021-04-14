using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADDomainTrustInfo : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADDomainTrustInfo.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADDomainTrustInfo.mostDerivedClass;
			}
		}

		public string TargetName
		{
			get
			{
				return (string)this[ADDomainTrustInfoSchema.TargetName];
			}
		}

		public TrustAttributeFlag TrustAttribute
		{
			get
			{
				return (TrustAttributeFlag)this[ADDomainTrustInfoSchema.TrustAttributes];
			}
		}

		private static ADDomainTrustInfoSchema schema = ObjectSchema.GetInstance<ADDomainTrustInfoSchema>();

		private static string mostDerivedClass = "trustedDomain";
	}
}
