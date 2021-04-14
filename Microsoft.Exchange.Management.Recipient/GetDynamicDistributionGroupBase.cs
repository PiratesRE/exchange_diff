using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetDynamicDistributionGroupBase : GetRecipientBase<DynamicGroupIdParameter, ADDynamicGroup>
	{
		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetDynamicDistributionGroupBase.SortPropertiesArray;
			}
		}

		protected override RecipientType[] RecipientTypes
		{
			get
			{
				return GetDynamicDistributionGroupBase.RecipientTypesArray;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<DynamicDistributionGroupSchema>();
			}
		}

		[Parameter(ParameterSetName = "ManagedBySet")]
		public GeneralRecipientIdParameter ManagedBy
		{
			get
			{
				return (GeneralRecipientIdParameter)base.Fields["ManagedBy"];
			}
			set
			{
				base.Fields["ManagedBy"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = base.InternalFilter;
				if (this.managerId != null)
				{
					QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADDynamicGroupSchema.ManagedBy, this.managerId);
					if (queryFilter != null)
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter2,
							queryFilter
						});
					}
					else
					{
						queryFilter = queryFilter2;
					}
				}
				return queryFilter;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.ManagedBy != null)
			{
				ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.ManagedBy, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.RecipientNotFoundException(this.ManagedBy.ToString())), new LocalizedString?(Strings.RecipientNotUniqueException(this.ManagedBy.ToString())));
				if (base.HasErrors)
				{
					return;
				}
				this.managerId = (ADObjectId)adrecipient.Identity;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DynamicDistributionGroup.FromDataObject((ADDynamicGroup)dataObject);
		}

		private ADObjectId managerId;

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.Alias
		};

		private static readonly RecipientType[] RecipientTypesArray = new RecipientType[]
		{
			RecipientType.DynamicDistributionGroup
		};
	}
}
