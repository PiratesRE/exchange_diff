using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[DataContract]
	[Serializable]
	public sealed class SplitPlanFolder
	{
		[DataMember]
		public PublicFolderId PublicFolderId { get; set; }

		[DataMember]
		public ulong ContentSize { get; set; }
	}
}
