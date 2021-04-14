using System;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class DialInRegion
	{
		public string Name { get; set; }

		public string Number { get; set; }

		public Collection<string> Languages { get; set; }
	}
}
