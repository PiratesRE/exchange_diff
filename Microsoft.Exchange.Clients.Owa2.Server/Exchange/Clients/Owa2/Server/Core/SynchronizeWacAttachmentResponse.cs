using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class SynchronizeWacAttachmentResponse
	{
		public SynchronizeWacAttachmentResponse(SynchronizeWacAttachmentResult result)
		{
			this.result = result;
		}

		[DataMember(Order = 1)]
		public SynchronizeWacAttachmentResult Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		private SynchronizeWacAttachmentResult result;
	}
}
