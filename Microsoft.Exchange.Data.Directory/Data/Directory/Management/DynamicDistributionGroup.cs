using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[ProvisioningObjectTag("DynamicDistributionGroup")]
	[Serializable]
	public class DynamicDistributionGroup : DistributionGroupBase, ISupportRecipientFilter
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return DynamicDistributionGroup.schema;
			}
		}

		public DynamicDistributionGroup()
		{
		}

		public DynamicDistributionGroup(ADDynamicGroup dataObject) : base(dataObject)
		{
		}

		internal static DynamicDistributionGroup FromDataObject(ADDynamicGroup dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return new DynamicDistributionGroup(dataObject);
		}

		public ADObjectId RecipientContainer
		{
			get
			{
				return (ADObjectId)this[DynamicDistributionGroupSchema.RecipientContainer];
			}
			set
			{
				this[DynamicDistributionGroupSchema.RecipientContainer] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public string RecipientFilter
		{
			get
			{
				return (string)this[DynamicDistributionGroupSchema.RecipientFilter];
			}
		}

		public string LdapRecipientFilter
		{
			get
			{
				return (string)this[DynamicDistributionGroupSchema.LdapRecipientFilter];
			}
		}

		[Parameter]
		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return (WellKnownRecipientType?)this[DynamicDistributionGroupSchema.IncludedRecipients];
			}
			set
			{
				this[DynamicDistributionGroupSchema.IncludedRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalDepartment];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalDepartment] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCompany];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCompany] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalStateOrProvince];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalStateOrProvince] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute1];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute1] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute2];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute2] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute3];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute3] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute4];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute4] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute5];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute5] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute6];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute6] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute7];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute7] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute8];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute8] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute9];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute9] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute10];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute10] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute11];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute11] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute12];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute12] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute13];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute13] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute14];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute14] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return (MultiValuedProperty<string>)this[DynamicDistributionGroupSchema.ConditionalCustomAttribute15];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ConditionalCustomAttribute15] = value;
			}
		}

		public WellKnownRecipientFilterType RecipientFilterType
		{
			get
			{
				return (WellKnownRecipientFilterType)this[DynamicDistributionGroupSchema.RecipientFilterType];
			}
		}

		[Parameter(Mandatory = false)]
		public string Notes
		{
			get
			{
				return (string)this[DynamicDistributionGroupSchema.Notes];
			}
			set
			{
				this[DynamicDistributionGroupSchema.Notes] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PhoneticDisplayName
		{
			get
			{
				return (string)this[DynamicDistributionGroupSchema.PhoneticDisplayName];
			}
			set
			{
				this[DynamicDistributionGroupSchema.PhoneticDisplayName] = value;
			}
		}

		public ADObjectId ManagedBy
		{
			get
			{
				return (ADObjectId)this[DynamicDistributionGroupSchema.ManagedBy];
			}
			set
			{
				this[DynamicDistributionGroupSchema.ManagedBy] = value;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.RecipientFilterSchema
		{
			get
			{
				return DynamicDistributionGroupSchema.RecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.LdapRecipientFilterSchema
		{
			get
			{
				return DynamicDistributionGroupSchema.LdapRecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.IncludedRecipientsSchema
		{
			get
			{
				return DynamicDistributionGroupSchema.IncludedRecipients;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalDepartmentSchema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalDepartment;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCompanySchema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCompany;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalStateOrProvinceSchema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalStateOrProvince;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute1Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute1;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute2Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute2;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute3Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute3;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute4Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute4;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute5Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute5;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute6Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute6;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute7Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute7;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute8Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute8;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute9Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute9;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute10Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute10;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute11Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute11;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute12Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute12;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute13Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute13;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute14Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute14;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute15Schema
		{
			get
			{
				return DynamicDistributionGroupSchema.ConditionalCustomAttribute15;
			}
		}

		private static DynamicDistributionGroupSchema schema = ObjectSchema.GetInstance<DynamicDistributionGroupSchema>();
	}
}
