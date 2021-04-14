using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateAttachmentPermissionsRequest
	{
		[DataMember]
		public AttachmentDataProviderPermissions[] AttachmentDataProviderPermissions;

		[DataMember]
		public string[] UserIds;
	}
}
