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
	public class MailUserIdParameter : MailUserIdParameterBase
	{
		public MailUserIdParameter(string identity) : base(identity)
		{
		}

		public MailUserIdParameter()
		{
		}

		public MailUserIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailUserIdParameter(MailUser user) : base(user.Id)
		{
		}

		public MailUserIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					MailUserIdParameter.GetMailUserRecipientTypeDetailsFilter()
				});
			}
		}

		public new static MailUserIdParameter Parse(string identity)
		{
			return new MailUserIdParameter(identity);
		}

		internal static QueryFilter GetMailUserRecipientTypeDetailsFilter()
		{
			return RecipientIdParameter.GetRecipientTypeDetailsFilter(new RecipientTypeDetails[]
			{
				RecipientTypeDetails.MailUser
			});
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailUser(id);
		}
	}
}
