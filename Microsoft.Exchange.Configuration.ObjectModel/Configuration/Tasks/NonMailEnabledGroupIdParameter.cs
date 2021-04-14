using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class NonMailEnabledGroupIdParameter : RecipientIdParameter
	{
		public NonMailEnabledGroupIdParameter(string identity) : base(identity)
		{
		}

		public NonMailEnabledGroupIdParameter()
		{
		}

		public NonMailEnabledGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public NonMailEnabledGroupIdParameter(WindowsGroup group) : base(group.Id)
		{
		}

		public NonMailEnabledGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return NonMailEnabledGroupIdParameter.AllowedRecipientTypes;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					RecipientIdParameter.GetRecipientTypeDetailsFilter(NonMailEnabledGroupIdParameter.AllowedRecipientTypeDetails)
				});
			}
		}

		public new static NonMailEnabledGroupIdParameter Parse(string identity)
		{
			return new NonMailEnabledGroupIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeNonMailEnabledGroup(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Group
		};

		internal static RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.NonUniversalGroup,
			RecipientTypeDetails.UniversalDistributionGroup,
			RecipientTypeDetails.UniversalSecurityGroup
		};
	}
}
