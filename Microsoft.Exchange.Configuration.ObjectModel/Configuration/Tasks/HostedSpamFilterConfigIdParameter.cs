using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class HostedSpamFilterConfigIdParameter : ADIdParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public HostedSpamFilterConfigIdParameter()
		{
		}

		public HostedSpamFilterConfigIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		protected HostedSpamFilterConfigIdParameter(string identity) : base(identity)
		{
		}

		public HostedSpamFilterConfigIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2007)
				});
			}
		}

		public static HostedSpamFilterConfigIdParameter Parse(string identity)
		{
			return new HostedSpamFilterConfigIdParameter(identity);
		}
	}
}
