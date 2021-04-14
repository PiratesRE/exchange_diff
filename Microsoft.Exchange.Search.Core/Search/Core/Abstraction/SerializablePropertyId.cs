using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal enum SerializablePropertyId : byte
	{
		None,
		AnnotationToken,
		Tasks,
		Meetings = 4,
		Addresses,
		Keywords,
		Phones = 9,
		Emails,
		Urls,
		Contacts,
		Language,
		OperatorTiming,
		Mdm,
		Max
	}
}
