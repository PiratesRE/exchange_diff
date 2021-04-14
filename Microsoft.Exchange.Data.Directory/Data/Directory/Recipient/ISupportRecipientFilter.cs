using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface ISupportRecipientFilter
	{
		string RecipientFilter { get; }

		string LdapRecipientFilter { get; }

		WellKnownRecipientType? IncludedRecipients { get; set; }

		MultiValuedProperty<string> ConditionalDepartment { get; set; }

		MultiValuedProperty<string> ConditionalCompany { get; set; }

		MultiValuedProperty<string> ConditionalStateOrProvince { get; set; }

		ADObjectId RecipientContainer { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute1 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute2 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute3 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute4 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute5 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute6 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute7 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute8 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute9 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute10 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute11 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute12 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute13 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute14 { get; set; }

		MultiValuedProperty<string> ConditionalCustomAttribute15 { get; set; }

		WellKnownRecipientFilterType RecipientFilterType { get; }

		ADPropertyDefinition RecipientFilterSchema { get; }

		ADPropertyDefinition LdapRecipientFilterSchema { get; }

		ADPropertyDefinition IncludedRecipientsSchema { get; }

		ADPropertyDefinition ConditionalDepartmentSchema { get; }

		ADPropertyDefinition ConditionalCompanySchema { get; }

		ADPropertyDefinition ConditionalStateOrProvinceSchema { get; }

		ADPropertyDefinition ConditionalCustomAttribute1Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute2Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute3Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute4Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute5Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute6Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute7Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute8Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute9Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute10Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute11Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute12Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute13Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute14Schema { get; }

		ADPropertyDefinition ConditionalCustomAttribute15Schema { get; }
	}
}
