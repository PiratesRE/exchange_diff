using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(OneDriveProItemsPagingMetadata))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(OneDriveProGroupsPagingMetadata))]
	public class AttachmentItemsPagingMetadata
	{
	}
}
