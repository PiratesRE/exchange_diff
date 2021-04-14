using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[KnownType(typeof(SplitPlanFolder))]
	[DataContract]
	[Serializable]
	public sealed class PublicFolderSplitPlan : IPublicFolderSplitPlan, IExtensibleDataObject
	{
		[DataMember]
		public List<SplitPlanFolder> FoldersToSplit { get; set; }

		[DataMember]
		public ulong TotalSizeOccupied { get; set; }

		[DataMember]
		public ulong TotalSizeToSplit { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }
	}
}
