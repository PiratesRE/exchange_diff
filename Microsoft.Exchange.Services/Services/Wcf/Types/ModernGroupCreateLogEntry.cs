using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	public class ModernGroupCreateLogEntry
	{
		[DataMember(Name = "Delta", IsRequired = false)]
		public float Delta { get; set; }

		[DataMember(Name = "Data", IsRequired = false)]
		public string Data { get; set; }
	}
}
