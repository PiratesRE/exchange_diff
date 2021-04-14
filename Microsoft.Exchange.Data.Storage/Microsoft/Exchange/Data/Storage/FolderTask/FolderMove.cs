using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	[KnownType(typeof(FolderInfo))]
	[DataContract]
	[Serializable]
	public sealed class FolderMove : IFolderMove
	{
		[DataMember]
		public List<FolderInfo> CandidateFolders { get; private set; }

		public ulong TotalSizeOccupiedByTarget { get; private set; }

		[DataMember]
		public ulong TotalSizeToMove { get; private set; }

		public FolderMove(List<FolderInfo> candidateFolders, ulong totalSizeOccupiedByTarget, ulong totalSizeToMove)
		{
			this.CandidateFolders = candidateFolders;
			this.TotalSizeOccupiedByTarget = totalSizeOccupiedByTarget;
			this.TotalSizeToMove = totalSizeToMove;
		}
	}
}
