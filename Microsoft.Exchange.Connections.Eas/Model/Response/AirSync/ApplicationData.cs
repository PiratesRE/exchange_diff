using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Common.Email;
using Microsoft.Exchange.Connections.Eas.Model.Common.WindowsLive;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "ApplicationData")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ApplicationData
	{
		[XmlElement(ElementName = "To", Namespace = "Email")]
		public string To { get; set; }

		[XmlElement(ElementName = "From", Namespace = "Email")]
		public string From { get; set; }

		[XmlElement(ElementName = "Subject", Namespace = "Email")]
		public string Subject { get; set; }

		[XmlElement(ElementName = "DateReceived", Namespace = "Email")]
		public string DateReceived { get; set; }

		[XmlElement(ElementName = "Importance", Namespace = "Email")]
		public byte Importance { get; set; }

		[XmlElement(ElementName = "Read", Namespace = "Email")]
		public byte? Read { get; set; }

		[XmlElement(ElementName = "Body", Namespace = "AirSyncBase")]
		public Body Body { get; set; }

		[XmlElement(ElementName = "MessageClass", Namespace = "Email")]
		public string MessageClass { get; set; }

		[XmlElement(ElementName = "InternetCPID", Namespace = "Email")]
		public string InternetCpid { get; set; }

		[XmlElement(ElementName = "Flag", Namespace = "Email")]
		public Flag Flag { get; set; }

		[XmlElement(ElementName = "ConversationId", Namespace = "Email2")]
		public string ConversationId { get; set; }

		[XmlElement(ElementName = "ConversationIndex", Namespace = "Email2")]
		public string ConversationIndex { get; set; }

		[XmlArray(ElementName = "Categories", Namespace = "Email")]
		public List<Category> Categories { get; set; }

		[XmlArray(ElementName = "SystemCategories", Namespace = "WindowsLive")]
		public List<CategoryId> SystemCategories { get; set; }

		[XmlElement(ElementName = "Anniversary", Namespace = "Contacts")]
		public string Anniversary { get; set; }

		[XmlElement(ElementName = "AssistantName", Namespace = "Contacts")]
		public string AssistantName { get; set; }

		[XmlElement(ElementName = "AssistantPhoneNumber", Namespace = "Contacts")]
		public string AssistantPhoneNumber { get; set; }

		[XmlElement(ElementName = "Birthday", Namespace = "Contacts")]
		public string Birthday { get; set; }

		[XmlElement(ElementName = "Business2PhoneNumber", Namespace = "Contacts")]
		public string Business2PhoneNumber { get; set; }

		[XmlElement(ElementName = "BusinessAddressCity", Namespace = "Contacts")]
		public string BusinessAddressCity { get; set; }

		[XmlElement(ElementName = "BusinessPhoneNumber", Namespace = "Contacts")]
		public string BusinessPhoneNumber { get; set; }

		[XmlElement(ElementName = "WebPage", Namespace = "Contacts")]
		public string WebPage { get; set; }

		[XmlElement(ElementName = "BusinessAddressCountry", Namespace = "Contacts")]
		public string BusinessAddressCountry { get; set; }

		[XmlElement(ElementName = "Department", Namespace = "Contacts")]
		public string Department { get; set; }

		[XmlElement(ElementName = "Email1Address", Namespace = "Contacts")]
		public string Email1Address { get; set; }

		[XmlElement(ElementName = "Email2Address", Namespace = "Contacts")]
		public string Email2Address { get; set; }

		[XmlElement(ElementName = "Email3Address", Namespace = "Contacts")]
		public string Email3Address { get; set; }

		[XmlElement(ElementName = "BusinessFaxNumber", Namespace = "Contacts")]
		public string BusinessFaxNumber { get; set; }

		[XmlElement(ElementName = "FileAs", Namespace = "Contacts")]
		public string FileAs { get; set; }

		[XmlElement(ElementName = "Alias", Namespace = "Contacts")]
		public string Alias { get; set; }

		[XmlElement(ElementName = "WeightedRank", Namespace = "Contacts")]
		public int WeightedRank { get; set; }

		[XmlElement(ElementName = "FirstName", Namespace = "Contacts")]
		public string FirstName { get; set; }

		[XmlElement(ElementName = "MiddleName", Namespace = "Contacts")]
		public string MiddleName { get; set; }

		[XmlElement(ElementName = "HomeAddressCity", Namespace = "Contacts")]
		public string HomeAddressCity { get; set; }

		[XmlElement(ElementName = "HomeAddressCountry", Namespace = "Contacts")]
		public string HomeAddressCountry { get; set; }

		[XmlElement(ElementName = "HomeFaxNumber", Namespace = "Contacts")]
		public string HomeFaxNumber { get; set; }

		[XmlElement(ElementName = "HomePhoneNumber", Namespace = "Contacts")]
		public string HomePhoneNumber { get; set; }

		[XmlElement(ElementName = "Home2PhoneNumber", Namespace = "Contacts")]
		public string Home2PhoneNumber { get; set; }

		[XmlElement(ElementName = "HomeAddressPostalCode", Namespace = "Contacts")]
		public string HomeAddressPostalCode { get; set; }

		[XmlElement(ElementName = "HomeAddressState", Namespace = "Contacts")]
		public string HomeAddressState { get; set; }

		[XmlElement(ElementName = "HomeAddressStreet", Namespace = "Contacts")]
		public string HomeAddressStreet { get; set; }

		[XmlElement(ElementName = "MobilePhoneNumber", Namespace = "Contacts")]
		public string MobilePhoneNumber { get; set; }

		[XmlElement(ElementName = "Suffix", Namespace = "Contacts")]
		public string Suffix { get; set; }

		[XmlElement(ElementName = "CompanyName", Namespace = "Contacts")]
		public string CompanyName { get; set; }

		[XmlElement(ElementName = "OtherAddressCity", Namespace = "Contacts")]
		public string OtherAddressCity { get; set; }

		[XmlElement(ElementName = "OtherAddressCountry", Namespace = "Contacts")]
		public string OtherAddressCountry { get; set; }

		[XmlElement(ElementName = "CarPhoneNumber", Namespace = "Contacts")]
		public string CarPhoneNumber { get; set; }

		[XmlElement(ElementName = "OtherAddressPostalCode", Namespace = "Contacts")]
		public string OtherAddressPostalCode { get; set; }

		[XmlElement(ElementName = "OtherAddressState", Namespace = "Contacts")]
		public string OtherAddressState { get; set; }

		[XmlElement(ElementName = "OtherAddressStreet", Namespace = "Contacts")]
		public string OtherAddressStreet { get; set; }

		[XmlElement(ElementName = "PagerNumber", Namespace = "Contacts")]
		public string PagerNumber { get; set; }

		[XmlElement(ElementName = "Title", Namespace = "Contacts")]
		public string Title { get; set; }

		[XmlElement(ElementName = "BusinessAddressPostalCode", Namespace = "Contacts")]
		public string BusinessAddressPostalCode { get; set; }

		[XmlElement(ElementName = "LastName", Namespace = "Contacts")]
		public string LastName { get; set; }

		[XmlElement(ElementName = "Spouse", Namespace = "Contacts")]
		public string Spouse { get; set; }

		[XmlElement(ElementName = "BusinessAddressState", Namespace = "Contacts")]
		public string BusinessAddressState { get; set; }

		[XmlElement(ElementName = "BusinessAddressStreet", Namespace = "Contacts")]
		public string BusinessAddressStreet { get; set; }

		[XmlElement(ElementName = "JobTitle", Namespace = "Contacts")]
		public string JobTitle { get; set; }

		[XmlElement(ElementName = "YomiFirstName", Namespace = "Contacts")]
		public string YomiFirstName { get; set; }

		[XmlElement(ElementName = "YomiLastName", Namespace = "Contacts")]
		public string YomiLastName { get; set; }

		[XmlElement(ElementName = "YomiCompanyName", Namespace = "Contacts")]
		public string YomiCompanyName { get; set; }

		[XmlElement(ElementName = "OfficeLocation", Namespace = "Contacts")]
		public string OfficeLocation { get; set; }

		[XmlElement(ElementName = "RadioPhoneNumber", Namespace = "Contacts")]
		public string RadioPhoneNumber { get; set; }

		[XmlElement(ElementName = "Picture", Namespace = "Contacts")]
		public string Picture { get; set; }

		[XmlElement(ElementName = "CustomerId", Namespace = "Contacts2")]
		public string CustomerId { get; set; }

		[XmlElement(ElementName = "GovernmentId", Namespace = "Contacts2")]
		public string GovernmentId { get; set; }

		[XmlElement(ElementName = "IMAddress", Namespace = "Contacts2")]
		public string IMAddress { get; set; }

		[XmlElement(ElementName = "IMAddress2", Namespace = "Contacts2")]
		public string IMAddress2 { get; set; }

		[XmlElement(ElementName = "IMAddress3", Namespace = "Contacts2")]
		public string IMAddress3 { get; set; }

		[XmlElement(ElementName = "ManagerName", Namespace = "Contacts2")]
		public string ManagerName { get; set; }

		[XmlElement(ElementName = "CompanyMainPhone", Namespace = "Contacts2")]
		public string CompanyMainPhone { get; set; }

		[XmlElement(ElementName = "AccountName", Namespace = "Contacts2")]
		public string AccountName { get; set; }

		[XmlElement(ElementName = "NickName", Namespace = "Contacts2")]
		public string NickName { get; set; }

		[XmlElement(ElementName = "MMS", Namespace = "Contacts2")]
		public string MMS { get; set; }
	}
}
