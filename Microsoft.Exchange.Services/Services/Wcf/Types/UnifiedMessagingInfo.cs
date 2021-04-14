using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UnifiedMessagingInfo : OptionsPropertyChangeTracker
	{
		public string EnableTemplate { get; set; }

		public string DisableTemplate { get; set; }

		public string CallForwardingType { get; set; }
	}
}
