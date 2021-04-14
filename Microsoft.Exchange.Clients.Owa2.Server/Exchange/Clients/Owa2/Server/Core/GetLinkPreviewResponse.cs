using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetLinkPreviewResponse
	{
		[DataMember]
		public BaseLinkPreview LinkPreview { get; set; }

		[DataMember]
		public string Error { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public bool IsDisabled { get; set; }

		[DataMember]
		public long ElapsedTimeToWebPageStepCompletion { get; set; }

		[DataMember]
		public long ElapsedTimeToRegExStepCompletion { get; set; }

		[DataMember]
		public long WebPageContentLength { get; set; }

		[DataMember]
		public int ImageTagCount { get; set; }

		[DataMember]
		public int DescriptionTagCount { get; set; }
	}
}
