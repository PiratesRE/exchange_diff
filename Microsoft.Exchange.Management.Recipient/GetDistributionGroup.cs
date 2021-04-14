using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "DistributionGroup", DefaultParameterSetName = "Identity")]
	[OutputType(new Type[]
	{
		typeof(DistributionGroup)
	})]
	public sealed class GetDistributionGroup : GetDistributionGroupBase
	{
		[Parameter(Mandatory = false)]
		public new long UsnForReconciliationSearch
		{
			get
			{
				return base.UsnForReconciliationSearch;
			}
			set
			{
				base.UsnForReconciliationSearch = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetails, Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.GroupMailbox);
				ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.RecipientTypeDetails, Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteGroupMailbox);
				return new AndFilter(new QueryFilter[]
				{
					base.InternalFilter,
					comparisonFilter,
					comparisonFilter2
				});
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADGroup adgroup = dataObject as ADGroup;
			if (adgroup != null && (adgroup.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.GroupMailbox || adgroup.RecipientTypeDetails == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteGroupMailbox))
			{
				base.WriteError(new RecipientTaskException(Strings.NotAValidDistributionGroup), ExchangeErrorCategory.Client, this.Identity.ToString());
			}
			return base.ConvertDataObjectToPresentationObject(dataObject);
		}
	}
}
