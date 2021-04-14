using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public interface IADOrgPerson : IADRecipient, IADObject, IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		string C { get; set; }

		string City { get; set; }

		string Co { get; set; }

		string Company { get; set; }

		int CountryCode { get; set; }

		string CountryOrRegionDisplayName { get; }

		CountryInfo CountryOrRegion { get; set; }

		string Department { get; set; }

		MultiValuedProperty<ADObjectId> DirectReports { get; }

		string Fax { get; set; }

		string FirstName { get; set; }

		string HomePhone { get; set; }

		MultiValuedProperty<string> IndexedPhoneNumbers { get; }

		string Initials { get; set; }

		MultiValuedProperty<CultureInfo> Languages { get; set; }

		string LastName { get; set; }

		ADObjectId Manager { get; set; }

		string MobilePhone { get; set; }

		string Office { get; set; }

		MultiValuedProperty<string> OtherFax { get; set; }

		MultiValuedProperty<string> OtherHomePhone { get; set; }

		MultiValuedProperty<string> OtherTelephone { get; set; }

		string Pager { get; set; }

		string Phone { get; set; }

		string PostalCode { get; set; }

		MultiValuedProperty<string> PostOfficeBox { get; set; }

		string RtcSipLine { get; }

		MultiValuedProperty<string> SanitizedPhoneNumbers { get; }

		string StateOrProvince { get; set; }

		string StreetAddress { get; set; }

		string TelephoneAssistant { get; set; }

		string Title { get; set; }

		MultiValuedProperty<string> UMCallingLineIds { get; set; }

		MultiValuedProperty<string> VoiceMailSettings { get; set; }

		object[][] GetManagementChainView(bool getPeers, params PropertyDefinition[] returnProperties);

		object[][] GetDirectReportsView(params PropertyDefinition[] returnProperties);

		void PopulateDtmfMap(bool create);
	}
}
