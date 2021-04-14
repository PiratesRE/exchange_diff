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
	public class NonMailEnabledUserIdParameter : RecipientIdParameter
	{
		public NonMailEnabledUserIdParameter(string identity) : base(identity)
		{
		}

		public NonMailEnabledUserIdParameter()
		{
		}

		public NonMailEnabledUserIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public NonMailEnabledUserIdParameter(User user) : base(user.Id)
		{
		}

		public NonMailEnabledUserIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return NonMailEnabledUserIdParameter.AllowedRecipientTypes;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					RecipientIdParameter.GetRecipientTypeDetailsFilter(NonMailEnabledUserIdParameter.AllowedRecipientTypeDetails)
				});
			}
		}

		public new static NonMailEnabledUserIdParameter Parse(string identity)
		{
			return new NonMailEnabledUserIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeNonMailEnabledUser(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User
		};

		internal static RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.User,
			RecipientTypeDetails.DisabledUser
		};
	}
}
