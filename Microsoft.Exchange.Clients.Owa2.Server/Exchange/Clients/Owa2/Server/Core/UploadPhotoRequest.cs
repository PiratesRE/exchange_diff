using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class UploadPhotoRequest
	{
		[DataMember]
		public UploadPhotoCommand Command { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember(IsRequired = true)]
		public string EmailAddress { get; set; }
	}
}
