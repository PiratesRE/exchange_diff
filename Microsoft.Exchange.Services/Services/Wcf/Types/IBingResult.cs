using System;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal interface IBingResult
	{
		string Name { get; }

		double Latitude { get; }

		double Longitude { get; }

		string StreetAddress { get; }

		string City { get; }

		string State { get; }

		string Country { get; }

		string PostalCode { get; }

		LocationSource LocationSource { get; }

		string LocationUri { get; }

		string PhoneNumber { get; }

		string LocalHomePage { get; }

		string BusinessHomePage { get; }
	}
}
