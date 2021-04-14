using System;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[Flags]
	public enum AdvancedFindComponents
	{
		Categories = 1,
		FromTo = 2,
		SubjectBody = 4,
		SearchTextInSubject = 8,
		SearchButton = 16,
		SearchTextInName = 32
	}
}
