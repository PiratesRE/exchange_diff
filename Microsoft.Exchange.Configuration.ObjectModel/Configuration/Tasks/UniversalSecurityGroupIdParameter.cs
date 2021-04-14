using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class UniversalSecurityGroupIdParameter : GroupIdParameter
	{
		public UniversalSecurityGroupIdParameter(string identity) : base(identity)
		{
		}

		public UniversalSecurityGroupIdParameter()
		{
		}

		public UniversalSecurityGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public UniversalSecurityGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return UniversalSecurityGroupIdParameter.AllowedRecipientTypes;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return UniversalSecurityGroupIdParameter.GetUniversalSecurityGroupFilter(base.AdditionalQueryFilter);
			}
		}

		public new static UniversalSecurityGroupIdParameter Parse(string identity)
		{
			return new UniversalSecurityGroupIdParameter(identity);
		}

		internal static QueryFilter GetUniversalSecurityGroupFilter(QueryFilter additionalFilter)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				additionalFilter,
				UniversalSecurityGroupIdParameter.USGFilter
			});
		}

		private new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Group,
			RecipientType.MailUniversalSecurityGroup
		};

		private static QueryFilter USGFilter = new AndFilter(new QueryFilter[]
		{
			new BitMaskAndFilter(ADGroupSchema.GroupType, 8UL),
			new BitMaskAndFilter(ADGroupSchema.GroupType, (ulong)int.MinValue)
		});
	}
}
