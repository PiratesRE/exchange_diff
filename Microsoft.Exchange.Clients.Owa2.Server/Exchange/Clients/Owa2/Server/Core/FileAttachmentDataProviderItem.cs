using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FileAttachmentDataProviderItem : AttachmentDataProviderItem
	{
		public FileAttachmentDataProviderItem()
		{
			base.Type = AttachmentDataProviderItemType.File;
		}
	}
}
