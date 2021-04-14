using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FavoriteFolderNode
	{
		[DataMember]
		internal VersionedId NavigationNodeId { get; set; }

		[DataMember]
		public byte[] NodeOrdinal { get; set; }

		[DataMember]
		public BaseFolderType Folder { get; internal set; }
	}
}
