using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class GetDistributionGroupBase : GetRecipientWithAddressListBase<DistributionGroupIdParameter, ADGroup>
	{
		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetDistributionGroupBase.SortPropertiesArray;
			}
		}

		protected override string SystemAddressListRdn
		{
			get
			{
				return "All Groups(VLV)";
			}
		}

		protected override RecipientType[] RecipientTypes
		{
			get
			{
				return GetDistributionGroupBase.RecipientTypesArray;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return this.RecipientTypeDetails;
			}
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<DistributionGroupSchema>();
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public RecipientTypeDetails[] RecipientTypeDetails
		{
			get
			{
				return (RecipientTypeDetails[])base.Fields["RecipientTypeDetails"];
			}
			set
			{
				base.VerifyValues<RecipientTypeDetails>(GetDistributionGroupBase.AllowedRecipientTypeDetails, value);
				base.Fields["RecipientTypeDetails"] = value;
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
				if (base.Fields.IsModified("ManagedBy"))
				{
					QueryFilter queryFilter2;
					if (this.managerId != null)
					{
						queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.ManagedBy, this.managerId);
					}
					else
					{
						queryFilter2 = new AndFilter(new QueryFilter[]
						{
							new NotFilter(new ExistsFilter(ADGroupSchema.RawManagedBy)),
							new NotFilter(new ExistsFilter(ADGroupSchema.CoManagedBy))
						});
					}
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
				if (!base.CurrentOrganizationId.Equals(adrecipient.OrganizationId))
				{
					base.CurrentOrganizationId = adrecipient.OrganizationId;
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DistributionGroup.FromDataObject((ADGroup)dataObject);
		}

		private ADObjectId managerId;

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailNonUniversalGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailUniversalDistributionGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.MailUniversalSecurityGroup,
			Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RoomList
		};

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.Alias
		};

		private static readonly RecipientType[] RecipientTypesArray = new RecipientType[]
		{
			RecipientType.MailUniversalDistributionGroup,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup
		};
	}
}
