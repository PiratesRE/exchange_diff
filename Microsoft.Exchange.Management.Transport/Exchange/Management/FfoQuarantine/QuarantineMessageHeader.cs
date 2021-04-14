using System;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[Serializable]
	public class QuarantineMessageHeader
	{
		public string Identity { get; set; }

		public string Organization { get; set; }

		public string Header { get; set; }
	}
}
