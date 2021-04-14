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
	public class MailboxPlanIdParameter : RecipientIdParameter
	{
		public MailboxPlanIdParameter(string identity) : base(identity)
		{
		}

		public MailboxPlanIdParameter()
		{
		}

		public MailboxPlanIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxPlanIdParameter(MailboxPlan mailboxPlan) : base(mailboxPlan.Id)
		{
		}

		public MailboxPlanIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return MailboxPlanIdParameter.AllowedRecipientTypes;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					MailboxPlanIdParameter.MailboxPlanFilter
				});
			}
		}

		public new static MailboxPlanIdParameter Parse(string identity)
		{
			return new MailboxPlanIdParameter(identity);
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeMailboxPlan(id);
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.UserMailbox
		};

		private static ComparisonFilter MailboxPlanFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan);
	}
}
