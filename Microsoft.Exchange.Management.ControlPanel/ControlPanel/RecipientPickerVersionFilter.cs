using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class RecipientPickerVersionFilter : RecipientPickerFilterBase
	{
		protected virtual ExchangeObjectVersion MinVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		protected virtual RecipientTypeDetails[] RecipientTypeDetailsWithoutVersionRestriction
		{
			get
			{
				return null;
			}
		}

		protected override void UpdateFilterProperty()
		{
			base.UpdateFilterProperty();
			if (this.MinVersion != null)
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, this.MinVersion);
				if (!this.RecipientTypeDetailsWithoutVersionRestriction.IsNullOrEmpty())
				{
					QueryFilter recipientTypeDetailsFilter = RecipientIdParameter.GetRecipientTypeDetailsFilter(base.RecipientTypeDetailsList);
					if (recipientTypeDetailsFilter != null)
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							recipientTypeDetailsFilter,
							queryFilter
						});
					}
					recipientTypeDetailsFilter = RecipientIdParameter.GetRecipientTypeDetailsFilter(this.RecipientTypeDetailsWithoutVersionRestriction);
					queryFilter = new OrFilter(new QueryFilter[]
					{
						recipientTypeDetailsFilter,
						queryFilter
					});
					base.RecipientTypeDetailsList = null;
				}
				string text = (string)base["Filter"];
				if (!text.IsNullOrBlank())
				{
					base["Filter"] = string.Empty;
					MonadFilter monadFilter = new MonadFilter(text, null, ObjectSchema.GetInstance<ReducedRecipientSchema>());
					queryFilter = new AndFilter(new QueryFilter[]
					{
						monadFilter.InnerFilter,
						queryFilter
					});
				}
				base["RecipientPreviewFilter"] = LdapFilterBuilder.LdapFilterFromQueryFilter(queryFilter);
			}
		}

		public new const string RbacParameters = "?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter";
	}
}
