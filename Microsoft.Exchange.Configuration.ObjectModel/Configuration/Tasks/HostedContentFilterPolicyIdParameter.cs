using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class HostedContentFilterPolicyIdParameter : ADIdParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public HostedContentFilterPolicyIdParameter()
		{
		}

		public HostedContentFilterPolicyIdParameter(ADObjectId adobjectid) : base(adobjectid)
		{
		}

		public HostedContentFilterPolicyIdParameter(string identity) : base(identity)
		{
		}

		public HostedContentFilterPolicyIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ExchangeVersion, ExchangeObjectVersion.Exchange2012)
				});
			}
		}

		public static HostedContentFilterPolicyIdParameter Parse(string identity)
		{
			return new HostedContentFilterPolicyIdParameter(identity);
		}
	}
}
