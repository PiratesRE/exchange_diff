using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SenderNotifySettings
	{
		[DataMember]
		public string NotifySender { get; set; }

		[DataMember]
		public string RejectMessage { get; set; }
	}
}
