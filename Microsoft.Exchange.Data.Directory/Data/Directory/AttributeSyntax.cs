using System;

namespace Microsoft.Exchange.Data.Directory
{
	public enum AttributeSyntax
	{
		Boolean = 1,
		Integer,
		Sid = 4,
		Octet = 4,
		ObjectIdentifier = 6,
		Enumeration = 10,
		DeliveryMechanism = 10,
		ExportInformationLevel = 10,
		PreferredDeliveryMethod = 10,
		Numeric = 18,
		Printable,
		Teletex,
		IA5 = 22,
		UTCTime,
		GeneralizedTime,
		CaseSensitive = 27,
		Unicode = 64,
		Interval,
		LargeInteger = 65,
		NTSecDesc,
		AccessPoint = 127,
		DNBinary = 127,
		DNString = 127,
		DSDN = 127,
		ORName = 127,
		PresentationAddress = 127,
		ReplicaLink = 127
	}
}
