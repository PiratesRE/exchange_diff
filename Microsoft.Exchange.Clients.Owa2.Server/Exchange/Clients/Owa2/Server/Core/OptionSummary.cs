using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class OptionSummary
	{
		[DataMember]
		public bool Oof { get; set; }

		[DataMember]
		public string TimeZone { get; set; }

		[DataMember]
		public EmailSignatureConfiguration Signature { get; set; }

		[DataMember]
		public bool AlwaysShowBcc { get; set; }

		[DataMember]
		public bool AlwaysShowFrom { get; set; }

		[DataMember]
		public bool ShowSenderOnTopInListView { get; set; }

		[DataMember]
		public bool ShowPreviewTextInListView { get; set; }
	}
}
