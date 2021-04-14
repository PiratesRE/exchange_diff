using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class ShareInviteFailure
	{
		[DataMember]
		public string Recipient { get; set; }

		[DataMember]
		public string Error { get; set; }
	}
}
