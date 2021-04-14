using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AttachmentDataProviderPermissions
	{
		[DataMember]
		public AttachmentPermissionAssignment[] PermissionAssignments;

		[DataMember]
		public string AttachmentDataProviderId;
	}
}
