using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ADSiteLink : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADSiteLink.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADSiteLink.mostDerivedClass;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		public int Cost
		{
			get
			{
				return (int)this[ADSiteLinkSchema.Cost];
			}
		}

		public int ADCost
		{
			get
			{
				return (int)this[ADSiteLinkSchema.ADCost];
			}
		}

		[Parameter]
		public int? ExchangeCost
		{
			get
			{
				return (int?)this[ADSiteLinkSchema.ExchangeCost];
			}
			set
			{
				this[ADSiteLinkSchema.ExchangeCost] = value;
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize> MaxMessageSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ADSiteLinkSchema.MaxMessageSize];
			}
			set
			{
				this[ADSiteLinkSchema.MaxMessageSize] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Sites
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ADSiteLinkSchema.Sites];
			}
		}

		internal static object CostGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADSiteLinkSchema.ExchangeCost] ?? propertyBag[ADSiteLinkSchema.ADCost];
		}

		private static ADSiteLinkSchema schema = ObjectSchema.GetInstance<ADSiteLinkSchema>();

		private static string mostDerivedClass = "siteLink";

		internal static ulong UnlimitedMaxMessageSize = ByteQuantifiedSize.MaxValue.ToBytes();
	}
}
