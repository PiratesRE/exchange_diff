using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ThrottlingPolicyAssociation", DefaultParameterSetName = "Identity")]
	public sealed class GetThrottlingPolicyAssociation : GetRecipientBase<ThrottlingPolicyAssociationIdParameter, ADRecipient>
	{
		private new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter]
		public ThrottlingPolicyIdParameter ThrottlingPolicy
		{
			get
			{
				return (ThrottlingPolicyIdParameter)base.Fields[ADRecipientSchema.ThrottlingPolicy];
			}
			set
			{
				base.Fields[ADRecipientSchema.ThrottlingPolicy] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return base.OptionalIdentityData.AdditionalFilter;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ThrottlingPolicyAssociationSchema>();
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetThrottlingPolicyAssociation.SortPropertiesArray;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADRecipient dataObject2 = (ADRecipient)dataObject;
			return ThrottlingPolicyAssociation.FromDataObject(dataObject2);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.Fields.IsModified(ADRecipientSchema.ThrottlingPolicy))
			{
				QueryFilter queryFilter;
				if (this.ThrottlingPolicy == null)
				{
					queryFilter = new NotFilter(new ExistsFilter(ADRecipientSchema.ThrottlingPolicy));
				}
				else
				{
					ThrottlingPolicy throttlingPolicy = (ThrottlingPolicy)base.GetDataObject<ThrottlingPolicy>(this.ThrottlingPolicy, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorThrottlingPolicyNotFound(this.ThrottlingPolicy.ToString())), new LocalizedString?(Strings.ErrorThrottlingPolicyNotUnique(this.ThrottlingPolicy.ToString())), ExchangeErrorCategory.Client);
					queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ThrottlingPolicy, (ADObjectId)throttlingPolicy.Identity);
				}
				base.OptionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					base.OptionalIdentityData.AdditionalFilter,
					queryFilter
				});
			}
			TaskLogger.LogExit();
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ThrottlingPolicyAssociationSchema.ThrottlingPolicyId
		};
	}
}
