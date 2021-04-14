using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public abstract class AddressListBase : ADPresentationObject, ISupportRecipientFilter
	{
		public AddressListBase()
		{
		}

		public AddressListBase(AddressBookBase dataObject) : base(dataObject)
		{
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public new string Name
		{
			get
			{
				return (string)this[AddressListBaseSchema.Name];
			}
			set
			{
				this[AddressListBaseSchema.Name] = value;
			}
		}

		public string RecipientFilter
		{
			get
			{
				return (string)this[AddressListBaseSchema.RecipientFilter];
			}
		}

		public string LdapRecipientFilter
		{
			get
			{
				return (string)this[AddressListBaseSchema.LdapRecipientFilter];
			}
		}

		public string LastUpdatedRecipientFilter
		{
			get
			{
				return (string)this[AddressListBaseSchema.LastUpdatedRecipientFilter];
			}
		}

		public bool RecipientFilterApplied
		{
			get
			{
				return (bool)this[AddressListBaseSchema.RecipientFilterApplied];
			}
		}

		[Parameter]
		public WellKnownRecipientType? IncludedRecipients
		{
			get
			{
				return (WellKnownRecipientType?)this[AddressListBaseSchema.IncludedRecipients];
			}
			set
			{
				this[AddressListBaseSchema.IncludedRecipients] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalDepartment
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalDepartment];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalDepartment] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCompany
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCompany];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCompany] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalStateOrProvince
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalStateOrProvince];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalStateOrProvince] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute1
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute1];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute1] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute2
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute2];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute2] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute3
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute3];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute3] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute4
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute4];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute4] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute5
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute5];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute5] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute6
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute6];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute6] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute7
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute7];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute7] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute8
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute8];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute8] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute9
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute9];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute9] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute10
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute10];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute10] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute11
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute11];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute11] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute12
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute12];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute12] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute13
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute13];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute13] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute14
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute14];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute14] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> ConditionalCustomAttribute15
		{
			get
			{
				return (MultiValuedProperty<string>)this[AddressListBaseSchema.ConditionalCustomAttribute15];
			}
			set
			{
				this[AddressListBaseSchema.ConditionalCustomAttribute15] = value;
			}
		}

		public ADObjectId RecipientContainer
		{
			get
			{
				return (ADObjectId)this[AddressListBaseSchema.RecipientContainer];
			}
			set
			{
				this[AddressListBaseSchema.RecipientContainer] = value;
			}
		}

		public WellKnownRecipientFilterType RecipientFilterType
		{
			get
			{
				return (WellKnownRecipientFilterType)this[AddressListBaseSchema.RecipientFilterType];
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.RecipientFilterSchema
		{
			get
			{
				return AddressBookBaseSchema.RecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.LdapRecipientFilterSchema
		{
			get
			{
				return AddressBookBaseSchema.LdapRecipientFilter;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.IncludedRecipientsSchema
		{
			get
			{
				return AddressBookBaseSchema.IncludedRecipients;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalDepartmentSchema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalDepartment;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCompanySchema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCompany;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalStateOrProvinceSchema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalStateOrProvince;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute1Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute1;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute2Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute2;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute3Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute3;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute4Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute4;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute5Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute5;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute6Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute6;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute7Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute7;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute8Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute8;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute9Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute9;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute10Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute10;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute11Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute11;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute12Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute12;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute13Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute13;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute14Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute14;
			}
		}

		ADPropertyDefinition ISupportRecipientFilter.ConditionalCustomAttribute15Schema
		{
			get
			{
				return AddressBookBaseSchema.ConditionalCustomAttribute15;
			}
		}
	}
}
