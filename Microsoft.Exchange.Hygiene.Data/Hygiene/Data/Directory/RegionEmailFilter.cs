using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	public class RegionEmailFilter : ADObject
	{
		public bool Enabled
		{
			get
			{
				return (bool)this[RegionEmailFilterSchema.FilterStatus];
			}
			set
			{
				this[RegionEmailFilterSchema.FilterStatus] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RegionEmailFilter.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RegionEmailFilter.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly string mostDerivedClass = "RegionEmailFilter";

		private static readonly RegionEmailFilterSchema schema = ObjectSchema.GetInstance<RegionEmailFilterSchema>();
	}
}
