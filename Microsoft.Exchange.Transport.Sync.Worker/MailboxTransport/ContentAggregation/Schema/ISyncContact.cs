using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ISyncContact : ISyncObject, IDisposeTrackable, IDisposable
	{
		ExDateTime? BirthDate { get; }

		ExDateTime? BirthDateLocal { get; }

		string BusinessAddressCity { get; }

		string BusinessAddressCountry { get; }

		string BusinessAddressPostalCode { get; }

		string BusinessAddressState { get; }

		string BusinessAddressStreet { get; }

		string BusinessFaxNumber { get; }

		string BusinessTelephoneNumber { get; }

		string CompanyName { get; }

		string DisplayName { get; }

		string Email1Address { get; }

		string Email2Address { get; }

		string Email3Address { get; }

		string FileAs { get; }

		string FirstName { get; }

		string Hobbies { get; }

		string HomeAddressCity { get; }

		string HomeAddressCountry { get; }

		string HomeAddressPostalCode { get; }

		string HomeAddressState { get; }

		string HomeAddressStreet { get; }

		string HomeTelephoneNumber { get; }

		string IMAddress { get; }

		string JobTitle { get; }

		string LastName { get; }

		string Location { get; }

		string MiddleName { get; }

		string MobileTelephoneNumber { get; }

		string OtherTelephoneNumber { get; }

		byte[] OscContactSources { get; }

		string PartnerNetworkContactType { get; }

		string PartnerNetworkId { get; }

		string PartnerNetworkProfilePhotoUrl { get; }

		string PartnerNetworkThumbnailPhotoUrl { get; }

		string PartnerNetworkUserId { get; }

		ExDateTime? PeopleConnectionCreationTime { get; }

		string ProtectedEmailAddress { get; }

		string ProtectedPhoneNumber { get; }

		string Schools { get; }

		string Webpage { get; }
	}
}
