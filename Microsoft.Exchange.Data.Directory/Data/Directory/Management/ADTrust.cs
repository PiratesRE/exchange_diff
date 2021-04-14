using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ADTrust : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ADTrust.schema;
			}
		}

		public ADTrust()
		{
		}

		public ADTrust(ADDomainTrustInfo dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		public ADTrustType TrustType
		{
			get
			{
				TrustAttributeFlag trustAttributeFlag = (TrustAttributeFlag)this[ADDomainTrustInfoSchema.TrustAttributes];
				if ((trustAttributeFlag & TrustAttributeFlag.ForestTransitive) == TrustAttributeFlag.ForestTransitive)
				{
					return ADTrustType.Forest;
				}
				if ((trustAttributeFlag & TrustAttributeFlag.ForestTransitive) != TrustAttributeFlag.ForestTransitive && (trustAttributeFlag & TrustAttributeFlag.WithinForest) != TrustAttributeFlag.WithinForest)
				{
					return ADTrustType.External;
				}
				return ADTrustType.None;
			}
		}

		private static ADTrustSchema schema = ObjectSchema.GetInstance<ADTrustSchema>();
	}
}
