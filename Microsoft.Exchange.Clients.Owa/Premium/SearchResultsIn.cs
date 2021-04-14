using System;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[Flags]
	public enum SearchResultsIn
	{
		DefaultFields = 0,
		Subject = 1,
		Body = 2,
		BodyAndSubject = 3
	}
}
